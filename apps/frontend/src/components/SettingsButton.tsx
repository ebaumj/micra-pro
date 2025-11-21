import { Component, createSignal, Show } from 'solid-js';

import {
  Button,
  handleError,
  Icon,
  Sheet,
  SheetContent,
  useDarkModeContext,
} from '@micra-pro/shared/ui';
import { createSystemAccessor } from '@micra-pro/shared/utils-ts';
import { T, useTranslationContext } from '../generated/language-types';
import { GrinderOffsetSelector } from '@micra-pro/bean-management/feature';
import { twMerge } from 'tailwind-merge';
import { BrewByWeightPannelStyleSelector } from '@micra-pro/brew-by-weight/feature';

export const SettingsButton: Component<{
  class?: string;
  onSettingChanged?: () => void;
}> = (props) => {
  const { t } = useTranslationContext();
  const [shutdown, setShutdown] = createSignal(false);
  const [reboot, setReboot] = createSignal(false);
  const [menuOpen, setMenuOpen] = createSignal(false);
  const system = createSystemAccessor();
  const darkMode = useDarkModeContext();
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
        <Icon iconName="settings" />
      </Button>
      <SheetContent
        onInteractOutside={(e) => e.preventDefault()}
        onFocusOutside={(e) => e.preventDefault()}
        class="flex w-80 flex-col p-4"
        closeButton={false}
      >
        <div class="itemx-start flex justify-end">
          <Button variant="ghost" onClick={() => setMenuOpen(false)}>
            <Icon iconName="close" />
          </Button>
        </div>
        <div class="flex w-full justify-center">
          <div class="flex h-10 w-56">
            <div
              class="z-10 flex w-1/2 items-center justify-center"
              onClick={() => darkMode.setDarkMode(false)}
            >
              <Icon iconName="light_mode" />
            </div>
            <div
              class="z-10 flex w-1/2 items-center justify-center"
              onClick={() => darkMode.setDarkMode(true)}
            >
              <Icon iconName="dark_mode" />
            </div>
          </div>
          <div class="fixed flex h-10 w-56 rounded-md border inset-shadow-sm">
            <div
              class={twMerge(
                'bg-secondary w-1/2 rounded inset-shadow-sm transition-transform duration-300',
                darkMode.darkMode() ? 'translate-x-full' : 'translate-x-0',
              )}
            />
          </div>
        </div>
        <BrewByWeightPannelStyleSelector />
        <div class="flex w-full items-center whitespace-nowrap">
          <T key="grinder-offset" />
          <div class="flex w-full justify-end">
            <GrinderOffsetSelector onChanged={props.onSettingChanged} />
          </div>
        </div>
        <Show when={!shutdown() && !reboot()}>
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
