import {
  Component,
  createEffect,
  createSignal,
  For,
  onMount,
  ValidComponent,
} from 'solid-js';
import {
  scanForScales,
  ScanProcess,
} from '@micra-pro/scale-management/data-access';
import { handleError } from '../utils/handleError';
import { useTranslationContext } from '../generated/language-types';
import { Dynamic } from 'solid-js/web';
import {
  Icon,
  Spinner,
  SpinnerButton,
  TextFieldRoot,
  TextField,
} from '@micra-pro/shared/ui';

export const ScanScalesDialog: Component<{
  isOpen: boolean;
  close: () => void;
  addDevice: (identifier: string, name: string) => void;
}> = (props) => {
  let content!: HTMLDivElement;

  const [adding, setAdding] = createSignal('');

  onMount(() => content.focus());

  const { t } = useTranslationContext();
  const [scanProcess, setScanProcess] = createSignal<ScanProcess>({
    state: 'finished',
    devices: [],
  });
  const startScanning = () => {
    const currentScanner = scanForScales();
    createEffect(() => setScanProcess(currentScanner()));
  };
  createEffect(() => {
    if (props.isOpen) startScanning();
  });
  createEffect(() => {
    const process = scanProcess();
    if (process.state === 'error') {
      console.error(process.debugInformation);
      handleError({ message: t('failed-to-scan') });
      props.close();
    }
  });

  const selectContent = (process: ScanProcess): ValidComponent => {
    switch (process.state) {
      case 'scanning':
        return () => (
          <div class="flex h-full w-full items-center justify-center">
            <Spinner class="h-24 w-24" />
          </div>
        );
      case 'finished':
        return () => {
          const devices = process.devices.map((d) => {
            const [deviceName, setDeviceNameName] = createSignal(d);
            return {
              name: deviceName,
              setName: setDeviceNameName,
              loading: () => adding() === d,
              save: () => {
                setAdding(d);
                props.addDevice(d, deviceName());
              },
            };
          });
          return (
            <div class="no-scrollbar flex h-full w-full flex-col gap-2 overflow-scroll px-6">
              <For each={devices}>
                {(device) => (
                  <div class="flex w-full gap-4 rounded-lg border bg-slate-50 p-2 shadow-sm">
                    <div class="flex h-full w-full items-center">
                      <TextFieldRoot
                        onChange={(name) => device.setName(name)}
                        class="w-full bg-white"
                      >
                        <TextField value={device.name()} />
                      </TextFieldRoot>
                    </div>
                    <div class="flex h-full items-center">
                      <SpinnerButton
                        class="h-10 w-10 bg-white p-0"
                        spinnerClass="p-2"
                        variant="outline"
                        onClick={device.save}
                        loading={device.loading()}
                      >
                        <Icon iconName="add" />
                      </SpinnerButton>
                    </div>
                  </div>
                )}
              </For>
            </div>
          );
        };
      case 'error':
        return () => <></>;
    }
  };

  return (
    <div class="h-64" ref={content}>
      <Dynamic component={selectContent(scanProcess())} />
    </div>
  );
};
