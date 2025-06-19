import { createMutation, createQuery } from '@micra-pro/shared/utils-ts';
import {
  BrewByWeightHistoryDocument,
  BrewByWeightHistoryQuery,
  BrewByWeightHistoryQueryVariables,
  CleanupBrewByWeightHistoryDocument,
  CleanupBrewByWeightHistoryMutation,
  CleanupBrewByWeightHistoryMutationVariables,
  RemoveHistoryEntryDocument,
  RemoveHistoryEntryMutation,
  RemoveHistoryEntryMutationVariables,
} from './generated/graphql';
export {
  type BrewByWeightHistoryFieldsFragment as BrewByWeightHistoryEntry,
  type HistoryEntryProcessFinished,
  type HistoryEntryProcessCancelled,
  type HistoryEntryProcessFailed,
} from './generated/graphql';

export const createHistoryAccessor = () => {
  const query = createQuery<
    BrewByWeightHistoryQuery,
    BrewByWeightHistoryQueryVariables
  >(BrewByWeightHistoryDocument, () => ({}));
  const removeMutation = createMutation<
    RemoveHistoryEntryMutation,
    RemoveHistoryEntryMutationVariables
  >(RemoveHistoryEntryDocument);
  const cleanup = createMutation<
    CleanupBrewByWeightHistoryMutation,
    CleanupBrewByWeightHistoryMutationVariables
  >(CleanupBrewByWeightHistoryDocument);
  const remove = (id: string) =>
    removeMutation({ entryId: id }).then((result) =>
      query.setDataStore('brewByWeightHistory', (entires) =>
        entires.filter((e) => e.id !== result.removeHistoryEntry.uuid),
      ),
    );
  return {
    finished: () =>
      query.resource.latest?.brewByWeightHistory.filter(
        (h) => h.__typename === 'HistoryEntryProcessFinished',
      ) ?? [],
    cancelled: () =>
      query.resource.latest?.brewByWeightHistory.filter(
        (h) => h.__typename === 'HistoryEntryProcessCancelled',
      ) ?? [],
    failed: () =>
      query.resource.latest?.brewByWeightHistory.filter(
        (h) => h.__typename === 'HistoryEntryProcessFailed',
      ) ?? [],
    cleanup: () =>
      cleanup({ keepLatestDistinctByProcessInputs: true }).then((result) => {
        const updated =
          result.cleanupBrewByWeightHistory.brewByWeightHistoryEntry;
        if (updated) query.setDataStore('brewByWeightHistory', updated);
      }),
    remove: (id: string) => remove(id),
    initalLoad: () => query.resource.state === 'pending',
  };
};
