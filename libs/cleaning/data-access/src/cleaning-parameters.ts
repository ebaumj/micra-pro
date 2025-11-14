import { createMutation, createQuery } from '@micra-pro/shared/utils-ts';
import {
  CleaningCycle,
  CleaningSequenceDocument,
  CleaningSequenceQuery,
  CleaningSequenceQueryVariables,
  ResetCleaningSequenceDocument,
  ResetCleaningSequenceMutation,
  ResetCleaningSequenceMutationVariables,
  SetCleaningSequenceDocument,
  SetCleaningSequenceMutation,
  SetCleaningSequenceMutationVariables,
} from './generated/graphql';
export { type CleaningCycle } from './generated/graphql';

export const cleaningParametersAccessor = () => {
  const sequenceQuery = createQuery<
    CleaningSequenceQuery,
    CleaningSequenceQueryVariables
  >(CleaningSequenceDocument, () => ({}));
  const sequenceMutation = createMutation<
    SetCleaningSequenceMutation,
    SetCleaningSequenceMutationVariables
  >(SetCleaningSequenceDocument);
  const resetSequenceMutation = createMutation<
    ResetCleaningSequenceMutation,
    ResetCleaningSequenceMutationVariables
  >(ResetCleaningSequenceDocument);
  return {
    sequence: () => sequenceQuery.resource.latest?.cleaningSequence ?? [],
    setSequence: (sequence: CleaningCycle[]) =>
      new Promise<void>((resolve, reject) =>
        sequenceMutation({ sequence })
          .then((r) => {
            const cycles = r.setCleaningSequence.cleaningCycle;
            if (cycles) {
              sequenceQuery.resourceActions.mutate((s) => ({
                ...s,
                cleaningSequence: cycles,
              }));
              resolve();
            } else reject();
          })
          .catch(reject),
      ),
    resetSequence: () =>
      new Promise<void>((resolve, reject) =>
        resetSequenceMutation({})
          .then((r) => {
            const cycles = r.resetCleaningSequence.cleaningCycle;
            if (cycles) {
              sequenceQuery.resourceActions.mutate((s) => ({
                ...s,
                cleaningSequence: cycles,
              }));
              resolve();
            } else reject();
          })
          .catch(reject),
      ),
    loading: () => sequenceQuery.resource.state === 'pending',
  };
};
