import {
  Accessor,
  createContext,
  createMemo,
  ParentComponent,
  useContext,
} from 'solid-js';
import { createClient } from 'graphql-ws';

const createGraphQlClient = (
  url: Accessor<string>,
  token: Accessor<string>,
  wsUrl: Accessor<string>,
) => {
  const client = createMemo(() =>
    createClient({
      url: wsUrl(),
      connectionParams: {
        authorization: `Bearer ${token()}`,
      },
    }),
  );

  return {
    url,
    token,
    client,
  };
};

export const GraphQlClientContext =
  createContext<ReturnType<typeof createGraphQlClient>>();

export type GraphQlProviderProps = {
  url: string;
  wsUrl: string;
  token: string;
};

export const GraphQlProvider: ParentComponent<GraphQlProviderProps> = (
  props,
) => {
  const context = createGraphQlClient(
    () => props.url,
    () => props.token,
    () => props.wsUrl,
  );

  return (
    <GraphQlClientContext.Provider value={context}>
      {props.children}
    </GraphQlClientContext.Provider>
  );
};

export const useGraphQl = () => {
  const context = useContext(GraphQlClientContext);
  if (!context) {
    throw new Error(
      'GraphQl context could not be found. Did you wrap your Component with GraphQlProvider?',
    );
  }
  return context;
};
