import { getUserRepository } from '../../../utils/database/database_access';
import bcrypt from 'bcrypt';
import jwt from 'jsonwebtoken';

const accessTokenLifetimeInMinutes = 90;
const refreshTokenLifetimeInDays = 30;
const mfaTokenLifetimeInMinutes = 2;
export const mfaIssuer = 'MfaChallenge';

type RequestBodyType =
  | {
      grant_type: 'password';
      username: string;
      password: string;
      client_id: string;
    }
  | {
      grant_type: 'refresh_token';
      refresh_token: string;
      client_id: string;
    };

type ResponseType =
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

export default defineEventHandler(async (event) => {
  const body = (await readBody(event)) as RequestBodyType;
  const runtimeConfig = useRuntimeConfig();
  if (body.grant_type === 'password') {
    const repository = getUserRepository(
      runtimeConfig.secrets.databaseConnectionString,
    );
    try {
      const user = await repository.getByUsername(body.username);
      if (!(await bcrypt.compare(body.password, user.password)))
        return errorEvent(event, wrongPasssword);
      if (body.client_id !== user.clientId && body.client_id)
        await repository.update({ ...user, clientId: body.client_id });
      if (!user.enabled2fa)
        return createResponse(runtimeConfig, user.id, body.client_id);
      return createMfaResponse(runtimeConfig, user.id, body.client_id);
    } catch {
      return errorEvent(event, invalidRequest);
    }
  } else if (body.grant_type === 'refresh_token') {
    try {
      return responseFromRefreshToken(
        event,
        body.refresh_token,
        runtimeConfig,
        body.client_id,
      );
    } catch {
      return errorEvent(event, invalidRequest);
    }
  }
  return errorEvent(event, unsupportedGrant);
});

export function errorEvent<T>(event: any, response: T): T {
  setResponseStatus(event, 400);
  return response;
}

export const invalidRequest = {
  error: 'invalid_request',
  error_description: 'token not valid',
};

const unsupportedGrant = {
  error: 'unsupported_grant_type',
  error_description: 'only password and refresh_token grants are supported',
};

const wrongPasssword = {
  error: 'invalid_grant',
  error_description: 'password not valid',
};

const unauthorizedClient = {
  error: 'unauthorized_client',
  error_description: 'client is not authorized to use refreshToken',
};

const createMfaResponse = (
  config: ReturnType<typeof useRuntimeConfig>,
  userId: string,
  clientId: string,
): ResponseType => {
  const mfaToken = jwt.sign({ clientId: clientId }, config.secrets.privateKey, {
    expiresIn: mfaTokenLifetimeInMinutes * 60,
    issuer: mfaIssuer,
    audience: config.authorization.jwtAudience,
    subject: userId,
  });
  return {
    access_token: mfaToken,
    token_type: 'mfa_challenge',
    expires_in: mfaTokenLifetimeInMinutes * 60,
  };
};

const responseFromRefreshToken = (
  event: any,
  token: string,
  config: ReturnType<typeof useRuntimeConfig>,
  clientId: string,
) => {
  const tokenDecoded = jwt.verify(token, config.secrets.privateKey, {
    audience: config.authorization.jwtAudience,
    issuer: config.authorization.jwtValidIssuers,
  }) as jwt.JwtPayload;
  if (!clientId || tokenDecoded.clientId !== clientId)
    return errorEvent(event, unauthorizedClient);
  if (!tokenDecoded.sub) return errorEvent(event, invalidRequest);
  return createResponse(config, tokenDecoded.sub, clientId);
};

export const createResponse = (
  config: ReturnType<typeof useRuntimeConfig>,
  userId: string,
  clientId: string,
): ResponseType => {
  const accessToken = jwt.sign({}, config.secrets.privateKey, {
    expiresIn: accessTokenLifetimeInMinutes * 60,
    issuer: config.authorization.jwtIssuer,
    audience: config.authorization.jwtAudience,
    subject: userId,
  });
  const refreshToken = jwt.sign(
    { clientId: clientId },
    config.secrets.privateKey,
    {
      expiresIn: refreshTokenLifetimeInDays * 60 * 60 * 24,
      issuer: config.authorization.jwtIssuer,
      audience: config.authorization.jwtAudience,
      subject: userId,
    },
  );
  return {
    access_token: accessToken,
    refresh_token: refreshToken,
    token_type: 'bearer',
    expires_in: accessTokenLifetimeInMinutes * 60,
  };
};
