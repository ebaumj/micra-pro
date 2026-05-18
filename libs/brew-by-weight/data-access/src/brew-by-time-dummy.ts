import {
  BrewByTimeTrackingFieldsFragment,
  BrewByTimeProcessRunning,
} from './generated/graphql';
import { createStore } from 'solid-js/store';
import moment from 'moment';

export const StartCoffeeByTime = (params: { targetTime: string }) => {
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

  let time = 0;
  const total = moment.duration(params.targetTime).asSeconds();
  let running = true;

  const update = () => {
    if (!running) return;
    time += 0.1;
    if (time < total) {
      setStore('brewData', (d) =>
        d.concat([
          {
            targetTime: params.targetTime,
            totalTime: `PT${time}S`,
            _stringValue: '',
          },
        ]),
      );
      setStore('state', {
        __typename: 'BrewByTimeProcessRunning',
        targetTime: params.targetTime,
        totalTime: `PT${time}S`,
      });
    } else {
      running = false;
      setStore('state', {
        __typename: 'BrewByTimeProcessFinished',
        targetTime: params.targetTime,
        extractionTime: `PT${time}S`,
      });
    }
    setTimeout(update, 100);
  };
  setStore('state', { __typename: 'BrewByTimeProcessStarted' });
  setTimeout(update, 100);

  const stop = () => {
    const last =
      store.brewData.length === 0
        ? { targetTime: 0, totalTime: 'PT0S' }
        : store.brewData[store.brewData.length - 1];
    setStore('state', {
      __typename: 'BrewByTimeProcessCancelled',
      targetTime: last.targetTime,
      totalTime: last.totalTime,
    });
    running = false;
  };
  return {
    cancel: stop,
    dataStore: store,
  };
};
