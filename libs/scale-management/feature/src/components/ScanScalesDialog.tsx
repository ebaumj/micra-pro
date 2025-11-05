import { Component, createSignal, For, onMount, Show } from 'solid-js';
import {
  BluetoothScale,
  scanForScales,
} from '@micra-pro/scale-management/data-access';
import {
  Icon,
  Spinner,
  SpinnerButton,
  TextFieldRoot,
  TextField,
} from '@micra-pro/shared/ui';
import moment from 'moment';
import { createStore } from 'solid-js/store';

export const ScanScalesDialog: Component<{
  isOpen: boolean;
  close: () => void;
  addDevice: (identifier: string, name: string) => void;
  isScanning: boolean;
}> = (props) => {
  let content!: HTMLDivElement;
  const [scales, setScales] = createStore({ scales: [] as BluetoothScale[] });
  const [adding, setAdding] = createSignal('');
  onMount(() => {
    content.focus();
    // eslint-disable-next-line solid/reactivity
    scanForScales(moment.duration(10, 'seconds'), (s) =>
      setScales('scales', scales.scales.length, s),
    );
  });

  return (
    <div class="flex h-64 flex-col" ref={content}>
      <div class="no-scrollbar flex h-full w-full flex-col gap-2 overflow-scroll px-6">
        <For each={scales.scales}>
          {(device, index) => (
            <div class="flex w-full gap-4 rounded-lg border p-2 shadow-xs">
              <div class="flex h-full w-full items-center">
                <TextFieldRoot
                  onChange={(name) =>
                    setScales('scales', index(), 'name', name)
                  }
                  class="w-full"
                >
                  <TextField value={device.name} />
                </TextFieldRoot>
              </div>
              <div class="flex h-full items-center">
                <SpinnerButton
                  class="h-10 w-10 p-0"
                  spinnerClass="p-2"
                  variant="outline"
                  onClick={() => {
                    setAdding(device.id);
                    props.addDevice(device.id, device.name);
                  }}
                  disabled={adding() !== ''}
                  loading={adding() === device.id}
                >
                  <Icon iconName="add" />
                </SpinnerButton>
              </div>
            </div>
          )}
        </For>
      </div>
      <div class="flex h-10 w-full justify-end p-2">
        <Show when={props.isScanning}>
          <Spinner class="h-full" />
        </Show>
      </div>
    </div>
  );
};
