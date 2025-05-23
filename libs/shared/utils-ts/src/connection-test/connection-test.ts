import { Accessor, createEffect, createSignal } from 'solid-js';
import { createQuery } from '../graphql-client/createQuery';
import {
  TestConnectionDocument,
  TestConnectionQuery,
  TestConnectionQueryVariables,
} from '../generated/graphql';

export const testConnection = (): {
  connectionState: Accessor<'loading' | 'connected' | 'error'>;
  refetch: () => void;
} => {
  var query = createQuery<TestConnectionQuery, TestConnectionQueryVariables>(
    TestConnectionDocument,
    () => ({}),
  );
  const [connected, setConnected] = createSignal<
    'loading' | 'connected' | 'error'
  >('loading');
  createEffect(() => {
    if (query.resource.state === 'ready')
      setConnected(
        query.resource.latest.testConnection ? 'connected' : 'error',
      );
    if (query.resource.error) {
      setConnected('error');
    }
  });
  return {
    connectionState: connected,
    refetch: query.resourceActions.refetch,
  };
};
