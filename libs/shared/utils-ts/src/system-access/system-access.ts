import {
  ConnectedWifiDocument,
  ConnectedWifiQuery,
  ConnectedWifiQueryVariables,
  ConnectWifiDocument,
  ConnectWifiMutation,
  ConnectWifiMutationVariables,
  RebootDocument,
  RebootMutation,
  RebootMutationVariables,
  ScanWifiDocument,
  ScanWifiQuery,
  ScanWifiQueryVariables,
  ShutdownDocument,
  ShutdownMutation,
  ShutdownMutationVariables,
  Wifi,
} from '../generated/graphql';
import { createMutation } from '../graphql-client/createMutation';
import { createQuery } from '../graphql-client/createQuery';

export const createSystemAccessor = (): {
  reboot: () => Promise<void>;
  shutdown: () => Promise<void>;
} => {
  const rebootMutation = createMutation<
    RebootMutation,
    RebootMutationVariables
  >(RebootDocument);
  const shutdownMutation = createMutation<
    ShutdownMutation,
    ShutdownMutationVariables
  >(ShutdownDocument);
  async function systemReboot(): Promise<void> {
    const result = await rebootMutation({});
    if (result.reboot.boolean === true) return Promise.resolve();
    return Promise.reject();
  }
  async function systemShutdown(): Promise<void> {
    const result = await shutdownMutation({});
    if (result.shutdown.boolean === true) return Promise.resolve();
    return Promise.reject();
  }
  return {
    reboot: () => systemReboot(),
    shutdown: () => systemShutdown(),
  };
};

export const wifiAccess = (): {
  scan: () => void;
  current: () => string | null | undefined;
  available: () => Wifi[];
  isScanning: () => boolean;
  connect: (ssid: string, password?: string) => Promise<boolean>;
} => {
  const currentNetworkQuery = createQuery<
    ConnectedWifiQuery,
    ConnectedWifiQueryVariables
  >(ConnectedWifiDocument, () => ({}));
  const scanQuery = createQuery<ScanWifiQuery, ScanWifiQueryVariables>(
    ScanWifiDocument,
    () => ({}),
  );
  const connectMutation = createMutation<
    ConnectWifiMutation,
    ConnectWifiMutationVariables
  >(ConnectWifiDocument);
  return {
    current: () => currentNetworkQuery.resource.latest?.connectedWifi,
    available: () => scanQuery.resource.latest?.scanWifi ?? [],
    isScanning: () =>
      scanQuery.resource.state === 'pending' ||
      scanQuery.resource.state === 'refreshing',
    scan: scanQuery.resourceActions.refetch,
    connect: async (ssid: string, password?: string): Promise<boolean> => {
      const result = await connectMutation({ ssid: ssid, password: password });
      const current = await currentNetworkQuery.resourceActions.refetch();
      if (current) currentNetworkQuery.resourceActions.mutate(current);
      return result.connectWifi.boolean ?? false;
    },
  };
};
