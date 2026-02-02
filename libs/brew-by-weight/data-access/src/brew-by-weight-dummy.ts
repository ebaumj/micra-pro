import {
  BrewByWeightTrackingFieldsFragment,
  BrewProcessRunning,
  Spout,
} from './generated/graphql';
import { createStore } from 'solid-js/store';
export { Spout } from './generated/graphql';
import moment from 'moment';
import {
  addHistoryCancelledInternal,
  addHistoryFinishedInternal,
} from './history-dummy';

export const StartCoffee = (params: {
  beanId: string;
  coffeeQuantity: number;
  grindSetting: number;
  inCupQuantity: number;
  spout: Spout;
  targetExtractionTime: string;
}) => {
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

  const flow =
    params.inCupQuantity /
    moment.duration(params.targetExtractionTime).asSeconds();
  let quantity = 0;
  let running = true;

  const update = () => {
    if (!running) return;
    quantity += flow / 10;
    const totalTime = `PT${quantity / flow}S`;
    if (quantity < params.inCupQuantity) {
      setStore('brewData', (d) =>
        d.concat([
          {
            flow: flow + (Math.random() - 0.5) / 10,
            totalQuantity: quantity,
            totalTime,
            _stringValue: '',
          },
        ]),
      );
      setStore('state', {
        __typename: 'BrewProcessRunning',
        flow,
        totalQuantity: quantity,
        totalTime,
      });
    } else {
      running = false;
      setStore('state', {
        __typename: 'BrewProcessFinished',
        averageFlow: flow,
        totalQuantity: quantity,
        extractionTime: totalTime,
      });
      addHistoryFinishedInternal({
        ...params,
        averageFlow: flow,
        extractionTime: totalTime,
        totalQuantity: quantity,
        runtimeData: store.brewData,
      });
    }
    setTimeout(update, 100);
  };
  setStore('state', { __typename: 'BrewProcessStarted' });
  setTimeout(update, 100);

  const stop = () => {
    const last =
      store.brewData.length === 0
        ? { totalQuantity: 0, totalTime: 'PT0S' }
        : store.brewData[store.brewData.length - 1];
    setStore('state', {
      __typename: 'BrewProcessCancelled',
      averageFlow: flow,
      totalQuantity: last.totalQuantity,
      totalTime: last.totalTime,
    });
    addHistoryCancelledInternal({
      ...params,
      averageFlow: flow,
      totalTime: last.totalQuantity,
      totalQuantity: quantity,
      runtimeData: store.brewData,
    });
    running = false;
  };
  return {
    cancel: stop,
    dataStore: store,
  };
};
