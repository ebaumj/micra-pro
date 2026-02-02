import { Accessor, createEffect, createSignal } from 'solid-js';
import { Wifi } from '../generated/graphql';

export const createSystemAccessor = (): {
  reboot: () => Promise<void>;
  shutdown: () => Promise<void>;
} => {
  return {
    reboot: () => Promise.reject(),
    shutdown: () => Promise.reject(),
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
  const [wifi, setWifi] = createSignal<string | undefined>();
  const storage = localStorage['Wifi'];
  if (storage)
    try {
      setWifi(JSON.parse(storage).wifi);
    } catch {
      // Data does not match
    }
  createEffect(() => (localStorage['Wifi'] = JSON.stringify({ wifi: wifi() })));
  const available: Wifi[] = [
    { passwordRequired: false, ssid: 'NotSecuredWifi' },
    { passwordRequired: true, ssid: 'AnyPassword' },
  ];
  return {
    scan: () => undefined,
    current: () => wifi() ?? '',
    available: () => available,
    isScanning: () => false,
    connect: (ssid: string, _?: string): Promise<boolean> => {
      setWifi(ssid);
      return Promise.resolve(true);
    },
    disconnect: async (_: string): Promise<void> => {
      setWifi(undefined);
      return Promise.resolve();
    },
    fetchCurrent: async (): Promise<void> => Promise.resolve(),
  };
};

export const updateAccess = (): {
  currentVersion: Accessor<string>;
  loading: Accessor<boolean>;
  installUpdate: (link: string, signature: string) => Promise<void>;
} => {
  return {
    currentVersion: () => '0.0.0',
    loading: () => false,
    installUpdate: (_: string, _1: string) => Promise.reject(),
  };
};
