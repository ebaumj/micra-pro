import { Accessor } from 'solid-js';
import {
  createMutation,
  createQuery,
  type Duration,
} from '@micra-pro/shared/utils-ts';
import {
  BluetoothScale,
  IsScannungDocument,
  IsScannungPollDocument,
  IsScannungPollQuery,
  IsScannungPollQueryVariables,
  IsScannungSubscription,
  IsScannungSubscriptionVariables,
  ScanForScalesDocument,
  ScanForScalesMutation,
  ScanForScalesMutationVariables,
  ScanResultDocument,
  ScanResultSubscription,
  ScanResultSubscriptionVariables,
  StopScanningDocument,
  StopScanningMutation,
  StopScanningMutationVariables,
} from './generated/graphql';
export { type BluetoothScale } from './generated/graphql';

export const createScanAccessor = (): {
  isScanning: Accessor<boolean>;
  stopScanning: () => void;
} => {
  const stopMutation = createMutation<
    StopScanningMutation,
    StopScanningMutationVariables
  >(StopScanningDocument);
  const isScanningQuery = createQuery<
    IsScannungPollQuery,
    IsScannungPollQueryVariables
  >(IsScannungPollDocument, () => ({}));
  isScanningQuery.subscribeToMore<
    IsScannungSubscription,
    IsScannungSubscriptionVariables
  >(
    IsScannungDocument,
    () => ({}),
    (newData, _, setData) => setData('isScanning', newData.isScanning),
  );
  return {
    isScanning: () => isScanningQuery.resource.latest?.isScanning ?? false,
    stopScanning: () => stopMutation({}),
  };
};

export const scanForScales = (
  maxTime: Duration,
  onScaleDiscovered: (scale: BluetoothScale) => void,
) => {
  createQuery<IsScannungPollQuery, IsScannungPollQueryVariables>(
    IsScannungPollDocument,
    () => ({}),
  ).subscribeToMore<ScanResultSubscription, ScanResultSubscriptionVariables>(
    ScanResultDocument,
    () => ({}),
    (newData, _o, _s) => onScaleDiscovered(newData.scanResult),
  );
  const mutation = createMutation<
    ScanForScalesMutation,
    ScanForScalesMutationVariables
  >(ScanForScalesDocument);
  mutation({ maxScanTime: maxTime?.toISOString() ?? null });
};
