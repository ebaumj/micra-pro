import { createMutation, createQuery } from '@micra-pro/shared/utils-ts';
import {
  FlowProfileTrackingFieldsFragment,
  FlowProfilingProcessRunning,
  FlowProfilingStateDocument,
  FlowProfilingStatePollDocument,
  FlowProfilingStatePollQuery,
  FlowProfilingStatePollQueryVariables,
  FlowProfilingStateSubscription,
  FlowProfilingStateSubscriptionVariables,
  IsFlowProfilingAvailableDocument,
  IsFlowProfilingAvailableQuery,
  IsFlowProfilingAvailableQueryVariables,
  StartFlowProfilingProcessDocument,
  StartFlowProfilingProcessMutation,
  StartFlowProfilingProcessMutationVariables,
  StopFlowProfilingProcessDocument,
  StopFlowProfilingProcessMutation,
  StopFlowProfilingProcessMutationVariables,
} from './generated/graphql';
import { createStore } from 'solid-js/store';
import { createEffect, createSignal } from 'solid-js';

export const IsFlowProfilingAvailable = () => {
  const query = createQuery<
    IsFlowProfilingAvailableQuery,
    IsFlowProfilingAvailableQueryVariables
  >(IsFlowProfilingAvailableDocument, () => ({}));
  return {
    isAvailable: () => query.resource.latest?.isFlowProfilingAvailable ?? false,
    refetch: () => query.resourceActions.refetch(),
  };
};

export const StartFlowProfiling = (params: {
  startFlow: number;
  dataPoints: { flow: number; time: string }[];
}) => {
  const [processId, setProcessId] = createSignal<string | undefined>();
  const [store, setStore] = createStore<{
    profileData: FlowProfilingProcessRunning[];
    state:
      | FlowProfileTrackingFieldsFragment
      | { __typename: 'Created' }
      | { __typename: 'NotStarted' };
  }>({
    profileData: [],
    state: { __typename: 'Created' },
  });
  const startMutation = createMutation<
    StartFlowProfilingProcessMutation,
    StartFlowProfilingProcessMutationVariables
  >(StartFlowProfilingProcessDocument);
  const stopMutation = createMutation<
    StopFlowProfilingProcessMutation,
    StopFlowProfilingProcessMutationVariables
  >(StopFlowProfilingProcessDocument);
  createEffect(() => {
    const id = processId();
    if (id) {
      createQuery<
        FlowProfilingStatePollQuery,
        FlowProfilingStatePollQueryVariables
      >(FlowProfilingStatePollDocument, () => ({
        processId: id,
      })).subscribeToMore<
        FlowProfilingStateSubscription,
        FlowProfilingStateSubscriptionVariables
      >(
        FlowProfilingStateDocument,
        () => ({ processId: id }),
        // eslint-disable-next-line solid/reactivity
        (newState, _, _s) => {
          setStore('state', newState.flowProfileState);
          if (
            newState.flowProfileState.__typename ===
            'FlowProfilingProcessRunning'
          )
            setStore(
              'profileData',
              store.profileData.length,
              newState.flowProfileState,
            );
        },
      );
    }
  });
  startMutation(params)
    .then((result) => {
      const id = result.startFlowProfilingProcess.uuid;
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
