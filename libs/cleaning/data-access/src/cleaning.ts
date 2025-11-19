import {
  createConfigAccessor,
  createMutation,
  createQuery,
} from '@micra-pro/shared/utils-ts';
import {
  CleaningIntervalDocument,
  CleaningIntervalQuery,
  CleaningIntervalQueryVariables,
  CleaningStateDocument,
  CleaningStatePollDocument,
  CleaningStatePollQuery,
  CleaningStatePollQueryVariables,
  CleaningStateSubscription,
  CleaningStateSubscriptionVariables,
  IsCleaningActiveDocument,
  IsCleaningActivePollDocument,
  IsCleaningActivePollQuery,
  IsCleaningActivePollQueryVariables,
  IsCleaningActiveSubscription,
  IsCleaningActiveSubscriptionVariables,
  LastCleaningTimeDocument,
  LastCleaningTimeQuery,
  LastCleaningTimeQueryVariables,
  SetCleaningIntervalDocument,
  SetCleaningIntervalMutation,
  SetCleaningIntervalMutationVariables,
  StartCleaningDocument,
  StartCleaningMutation,
  StartCleaningMutationVariables,
  StopCleaningDocument,
  StopCleaningMutation,
  StopCleaningMutationVariables,
} from './generated/graphql';
import { createEffect } from 'solid-js';
export { type CleaningStateFieldsFragment as CleaningState } from './generated/graphql';

export const cleaningAccessor = () => {
  const reminder = createConfigAccessor<{ doRemind: boolean }>(
    'CleaningReminder',
  );
  const intervalQuery = createQuery<
    CleaningIntervalQuery,
    CleaningIntervalQueryVariables
  >(CleaningIntervalDocument, () => ({}));
  const intervalMutation = createMutation<
    SetCleaningIntervalMutation,
    SetCleaningIntervalMutationVariables
  >(SetCleaningIntervalDocument);
  const stateQuery = createQuery<
    CleaningStatePollQuery,
    CleaningStatePollQueryVariables
  >(CleaningStatePollDocument, () => ({}));
  stateQuery.subscribeToMore<
    CleaningStateSubscription,
    CleaningStateSubscriptionVariables
  >(
    CleaningStateDocument,
    () => ({}),
    (next, _, set) => set(next),
  );
  const isCleaningQuery = createQuery<
    IsCleaningActivePollQuery,
    IsCleaningActivePollQueryVariables
  >(IsCleaningActivePollDocument, () => ({}));
  isCleaningQuery.subscribeToMore<
    IsCleaningActiveSubscription,
    IsCleaningActiveSubscriptionVariables
  >(
    IsCleaningActiveDocument,
    () => ({}),
    (next, _, set) => set(next),
  );
  const lastDateQuery = createQuery<
    LastCleaningTimeQuery,
    LastCleaningTimeQueryVariables
  >(LastCleaningTimeDocument, () => ({}));
  const startMutation = createMutation<
    StartCleaningMutation,
    StartCleaningMutationVariables
  >(StartCleaningDocument);
  const stopMutation = createMutation<
    StopCleaningMutation,
    StopCleaningMutationVariables
  >(StopCleaningDocument);
  const refetchLastDate = async () => {
    const lastDate = await lastDateQuery.resourceActions.refetch();
    if (lastDate?.lastCleaningTime)
      lastDateQuery.resourceActions.mutate((d) => ({
        ...d,
        lastCleaningTime: lastDate.lastCleaningTime,
      }));
  };
  createEffect(
    () =>
      isCleaningQuery.resource.latest?.isCleaningActive === false &&
      refetchLastDate(),
  );
  return {
    interval: () => intervalQuery.resource.latest?.cleaningInterval,
    lastCleaningDate: () =>
      lastDateQuery.resource.latest?.lastCleaningTime
        ? new Date(lastDateQuery.resource.latest.lastCleaningTime)
        : new Date(),
    doRemind: () => reminder.config()?.doRemind ?? true,
    setDoRemind: (doRemind: boolean) => reminder.writeConfig({ doRemind }),
    setInterval: (interval: any) =>
      new Promise<void>((resolve, reject) =>
        intervalMutation({ interval })
          .then((r) => {
            const interval = r.setCleaningInterval.timeSpan;
            if (interval) {
              intervalQuery.resourceActions.mutate((i) => ({
                ...i,
                cleaningInterval: interval,
              }));
              resolve();
            } else reject();
          })
          .catch(reject),
      ),
    state: () =>
      stateQuery.resource.latest?.cleaningState ?? {
        __typename: 'CleaningStateStarted',
      },
    isCleaning: () =>
      isCleaningQuery.resource.latest?.isCleaningActive ?? false,
    start: () =>
      new Promise<void>((resolve, reject) =>
        startMutation({})
          .then(() => resolve)
          .catch(reject),
      ),
    stop: () =>
      new Promise<void>((resolve, reject) =>
        stopMutation({})
          .then(() => resolve)
          .catch(reject),
      ),
  };
};
