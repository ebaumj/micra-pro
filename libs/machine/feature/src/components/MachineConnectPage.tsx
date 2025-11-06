import { Component, createSignal, For, onMount } from 'solid-js';
import { scanMachinesAccessor } from '@micra-pro/machine/data-access';
import { handleError, Icon, SpinnerButton } from '@micra-pro/shared/ui';
import { T, useTranslationContext } from '../generated/language-types';

export const MachineConnectPage: Component<{
  connectMachine: (deviceId: string) => Promise<void>;
}> = (props) => {
  const { t } = useTranslationContext();
  const [connecting, setConnecting] = createSignal('');
  const scanAccessor = scanMachinesAccessor();
  const scanButtonPress = () =>
    scanAccessor.scanning() ? scanAccessor.stop() : scanAccessor.start();
  onMount(() => {
    if (!scanAccessor.scanning()) scanAccessor.start();
  });
  const connectPress = (deviceId: string, name: string) => {
    setConnecting(deviceId);
    props.connectMachine(deviceId).catch((_) => {
      handleError({
        title: t('error'),
        message: t('connection-failed', { machine: name }),
      });
      setConnecting('');
    });
  };
  return (
    <div class="flex h-full w-full flex-col gap-2">
      <div class="no-scrollbar flex h-full flex-col gap-2 overflow-y-scroll">
        <For each={scanAccessor.results()}>
          {(d) => (
            <div class="flex items-center rounded-md border px-2 py-1 whitespace-nowrap shadow-xs">
              {d.name}
              <div class="flex w-full justify-end">
                <SpinnerButton
                  onClick={() => connectPress(d.id, d.name)}
                  class="flex w-36 items-center justify-center p-0"
                  loading={connecting() === d.id}
                  disabled={connecting() !== ''}
                >
                  <T key="connect" />
                </SpinnerButton>
              </div>
            </div>
          )}
        </For>
      </div>
      <div class="flex h-12 items-center justify-end">
        <SpinnerButton
          onClick={scanButtonPress}
          class="flex h-10 w-10 items-center justify-center p-0"
          loading={scanAccessor.scanning()}
        >
          <Icon iconName="search" />
        </SpinnerButton>
      </div>
    </div>
  );
};
