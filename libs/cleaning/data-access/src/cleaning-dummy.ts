import { createConfigAccessor } from '@micra-pro/shared/utils-ts';
import { CleaningStatePollQuery } from './generated/graphql';
import { createEffect, createSignal } from 'solid-js';
export { type CleaningStateFieldsFragment as CleaningState } from './generated/graphql';

export const cleaningAccessor = () => {
  const [config, setConfig] = createSignal({
    interval: 'P30D',
    lastCleaningDate: new Date('2000-01-01T00:00:00.000Z'),
  });
  const storage = localStorage['CleaningConfig'];
  if (storage) {
    try {
      const c = JSON.parse(storage);
      setConfig({
        interval: c.interval,
        lastCleaningDate: new Date(c.lastCleaningDate),
      });
    } catch {
      // Data does not match
    }
  }
  createEffect(
    () => (localStorage['CleaningConfig'] = JSON.stringify(config())),
  );
  const reminder = createConfigAccessor<{ doRemind: boolean }>(
    'CleaningReminder',
  );

  const [state, setState] = createSignal<CleaningStatePollQuery>({
    cleaningState: { __typename: 'CleaningStateStarted' },
  });
  const [active, setActive] = createSignal(false);

  const start = () => {
    let i = 0;
    const update = () => {
      i++;
      if (i < 40 && active()) {
        setState({
          cleaningState: {
            __typename: 'CleaningStateRunning',
            cycleNumber: 0,
            cycleTime: `PT${i / 10}S`,
            totalTime: `PT${i / 10}S`,
          },
        });
      } else {
        setState({
          cleaningState: {
            __typename: 'CleaningStateFinished',
            totalTime: `PT4S`,
            totalCycles: 1,
          },
        });
        setActive(false);
        setConfig((c) => ({ ...c, lastCleaningDate: new Date() }));
      }
      setTimeout(update, 100);
    };
    setState({ cleaningState: { __typename: 'CleaningStateStarted' } });
    setActive(true);
    setTimeout(update, 100);
    return Promise.resolve();
  };

  const stop = () => {
    setState({
      cleaningState: {
        __typename: 'CleaningStateCancelled',
        totalCycles: 0,
        totalTime: 0,
      },
    });
    setActive(false);
    return Promise.resolve();
  };

  return {
    interval: () => config().interval,
    lastCleaningDate: () => config().lastCleaningDate,
    doRemind: () => reminder.config()?.doRemind ?? true,
    setDoRemind: (doRemind: boolean) => reminder.writeConfig({ doRemind }),
    setInterval: (interval: any) => {
      setConfig((c) => ({ ...c, interval: interval }));
      return Promise.resolve();
    },
    state: () => state().cleaningState,
    isCleaning: active,
    start,
    stop,
  };
};
