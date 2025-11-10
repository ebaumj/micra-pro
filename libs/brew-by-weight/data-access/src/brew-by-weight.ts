import { createMutation, createQuery } from '@micra-pro/shared/utils-ts';
import {
  BrewByWeightTrackingFieldsFragment,
  BrewProcessRunning,
  BrewStateDocument,
  BrewStatePollDocument,
  BrewStatePollQuery,
  BrewStatePollQueryVariables,
  BrewStateSubscription,
  BrewStateSubscriptionVariables,
  Spout,
  StartBrewProcessDocument,
  StartBrewProcessMutation,
  StartBrewProcessMutationVariables,
  StopBrewProcessDocument,
  StopBrewProcessMutation,
  StopBrewProcessMutationVariables,
} from './generated/graphql';
import { createStore } from 'solid-js/store';
import { createEffect, createSignal } from 'solid-js';
export { Spout } from './generated/graphql';

export const StartCoffee = (params: {
  beanId: string;
  coffeeQuantity: number;
  grindSetting: number;
  inCupQuantity: number;
  spout: Spout;
  targetExtractionTime: string;
}) => {
  const [processId, setProcessId] = createSignal<string | undefined>();
  const [store, setStore] = createStore<{
    brewData: BrewProcessRunning[];
    state:
      | BrewByWeightTrackingFieldsFragment
      | { __typename: 'Created' }
      | { __typename: 'NotStarted' };
  }>({
    brewData: [],
    state: { __typename: 'Created' },
  });
  const startMutation = createMutation<
    StartBrewProcessMutation,
    StartBrewProcessMutationVariables
  >(StartBrewProcessDocument);
  const stopMutation = createMutation<
    StopBrewProcessMutation,
    StopBrewProcessMutationVariables
  >(StopBrewProcessDocument);
  createEffect(() => {
    const id = processId();
    if (id) {
      createQuery<BrewStatePollQuery, BrewStatePollQueryVariables>(
        BrewStatePollDocument,
        () => ({ processId: id }),
      ).subscribeToMore<BrewStateSubscription, BrewStateSubscriptionVariables>(
        BrewStateDocument,
        () => ({ processId: id }),
        // eslint-disable-next-line solid/reactivity
        (newState, _, _s) => {
          setStore('state', newState.brewState);
          if (newState.brewState.__typename === 'BrewProcessRunning')
            setStore('brewData', store.brewData.length, newState.brewState);
        },
      );
    }
  });
  startMutation(params)
    .then((result) => {
      const id = result.startBrewProcess.uuid;
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
