import { Accessor } from 'solid-js';
import {
  Backup,
  BackupDataDocument,
  BackupDataMutation,
  BackupDataMutationVariables,
  BackupsDocument,
  BackupsQuery,
  BackupsQueryVariables,
  ConnectedWifiDocument,
  ConnectedWifiQuery,
  ConnectedWifiQueryVariables,
  ConnectWifiDocument,
  ConnectWifiMutation,
  ConnectWifiMutationVariables,
  DeleteBackupDocument,
  DeleteBackupMutation,
  DeleteBackupMutationVariables,
  DisconnectWifiDocument,
  DisconnectWifiMutation,
  DisconnectWifiMutationVariables,
  InstallUpdateDocument,
  InstallUpdateMutation,
  InstallUpdateMutationVariables,
  RebootDocument,
  RebootMutation,
  RebootMutationVariables,
  RestoreDataDocument,
  RestoreDataMutation,
  RestoreDataMutationVariables,
  ScanWifiDocument,
  ScanWifiQuery,
  ScanWifiQueryVariables,
  ShutdownDocument,
  ShutdownMutation,
  ShutdownMutationVariables,
  SystemVersionDocument,
  SystemVersionQuery,
  SystemVersionQueryVariables,
  UseBackupsDocument,
  UseBackupsQuery,
  UseBackupsQueryVariables,
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
  disconnect: (ssid: string) => Promise<void>;
  fetchCurrent: () => Promise<void>;
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
  const disconnectMutation = createMutation<
    DisconnectWifiMutation,
    DisconnectWifiMutationVariables
  >(DisconnectWifiDocument);
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
    disconnect: async (ssid: string): Promise<void> => {
      await disconnectMutation({ ssid });
      const current = await currentNetworkQuery.resourceActions.refetch();
      if (current) currentNetworkQuery.resourceActions.mutate(current);
    },
    fetchCurrent: async (): Promise<void> => {
      const current = await currentNetworkQuery.resourceActions.refetch();
      if (current) currentNetworkQuery.resourceActions.mutate(current);
    },
  };
};

export const updateAccess = (): {
  currentVersion: Accessor<string>;
  allowUpdates: Accessor<boolean>;
  loading: Accessor<boolean>;
  installUpdate: (link: string, signature: string) => Promise<void>;
} => {
  const query = createQuery<SystemVersionQuery, SystemVersionQueryVariables>(
    SystemVersionDocument,
    () => ({}),
  );
  const mutation = createMutation<
    InstallUpdateMutation,
    InstallUpdateMutationVariables
  >(InstallUpdateDocument);
  return {
    currentVersion: () => query.resource.latest?.systemVersion.version ?? '',
    allowUpdates: () =>
      query.resource.latest?.systemVersion.allowUpdates ?? false,
    loading: () => query.resource.state === 'pending',
    installUpdate: async (link: string, signature: string): Promise<void> => {
      const result = await mutation({ link: link, signature: signature });
      if (result.installUpdate.boolean !== true) throw new Error();
    },
  };
};

export const backupAccess = (): {
  useBackups: Accessor<boolean>;
  available: Accessor<Backup[]>;
  backupData: () => Promise<void>;
  restoreData: (directory: string) => Promise<void>;
  deleteBackup: (directory: string) => Promise<void>;
} => {
  const enabledQuery = createQuery<UseBackupsQuery, UseBackupsQueryVariables>(
    UseBackupsDocument,
    () => ({}),
  );
  const availableQuery = createQuery<BackupsQuery, BackupsQueryVariables>(
    BackupsDocument,
    () => ({}),
  );
  const backupMutation = createMutation<
    BackupDataMutation,
    BackupDataMutationVariables
  >(BackupDataDocument);
  const restoreMutation = createMutation<
    RestoreDataMutation,
    RestoreDataMutationVariables
  >(RestoreDataDocument);
  const deleteMutation = createMutation<
    DeleteBackupMutation,
    DeleteBackupMutationVariables
  >(DeleteBackupDocument);
  return {
    useBackups: () => {
      if (!enabledQuery.resource.latest?.useBackups) {
        enabledQuery.resourceActions.refetch();
        return false;
      }
      return enabledQuery.resource.latest.useBackups;
    },
    available: () => {
      if (!availableQuery.resource.latest?.backups) {
        availableQuery.resourceActions.refetch();
        return [];
      }
      return availableQuery.resource.latest.backups;
    },
    backupData: () =>
      new Promise<void>((resolve, reject) =>
        backupMutation({})
          .then((r) => {
            if (r.backupData.boolean) {
              availableQuery.resourceActions.refetch();
              resolve();
            } else reject();
          })
          .catch(reject),
      ),
    restoreData: (directory: string) =>
      new Promise<void>((resolve, reject) =>
        restoreMutation({ directory })
          .then((r) => (r.restoreData.boolean ? resolve() : reject()))
          .catch(reject),
      ),
    deleteBackup: (directory: string) =>
      new Promise<void>((resolve, reject) =>
        deleteMutation({ directory })
          .then((r) => {
            if (r.deleteBackup.boolean) {
              availableQuery.resourceActions.refetch();
              resolve();
            } else reject();
          })
          .catch(reject),
      ),
  };
};
