import { type Duration } from '@micra-pro/shared/ui';
import { Accessor, createSignal } from 'solid-js';
import { createMutation } from '@micra-pro/shared/utils-ts';
import {
  ScanForScalesDocument,
  ScanForScalesMutation,
  ScanForScalesMutationVariables,
} from './generated/graphql';

export type ScanProcess =
  | {
      state: 'scanning';
      stop: () => void;
    }
  | {
      state: 'finished';
      devices: string[];
    }
  | {
      state: 'error';
      debugInformation: any;
    };

export const scanForScales = (maxTime?: Duration): Accessor<ScanProcess> => {
  const mutation = createMutation<
    ScanForScalesMutation,
    ScanForScalesMutationVariables
  >(ScanForScalesDocument);
  const stop = () => {
    setState({ state: 'finished', devices: [] });
  };
  const [state, setState] = createSignal<ScanProcess>({
    state: 'scanning',
    stop,
  });
  mutation({ maxScanTime: maxTime?.toISOString() ?? null })
    .then((result) =>
      setState((state) =>
        state.state === 'finished'
          ? state
          : { state: 'finished', devices: result.scanForScales.string ?? [] },
      ),
    )
    .catch((error) =>
      setState((state) =>
        state.state === 'finished'
          ? state
          : { state: 'error', debugInformation: error },
      ),
    );
  return state;
};
