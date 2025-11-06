import {
  Accessor,
  createContext,
  createEffect,
  createSignal,
  onCleanup,
  ParentComponent,
  Show,
  useContext,
} from 'solid-js';
import {
  MachineState,
  useMachineAccessor,
  type Boilers,
  type SmartStandby,
} from '@micra-pro/machine/data-access';
import { Spinner } from '@micra-pro/shared/ui';
import { T } from '../generated/language-types';

const PollPeriodMs = 1000;

const MachineContext = createContext<{
  isConnected: Accessor<boolean>;
  isStandby: Accessor<boolean>;
  setStandby: (standby: boolean) => Promise<void>;
  connect: (deviceId: string) => Promise<void>;
  disconnect: () => Promise<void>;
  boilers: Accessor<Boilers | undefined>;
  smartStandby: Accessor<SmartStandby | undefined>;
  setSmartStandby: (standby: SmartStandby) => Promise<void>;
  setCoffeeTemperature: (temperature: number) => Promise<void>;
  setSteamLevel: (level: number) => Promise<void>;
  setSteamEnabled: (enabled: boolean) => Promise<void>;
}>();

export const useMachineContextInternal = () => {
  const ctx = useContext(MachineContext);
  if (!ctx) throw new Error('Machine Context not found!');
  return ctx;
};

export const useMachineContext = () => {
  const ctx = useMachineContextInternal();
  return {
    coffeeTemperature: () => ctx.boilers()?.coffeeBoiler.temperature,
  };
};

export const MachineContextProvider: ParentComponent = (props) => {
  let timer: NodeJS.Timeout | undefined;
  const accessor = useMachineAccessor();
  const fetch = () => {
    if (accessor.state() !== MachineState.NotConnected)
      accessor
        .refetch()
        .finally(() => (timer = setTimeout(fetch, PollPeriodMs)));
    else timer = setTimeout(fetch, PollPeriodMs);
  };
  createEffect(() => {
    if (accessor.state() === MachineState.NotConnected && timer) {
      clearTimeout(timer);
      timer = undefined;
    }
    if (accessor.state() !== MachineState.NotConnected && !timer)
      accessor
        .refetch()
        .finally(() => (timer = setTimeout(fetch, PollPeriodMs)));
  });
  onCleanup(() => timer && clearTimeout(timer));
  return (
    <MachineContext.Provider
      value={{
        isConnected: () => accessor.state() !== MachineState.NotConnected,
        isStandby: () => accessor.state() === MachineState.Standby,
        setStandby: accessor.setStandby,
        connect: accessor.connect,
        disconnect: accessor.disconnect,
        boilers: accessor.boilers,
        smartStandby: accessor.smartStandby,
        setSmartStandby: accessor.setSmartStandby,
        setCoffeeTemperature: accessor.setCoffeeTemperature,
        setSteamLevel: accessor.setSteamLevel,
        setSteamEnabled: accessor.setSteamEnabled,
      }}
    >
      <MachineStandbyOverlay>{props.children}</MachineStandbyOverlay>
    </MachineContext.Provider>
  );
};

const MachineStandbyOverlay: ParentComponent = (props) => {
  const machineContext = useMachineContextInternal();
  const [loading, setLoading] = createSignal(false);
  createEffect(() => machineContext.isStandby() && setLoading(false));
  const wakeUp = () => {
    setLoading(true);
    machineContext.setStandby(false);
  };
  return (
    <>
      <Show when={machineContext.isStandby()}>
        <div class="flex h-full w-full items-center justify-center bg-black text-gray-400">
          <Show when={!loading()}>
            <div
              class="rounded-lg border border-gray-700 px-8 py-4 shadow-md shadow-gray-700 active:bg-gray-900"
              onClick={wakeUp}
            >
              <T key="wake-up" />
            </div>
          </Show>
          <Show when={loading()}>
            <Spinner class="h-16 fill-gray-400 text-gray-700" />
          </Show>
        </div>
      </Show>
      <Show when={!machineContext.isStandby()}>{props.children}</Show>
    </>
  );
};
