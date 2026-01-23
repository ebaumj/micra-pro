import { Component, createSignal } from 'solid-js';
import {
  Button,
  handleError,
  Icon,
  Sheet,
  SheetContent,
  useDarkModeContext,
} from '@micra-pro/shared/ui';
import { createSystemAccessor } from '@micra-pro/shared/utils-ts';
import { useTranslationContext } from '../generated/language-types';
import { twMerge } from 'tailwind-merge';
import { BrewByWeightPannelStyleSelector } from '@micra-pro/brew-by-weight/feature';
import { WifiButton } from './WifiButton';
import { ScaleSelector } from '@micra-pro/scale-management/feature';
import { UpdateButton } from './UpdateButton';

const ActionButton: Component<{
  class?: string;
  disabled?: boolean;
  loading?: boolean;
  iconName: string;
  onClick?: () => void;
}> = (props) => {
  return (
    <Button
      class={twMerge('flex h-full items-center', props.class)}
      disabled={props.disabled}
      onClick={props.onClick}
      variant="outline"
    >
      <Icon
        iconName={props.iconName}
        class={twMerge(
          'flex items-center text-2xl',
          props.loading ? 'animate-spin' : '',
        )}
      />
    </Button>
  );
};

export const SettingsButton: Component<{
  class?: string;
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
        <div class="flex h-full w-full flex-col items-end justify-end gap-2">
          <div class="flex h-14 w-full gap-2">
            <WifiButton class="w-1/3" />
            <ScaleSelector class="h-full w-1/3" />
            <UpdateButton class="w-1/3" />
          </div>
          <div class="flex h-10 w-full gap-2">
            <ActionButton
              iconName="restore_page"
              class="w-1/3"
              onClick={() => location.reload()}
              disabled={shutdown() || reboot()}
            />
            <ActionButton
              iconName="replay"
              class="w-1/3"
              disabled={shutdown() || reboot()}
              loading={reboot()}
              onClick={rebootCommand}
            />
            <ActionButton
              iconName="power_settings_new"
              class="w-1/3"
              disabled={shutdown() || reboot()}
              loading={shutdown()}
              onClick={shutdownCommand}
            />
          </div>
        </div>
      </SheetContent>
    </Sheet>
  );
};
