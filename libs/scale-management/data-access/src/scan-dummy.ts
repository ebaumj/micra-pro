import { Accessor } from 'solid-js';
import { BluetoothScale } from './generated/graphql';
export { type BluetoothScale } from './generated/graphql';
import moment from 'moment';

export const createScanAccessor = (): {
  isScanning: Accessor<boolean>;
  stopScanning: () => void;
} => {
  return {
    isScanning: () => false,
    stopScanning: () => undefined,
  };
};

export const scanForScales = (
  _: moment.Duration,
  onScaleDiscovered: (scale: BluetoothScale) => void,
) => {
  onScaleDiscovered({ id: 'dummy1', name: 'Dummy Scale 1' });
  onScaleDiscovered({ id: 'dummy2', name: 'Dummy Scale 2' });
};
