import { createMutation, createQuery } from '@micra-pro/shared/utils-ts';
import {
  BoilersDocument,
  BoilersQuery,
  BoilersQueryVariables,
  ConnectMachineDocument,
  ConnectMachineMutation,
  ConnectMachineMutationVariables,
  DisconnectMachineDocument,
  DisconnectMachineMutation,
  DisconnectMachineMutationVariables,
  IsScanningDocument,
  IsScanningPollDocument,
  IsScanningPollQuery,
  IsScanningPollQueryVariables,
  IsScanningSubscription,
  IsScanningSubscriptionVariables,
  MachineState,
  ScanDevicesDocument,
  ScanDevicesMutation,
  ScanDevicesMutationVariables,
  ScanResultsDocument,
  ScanResultsPollDocument,
  ScanResultsPollQuery,
  ScanResultsPollQueryVariables,
  ScanResultsSubscription,
  ScanResultsSubscriptionVariables,
  SetCoffeeTemperatureDocument,
  SetCoffeeTemperatureMutation,
  SetCoffeeTemperatureMutationVariables,
  SetSmartStandbyDocument,
  SetSmartStandbyMutation,
  SetSmartStandbyMutationVariables,
  SetStandbyDocument,
  SetStandbyMutation,
  SetStandbyMutationVariables,
  SetSteamEnabledDocument,
  SetSteamEnabledMutation,
  SetSteamEnabledMutationVariables,
  SetSteamLevelDocument,
  SetSteamLevelMutation,
  SetSteamLevelMutationVariables,
  SmartStandby,
  SmartStandbyDocument,
  SmartStandbyQuery,
  SmartStandbyQueryVariables,
  StateDocument,
  StatePollDocument,
  StatePollQuery,
  StatePollQueryVariables,
  StateSubscription,
  StateSubscriptionVariables,
  StopScanningDocument,
  StopScanningMutation,
  StopScanningMutationVariables,
} from './generated/graphql';
export {
  type Boilers,
  type SmartStandby,
  MachineState,
  SmartStandbyMode,
} from './generated/graphql';

export const useMachineAccessor = () => {
  const smartStandbyQuery = createQuery<
    SmartStandbyQuery,
    SmartStandbyQueryVariables
  >(SmartStandbyDocument, () => ({}));
  const boilersQuery = createQuery<BoilersQuery, BoilersQueryVariables>(
    BoilersDocument,
    () => ({}),
  );
  const stateQuery = createQuery<StatePollQuery, StatePollQueryVariables>(
    StatePollDocument,
    () => ({}),
  );
  stateQuery.subscribeToMore<StateSubscription, StateSubscriptionVariables>(
    StateDocument,
    () => ({}),
    (newData, _, setData) => setData(newData),
  );
  const setStateMutation = createMutation<
    SetStandbyMutation,
    SetStandbyMutationVariables
  >(SetStandbyDocument);
  const connectMutation = createMutation<
    ConnectMachineMutation,
    ConnectMachineMutationVariables
  >(ConnectMachineDocument);
  const disconnectMutation = createMutation<
    DisconnectMachineMutation,
    DisconnectMachineMutationVariables
  >(DisconnectMachineDocument);
  const setSmartStandbyMutation = createMutation<
    SetSmartStandbyMutation,
    SetSmartStandbyMutationVariables
  >(SetSmartStandbyDocument);
  const setCoffeeTemperatureMutation = createMutation<
    SetCoffeeTemperatureMutation,
    SetCoffeeTemperatureMutationVariables
  >(SetCoffeeTemperatureDocument);
  const setSteamLevelMutation = createMutation<
    SetSteamLevelMutation,
    SetSteamLevelMutationVariables
  >(SetSteamLevelDocument);
  const setSteamEnabledMutation = createMutation<
    SetSteamEnabledMutation,
    SetSteamEnabledMutationVariables
  >(SetSteamEnabledDocument);
  const refetch = async () => {
    const s = await smartStandbyQuery.resourceActions.refetch();
    if (s)
      smartStandbyQuery.resourceActions.mutate((d) => ({
        ...d,
        smartStandby: s.smartStandby,
      }));
    const b = await boilersQuery.resourceActions.refetch();
    if (b)
      boilersQuery.resourceActions.mutate((d) => ({
        ...d,
        boilers: b.boilers,
      }));
  };
  return {
    connect: (deviceId: string) =>
      new Promise<void>((resolve, reject) =>
        connectMutation({ deviceId })
          .then((d) => (d.connectMachine.boolean ? resolve() : reject()))
          .catch(reject),
      ),
    disconnect: () =>
      new Promise<void>((resolve, reject) =>
        disconnectMutation({})
          .then((d) => (d.disconnectMachine.boolean ? resolve() : reject()))
          .catch(reject),
      ),
    state: () =>
      stateQuery.resource.latest?.machineState ?? MachineState.NotConnected,
    setStandby: (standby: boolean): Promise<void> =>
      new Promise<void>((resolve, reject) =>
        setStateMutation({ standby })
          .then((d) => (d.setStandby ? resolve() : reject()))
          .catch(reject),
      ),
    boilers: () => boilersQuery.resource.latest?.boilers,
    smartStandby: () => smartStandbyQuery.resource.latest?.smartStandby,
    setSmartStandby: (standby: SmartStandby) =>
      new Promise<void>((resolve, reject) =>
        setSmartStandbyMutation({ standby })
          .then((r) => {
            smartStandbyQuery.resourceActions.mutate((d) => ({
              ...d,
              smartStandby: r.setSmartStandby.smartStandby!,
            }));
            resolve();
          })
          .catch(reject),
      ),
    setCoffeeTemperature: (temperature: number) =>
      new Promise<void>((resolve, reject) =>
        setCoffeeTemperatureMutation({ temperature })
          .then((r) => {
            if (r.setCoffeeTemperature.boilers)
              boilersQuery.resourceActions.mutate((d) => ({
                ...d,
                boilers: r.setCoffeeTemperature.boilers!,
              }));
            resolve();
          })
          .catch(reject),
      ),
    setSteamLevel: (level: number) =>
      new Promise<void>((resolve, reject) =>
        setSteamLevelMutation({ level })
          .then((r) => {
            if (r.setSteamLevel.boilers)
              boilersQuery.resourceActions.mutate((d) => ({
                ...d,
                boilers: r.setSteamLevel.boilers!,
              }));
            resolve();
          })
          .catch(reject),
      ),
    setSteamEnabled: (enabled: boolean) =>
      new Promise<void>((resolve, reject) =>
        setSteamEnabledMutation({ enabled })
          .then((r) => {
            if (r.setSteamEnabled.boilers)
              boilersQuery.resourceActions.mutate((d) => ({
                ...d,
                boilers: r.setSteamEnabled.boilers!,
              }));
            resolve();
          })
          .catch(reject),
      ),
    refetch,
  };
};

export const scanMachinesAccessor = () => {
  const scanningQuery = createQuery<
    IsScanningPollQuery,
    IsScanningPollQueryVariables
  >(IsScanningPollDocument, () => ({}));
  scanningQuery.subscribeToMore<
    IsScanningSubscription,
    IsScanningSubscriptionVariables
  >(
    IsScanningDocument,
    () => ({}),
    (newData, _, setData) => setData('isScanning', newData.isScanning),
  );
  const resultsQuery = createQuery<
    ScanResultsPollQuery,
    ScanResultsPollQueryVariables
  >(ScanResultsPollDocument, () => ({}));
  resultsQuery.subscribeToMore<
    ScanResultsSubscription,
    ScanResultsSubscriptionVariables
  >(
    ScanResultsDocument,
    () => ({}),
    (newData, _, setData) => setData('scanResults', newData.scanResults),
  );
  const startMutation = createMutation<
    ScanDevicesMutation,
    ScanDevicesMutationVariables
  >(ScanDevicesDocument);
  const stopMutation = createMutation<
    StopScanningMutation,
    StopScanningMutationVariables
  >(StopScanningDocument);
  return {
    start: () => startMutation({}),
    stop: () => stopMutation({}),
    results: () => resultsQuery.resource.latest?.scanResults ?? [],
    scanning: () => scanningQuery.resource.latest?.isScanning ?? false,
  };
};
