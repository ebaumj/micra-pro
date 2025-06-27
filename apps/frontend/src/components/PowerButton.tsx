import { Component, createSignal, Show } from 'solid-js';

import {
  Button,
  handleError,
  Icon,
  Sheet,
  SheetContent,
} from '@micra-pro/shared/ui';
import { createSystemAccessor } from '@micra-pro/shared/utils-ts';
import { T, useTranslationContext } from '../generated/language-types';

export const PowerButton: Component<{ class?: string }> = (props) => {
  const { t } = useTranslationContext();
  const [shutdown, setShutdown] = createSignal(false);
  const [reboot, setReboot] = createSignal(false);
  const [menuOpen, setMenuOpen] = createSignal(false);
  const system = createSystemAccessor();
  const shutdownCommand = () => {
    setShutdown(true);
    system.shutdown().catch((e) => {
      setShutdown(false);
      console.log(e);
      handleError({ title: t('shutdown-failed'), message: '' });
    });
  };
  const rebootCommand = () => {
    setReboot(true);
    system.reboot().catch(() => {
      setReboot(false);
      handleError({ title: t('reboot-failed'), message: '' });
    });
  };
  return (
    <Sheet open={menuOpen()}>
      <Button
        variant="outline"
        class={props.class}
        onClick={() => setMenuOpen(true)}
      >
        <Icon iconName="settings_power" />
      </Button>
      <SheetContent
        onInteractOutside={(e) => e.preventDefault()}
        onFocusOutside={(e) => e.preventDefault()}
        class="flex w-80 flex-col p-4"
        closeButton={false}
      >
        <Show when={!shutdown() && !reboot()}>
          <div class="itemx-start flex justify-end">
            <Button variant="ghost" onClick={() => setMenuOpen(false)}>
              <Icon iconName="close" />
            </Button>
          </div>
          <div class="flex h-full w-full flex-col items-end justify-end gap-4">
            <Button
              variant="default"
              class="flex w-full gap-2"
              onClick={shutdownCommand}
            >
              <Icon iconName="power_settings_new" />
              <T key="shutdown" />
            </Button>
            <Button
              variant="default"
              class="flex w-full gap-2"
              onClick={rebootCommand}
            >
              <Icon iconName="refresh" />
              <T key="reboot" />
            </Button>
          </div>
        </Show>
        <Show when={shutdown()}>
          <div class="flex h-full w-full items-center justify-center">
            <div class="flex h-16 w-16 animate-spin items-center justify-center text-4xl">
              <Icon iconName="power_settings_new" />
            </div>
          </div>
        </Show>
        <Show when={reboot() && !shutdown()}>
          <div class="flex h-full w-full items-center justify-center">
            <div class="flex h-16 w-16 animate-spin items-center justify-center text-4xl">
              <Icon iconName="refresh" />
            </div>
          </div>
        </Show>
      </SheetContent>
    </Sheet>
  );
};
