import { createConfigAccessor } from '@micra-pro/shared/utils-ts';
import {
  Accessor,
  createContext,
  createEffect,
  createSignal,
  on,
  ParentComponent,
  Setter,
  useContext,
} from 'solid-js';
import { v4 as uuid } from 'uuid';

const refreshPollIntervalSeconds = 10;

type RecipeHubAuthorization = {
  experiation: Date;
  accessToken: string;
  refreshToken: string;
  user: string;
};

type RecipeHubClientContextType = {
  authorization: Accessor<RecipeHubAuthorization | null>;
  setAuthorization: Setter<RecipeHubAuthorization | null>;
  clientId: string;
  url: string;
};

const RecipeHubClientContext = createContext<RecipeHubClientContextType>();

const recipeHubRequest = (
  ctx: RecipeHubClientContextType,
  requestUrl: string,
  method?: string,
  body?: XMLHttpRequestBodyInit,
) => {
  const authorization = ctx.authorization();
  return fetch(
    `${ctx.url}/${requestUrl}${authorization ? `?token=${authorization.accessToken}` : ''}`,
    {
      method: method,
      body: body,
    },
  );
};

type TokenResponseSuccess =
  | {
      access_token: string;
      refresh_token: string;
      token_type: 'bearer';
      expires_in: number;
    }
  | {
      access_token: string;
      token_type: 'mfa_challenge';
      expires_in: number;
    };

type TokenResponseFailed =
  | {
      error: 'invalid_request';
    }
  | {
      error: 'unsupported_grant_type';
    }
  | {
      error: 'invalid_grant';
    }
  | {
      error: 'unauthorized_client';
    };

const mfaChallenge = async (
  ctx: RecipeHubClientContextType,
  mfaCode: string,
  mfaToken: string,
  username: string,
): Promise<
  { result: 'success' } | { result: 'unauthorized' } | { result: 'failed' }
> => {
  const response = await fetch(`${ctx.url}/token/mfa?token=${mfaToken}`, {
    method: 'POST',
    body: new URLSearchParams({ code: mfaCode }),
  });
  if (response.status === 200) {
    const result = (await response.json()) as TokenResponseSuccess;
    const exp = new Date();
    exp.setSeconds(exp.getSeconds() + result.expires_in);
    switch (result.token_type) {
      case 'bearer':
        ctx.setAuthorization({
          user: username,
          accessToken: result.access_token,
          refreshToken: result.refresh_token,
          experiation: exp,
        });
        return { result: 'success' };
      case 'mfa_challenge':
        return { result: 'failed' };
    }
  } else if (response.status === 400) {
    const result = (await response.json()) as TokenResponseFailed;
    switch (result.error) {
      case 'invalid_request':
      case 'unsupported_grant_type':
        return { result: 'failed' };
      case 'invalid_grant':
      case 'unauthorized_client':
        return { result: 'unauthorized' };
    }
  }
  return { result: 'failed' };
};

const refreshAccessToken = async (
  ctx: RecipeHubClientContextType,
): Promise<void> => {
  const authorization = ctx.authorization();
  const now = new Date();
  now.setSeconds(now.getSeconds() + 5 * refreshPollIntervalSeconds);
  if (authorization && now > new Date(authorization.experiation)) {
    try {
      const response = await fetch(`${ctx.url}/token`, {
        method: 'POST',
        body: new URLSearchParams({
          grant_type: 'refresh_token',
          refresh_token: authorization.refreshToken,
          client_id: ctx.clientId,
        }),
      });
      if (response.status === 200) {
        const result = (await response.json()) as TokenResponseSuccess;
        const exp = new Date();
        exp.setSeconds(exp.getSeconds() + result.expires_in);
        if (result.token_type === 'bearer')
          ctx.setAuthorization({
            user: authorization.user,
            accessToken: result.access_token,
            refreshToken: result.refresh_token,
            experiation: exp,
          });
      }
    } catch (e) {
      console.log(e);
      if (
        new Date() > new Date(authorization.experiation) &&
        ctx.authorization()
      )
        ctx.setAuthorization(null);
    }
  }
  setTimeout(() => refreshAccessToken(ctx), refreshPollIntervalSeconds * 1000);
};

const authorize = async (
  ctx: RecipeHubClientContextType,
  username: string,
  password: string,
): Promise<
  | { result: 'success' }
  | { result: 'unauthorized' }
  | { result: 'failed' }
  | {
      result: 'mfa';
      due: number;
      mfa: (
        mfaCode: string,
      ) => Promise<
        | { result: 'success' }
        | { result: 'unauthorized' }
        | { result: 'failed' }
      >;
    }
> => {
  const response = await fetch(`${ctx.url}/token`, {
    method: 'POST',
    body: new URLSearchParams({
      grant_type: 'password',
      username: username,
      password: password,
      client_id: ctx.clientId,
    }),
  });
  if (response.status === 200) {
    const result = (await response.json()) as TokenResponseSuccess;
    const exp = new Date();
    exp.setSeconds(exp.getSeconds() + result.expires_in);
    switch (result.token_type) {
      case 'bearer':
        ctx.setAuthorization({
          user: username,
          accessToken: result.access_token,
          refreshToken: result.refresh_token,
          experiation: exp,
        });
        return { result: 'success' };
      case 'mfa_challenge':
        return {
          result: 'mfa',
          due: result.expires_in,
          mfa: (mfaCode: string) =>
            mfaChallenge(ctx, mfaCode, result.access_token, username),
        };
    }
  } else if (response.status === 400) {
    const result = (await response.json()) as TokenResponseFailed;
    switch (result.error) {
      case 'invalid_request':
      case 'unsupported_grant_type':
        return { result: 'failed' };
      case 'invalid_grant':
      case 'unauthorized_client':
        return { result: 'unauthorized' };
    }
  }
  return { result: 'failed' };
};

export const getClientId = () => {
  const ctx = useContext(RecipeHubClientContext);
  if (!ctx) throw new Error('Failed to read Recipe Hub Context!');
  return ctx.clientId;
};

export const useRecipeHubClient = () => {
  const ctx = useContext(RecipeHubClientContext);
  if (!ctx) throw new Error('Failed to read Recipe Hub Context!');
  return {
    user: () => ctx.authorization()?.user,
    login: (username: string, password: string) =>
      authorize(ctx, username, password),
    logout: () => ctx.setAuthorization(null),
    apiCall: (
      requestUrl: string,
      method?: string,
      body?: XMLHttpRequestBodyInit,
    ) => recipeHubRequest(ctx, requestUrl, method, body),
  };
};

const RecipeHubClient: ParentComponent = (props) => {
  const configAccessor = createConfigAccessor<{
    clientId: string;
    authorization?: RecipeHubAuthorization;
  }>('RecipeHub');
  const ctx = useContext(RecipeHubClientContext)!;
  createEffect(
    on(configAccessor.loading, (loading) => {
      if (!loading) {
        const config = configAccessor.config();
        if (config) {
          ctx.clientId = config.clientId;
          if (config.authorization) ctx.setAuthorization(config.authorization);
        } else {
          const clientId = uuid();
          configAccessor.writeConfig({ clientId: clientId });
          ctx.clientId = clientId;
        }
        createEffect(() =>
          configAccessor.writeConfig({
            clientId: ctx.clientId,
            authorization: ctx.authorization() ?? undefined,
          }),
        );
        refreshAccessToken(ctx);
      }
    }),
  );
  return <>{props.children}</>;
};

export const RecipeHubClientProvider: ParentComponent<{
  url: string;
}> = (props) => {
  const [authorization, setAuthorization] =
    createSignal<RecipeHubAuthorization | null>(null);
  return (
    <RecipeHubClientContext.Provider
      value={{
        clientId: '',
        // eslint-disable-next-line solid/reactivity
        url: props.url,
        authorization: authorization,
        setAuthorization: setAuthorization,
      }}
    >
      <RecipeHubClient>{props.children}</RecipeHubClient>
    </RecipeHubClientContext.Provider>
  );
};
