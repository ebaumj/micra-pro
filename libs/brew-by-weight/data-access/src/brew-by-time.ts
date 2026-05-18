import { createMutation, createQuery } from '@micra-pro/shared/utils-ts';
import {
  BrewByTimeTrackingFieldsFragment,
  BrewByTimeProcessRunning,
  BrewByTimeStateDocument,
  BrewByTimeStatePollDocument,
  BrewByTimeStatePollQuery,
  BrewByTimeStatePollQueryVariables,
  BrewByTimeStateSubscription,
  BrewByTimeStateSubscriptionVariables,
  StartBrewByTimeProcessDocument,
  StartBrewByTimeProcessMutation,
  StartBrewByTimeProcessMutationVariables,
  StopBrewProcessDocument,
  StopBrewProcessMutation,
  StopBrewProcessMutationVariables,
} from './generated/graphql';
import { createStore } from 'solid-js/store';
import { createEffect, createSignal } from 'solid-js';
export { Spout } from './generated/graphql';

export const StartCoffeeByTime = (params: { targetTime: string }) => {
  const [processId, setProcessId] = createSignal<string | undefined>();
  const [store, setStore] = createStore<{
    brewData: BrewByTimeProcessRunning[];
    state:
      | BrewByTimeTrackingFieldsFragment
      | { __typename: 'Created' }
      | { __typename: 'NotStarted' };
  }>({
    brewData: [],
    state: { __typename: 'Created' },
  });
  const startMutation = createMutation<
    StartBrewByTimeProcessMutation,
    StartBrewByTimeProcessMutationVariables
  >(StartBrewByTimeProcessDocument);
  const stopMutation = createMutation<
    StopBrewProcessMutation,
    StopBrewProcessMutationVariables
  >(StopBrewProcessDocument);
  createEffect(() => {
    const id = processId();
    if (id) {
      createQuery<BrewByTimeStatePollQuery, BrewByTimeStatePollQueryVariables>(
        BrewByTimeStatePollDocument,
        () => ({ processId: id }),
      ).subscribeToMore<
        BrewByTimeStateSubscription,
        BrewByTimeStateSubscriptionVariables
      >(
        BrewByTimeStateDocument,
        () => ({ processId: id }),
        // eslint-disable-next-line solid/reactivity
        (newState, _, _s) => {
          setStore('state', newState.brewByTimeState);
          if (
            newState.brewByTimeState.__typename === 'BrewByTimeProcessRunning'
          )
            setStore(
              'brewData',
              store.brewData.length,
              newState.brewByTimeState,
            );
        },
      );
    }
  });
  startMutation(params)
    .then((result) => {
      const id = result.startBrewByTimeProcess.uuid;
      if (id) setProcessId(id);
      else fail();
    })
    .catch(() => fail());
  const fail = () => setStore('state', { __typename: 'NotStarted' });
  const stop = () => {
    const id = processId();
    if (id) stopMutation({ processId: id });
  };
  return {
    cancel: stop,
    dataStore: store,
  };
};
