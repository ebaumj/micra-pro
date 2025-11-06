import { Component, Match, Switch } from 'solid-js';
import { MachineConnectPage } from './MachineConnectPage';
import { MachineAccessPage } from './MachineAccessPage';
import { useMachineContextInternal } from './MachineContextProvider';

export const MachinePage: Component = () => {
  const machineAccessor = useMachineContextInternal();
  return (
    <Switch>
      <Match when={!machineAccessor.isConnected()}>
        <MachineConnectPage connectMachine={machineAccessor.connect} />
      </Match>
      <Match when={machineAccessor.isConnected()}>
        <MachineAccessPage />
      </Match>
    </Switch>
  );
};
