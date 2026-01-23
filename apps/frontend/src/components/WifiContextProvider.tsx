import { wifiAccess } from '@micra-pro/shared/utils-ts';
import { createContext, onMount, ParentComponent, useContext } from 'solid-js';

const WifiContext = createContext<{
  scan: () => void;
  current: () => string | null | undefined;
  available: () => {
    passwordRequired: boolean;
    ssid: string;
  }[];
  isScanning: () => boolean;
  connect: (ssid: string, password?: string) => Promise<boolean>;
  disconnect: (ssid: string) => Promise<void>;
}>();
const PollTimeMinutes = 1;

export const useWifiContext = () => {
  const ctx = useContext(WifiContext);
  if (!ctx) throw new Error("Can't find Wifi Context!");
  return ctx;
};

export const WifiContextProvider: ParentComponent<{}> = (props) => {
  const accessor = wifiAccess();
  const poll = () => {
    accessor.fetchCurrent().catch();
    setTimeout(poll, PollTimeMinutes * 60000);
  };
  onMount(poll);
  return (
    <WifiContext.Provider value={accessor}>
      {props.children}
    </WifiContext.Provider>
  );
};
