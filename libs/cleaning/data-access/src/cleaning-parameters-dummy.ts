import { createEffect, createSignal } from 'solid-js';
export { type CleaningCycle } from './generated/graphql';
import { type CleaningCycle } from './generated/graphql';

export const cleaningParametersAccessor = () => {
  const defaultSequence = [{ paddleOnTime: 'PT2S', paddleOffTime: 'PT2S' }];
  const [sequence, setSequence] =
    createSignal<CleaningCycle[]>(defaultSequence);
  const storage = localStorage['CleaningSequence'];
  if (storage)
    try {
      setSequence(JSON.parse(storage).sequence);
    } catch {
      // Data does not match
    }
  createEffect(
    () =>
      (localStorage['CleaningSequence'] = JSON.stringify({
        sequence: sequence(),
      })),
  );
  return {
    sequence,
    setSequence,
    resetSequence: () => setSequence(defaultSequence),
    loading: () => false,
  };
};
