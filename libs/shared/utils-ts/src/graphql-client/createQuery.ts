import { Accessor, createResource, createSignal, onCleanup } from 'solid-js';
import { DocumentNode, ExecutionResult, GraphQLError } from 'graphql';
import { useGraphQl } from './GraphQlClientProvider';
import { createStore, reconcile, SetStoreFunction } from 'solid-js/store';

class GraphQlError extends Error {
  constructor(public graphQlErrors: readonly GraphQLError[]) {
    super(graphQlErrors.map((error) => error.message).join('\n'));
    this.name = 'GraphQlError';
  }
}

type MergeFunction<TData, TNextData> = (
  nextData: TNextData,
  prevData: TData,
  setData: SetStoreFunction<TData>,
) => void;

export const createQuery = <TData extends object, TVariables>(
  queryDocument: DocumentNode,
  variables: Accessor<TVariables>,
) => {
  const context = useGraphQl();

  const [subscriptionData, setSubscriptionData] = createSignal<{
    document: DocumentNode;
    variables: Accessor<Record<string, unknown> | null>;
    merge: MergeFunction<TData, object>;
  } | null>(null);

  let unsubscribe: (() => void) | null = null;

  const getQueryData = () => ({
    url: context.url(),
    queryVariables: variables(),
    subscriptionData: subscriptionData(),
    subscriptionDocument: subscriptionData()?.document,
    subscriptionVariables: subscriptionData()?.variables(),
    mergeFunction: subscriptionData()?.merge,
  });

  const [dataStore, setDataStore] = createStore<TData>({} as TData);

  const [resource, resourceActions] = createResource<
    TData,
    ReturnType<typeof getQueryData>
  >(getQueryData, async (data) => {
    unsubscribe?.();

    const response = await fetch(data.url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        query: queryDocument.loc?.source.body,
        variables: data.queryVariables,
      }),
    });

    const result = (await response.json()) as ExecutionResult<TData>;

    if (result.errors && result.errors.length > 0) {
      throw new GraphQlError(result.errors);
    }
    if (!result.data) {
      throw new Error('No data returned');
    }

    if (result.data) setDataStore(reconcile(result.data));

    if (!data.subscriptionData) {
      return dataStore;
    }

    unsubscribe = context.client().subscribe(
      {
        query: data.subscriptionDocument?.loc?.source.body ?? '',
        variables: data.subscriptionVariables,
      },
      {
        next(value: ExecutionResult<object>) {
          if (value.data) {
            data.subscriptionData?.merge(value.data, dataStore, setDataStore);
          }
          if (value.errors) {
            // TODO: Log errors
          }
        },
        error(_: Error) {
          // TODO: Log error
        },
        complete() {
          return;
        },
      },
    );

    return dataStore;
  });

  const subscribeToMore = <
    TSubscriptionData extends object,
    TSubscriptionVariables extends Record<string, unknown> | null,
  >(
    subscriptionDocument: DocumentNode,
    subscriptionVariables: Accessor<TSubscriptionVariables>,
    merge: MergeFunction<TData, TSubscriptionData>,
  ) => {
    setSubscriptionData({
      document: subscriptionDocument,
      variables: subscriptionVariables,
      merge: merge as MergeFunction<TData, object>,
    });
  };

  onCleanup(() => unsubscribe?.());

  return {
    resource,
    resourceActions,
    subscribeToMore,
    setDataStore,
  };
};
