import { Component, createSignal, For, JSX } from 'solid-js';
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
import { useNumberPickerStyle } from './NumberPickerStyleProvider';

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

const Selector: Component<{
  options: {
    element: JSX.Element;
    onSelect: () => void;
  }[];
  selected: number;
  class?: string;
}> = (props) => {
  return (
    <div class="flex w-full justify-center">
      <div class={twMerge('relative', props.class)}>
        <div class="absolute flex h-full w-full">
          <For each={props.options}>
            {(option) => (
              <div
                class="z-10 flex items-center justify-center"
                style={{ width: `${100 / props.options.length}%` }}
                onClick={option.onSelect}
              >
                {option.element}
              </div>
            )}
          </For>
        </div>
        <div class="absolute flex h-full w-full rounded-md border inset-shadow-sm">
          <div
            style={{
              width: `${100 / props.options.length}%`,
              transform: `translateX(${props.selected * 100}%)`,
            }}
            class="bg-secondary rounded inset-shadow-sm transition-transform duration-300"
          />
        </div>
      </div>
    </div>
  );
};

const IconBox: Component<{ name: string }> = (props) => (
  <div class="h-full p-2">
    <div class="flex aspect-square h-full items-center justify-center rounded-sm border shadow-sm">
      <Icon iconName={props.name} />
    </div>
  </div>
);

export const SettingsButton: Component<{
  class?: string;
}> = (props) => {
  const { t } = useTranslationContext();
  const [shutdown, setShutdown] = createSignal(false);
  const [reboot, setReboot] = createSignal(false);
  const [menuOpen, setMenuOpen] = createSignal(false);
  const system = createSystemAccessor();
  const darkMode = useDarkModeContext();
  const numberPicker = useNumberPickerStyle();
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
        <Selector
          selected={darkMode.darkMode() ? 1 : 0}
          options={[
            {
              onSelect: () => darkMode.setDarkMode(false),
              element: <Icon iconName={'light_mode'} />,
            },
            {
              onSelect: () => darkMode.setDarkMode(true),
              element: <Icon iconName={'dark_mode'} />,
            },
          ]}
          class="h-10 w-56"
        />
        <Selector
          selected={numberPicker.style() === 'NumberWheel' ? 1 : 0}
          options={[
            {
              onSelect: () => numberPicker.setStyle('NumberPad'),
              element: <IconBox name="grid_view" />,
            },
            {
              onSelect: () => numberPicker.setStyle('NumberWheel'),
              element: <IconBox name="view_column" />,
            },
          ]}
          class="h-14 w-56"
        />
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
