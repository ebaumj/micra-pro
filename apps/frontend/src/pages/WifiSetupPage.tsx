import {
  handleError,
  Icon,
  Spinner,
  SpinnerButton,
  TextField,
  TextFieldRoot,
} from '@micra-pro/shared/ui';
import { wifiAccess } from '@micra-pro/shared/utils-ts';
import { Component, createSignal, For, Show } from 'solid-js';
import { T, useTranslationContext } from '../generated/language-types';

const Network: Component<{
  ssid: string;
  passwordRequired: boolean;
  connect: (password?: string) => Promise<any>;
}> = (props) => {
  const [connecting, setConnecting] = createSignal(false);
  const [password, setPassword] = createSignal('');
  const { t } = useTranslationContext();
  const result = (success?: boolean) => {
    if (!success)
      handleError({
        title: t('wifi-failed-title'),
        message: t('wifi-failed', { network: props.ssid }),
      });
  };
  const connect = () => {
    setConnecting(true);
    props
      .connect(props.passwordRequired ? password() : undefined)
      .then(result)
      .catch(result)
      .finally(() => {
        setConnecting(false);
        setPassword('');
      });
  };
  return (
    <div class="flex h-12 w-full gap-4 rounded-md border pl-2">
      <div class="flex items-center justify-center">
        <Icon iconName={props.passwordRequired ? 'wifi' : 'wifi_password'} />
      </div>
      <div class="flex w-1/2 items-center">{props.ssid}</div>
      <div class="flex w-1/2 items-center">
        <Show when={props.passwordRequired}>
          <TextFieldRoot onChange={setPassword} value={password()}>
            <TextField placeholder={t('wifi-password')} type="password" />
          </TextFieldRoot>
        </Show>
      </div>
      <div class="flex w-40 items-center">
        <SpinnerButton
          spinnerClass="h-full"
          class="flex w-full items-center justify-center"
          onClick={connect}
          loading={connecting()}
          disabled={
            connecting() || (props.passwordRequired && password() === '')
          }
        >
          <T key="wifi-connect" />
        </SpinnerButton>
      </div>
    </div>
  );
};

export const WifiSetupPage: Component<{}> = () => {
  const wifiAccessor = wifiAccess();
  const [disconnecting, setDisconnecting] = createSignal(false);
  const disconnect = (ssid: string) => {
    setDisconnecting(true);
    wifiAccessor.disconnect(ssid).finally(() => setDisconnecting(false));
  };
  return (
    <div class="flex h-full flex-col">
      <div class="flex min-h-14 w-full items-center gap-4 rounded-md bg-slate-100 px-2 shadow-md">
        <Show when={wifiAccessor.current() !== undefined}>
          <>
            <div class="flex gap-4 whitespace-nowrap">
              <Icon
                iconName={wifiAccessor.current() ? 'wifi' : 'wifi_off'}
                class={wifiAccessor.current() ? 'opacity-100' : 'opacity-50'}
              />
              {wifiAccessor.current()}
            </div>
            <div class="flex w-full justify-end">
              <Show when={wifiAccessor.current()}>
                {(ssid) => (
                  <SpinnerButton
                    spinnerClass="h-full"
                    variant="outline"
                    class="flex items-center justify-center px-8"
                    onClick={() => disconnect(ssid())}
                    loading={disconnecting()}
                    disabled={disconnecting()}
                  >
                    <Icon iconName="wifi_off" />
                  </SpinnerButton>
                )}
              </Show>
            </div>
          </>
        </Show>
        <Show when={wifiAccessor.current() === undefined}>
          <Spinner class="h-6 px-1" />
        </Show>
      </div>
      <div class="no-scrollbar flex h-full flex-col gap-2 overflow-scroll py-2">
        <For
          each={wifiAccessor
            .available()
            .filter((a) => a.ssid !== wifiAccessor.current())}
        >
          {(network) => (
            <Network
              ssid={network.ssid}
              passwordRequired={network.passwordRequired}
              connect={(password?: string) =>
                wifiAccessor.connect(network.ssid, password)
              }
            />
          )}
        </For>
      </div>
      <div class="flex justify-end pt-4">
        <SpinnerButton
          class="flex h-14 w-14 items-center justify-center shadow-md"
          variant="outline"
          onClick={wifiAccessor.scan}
          loading={wifiAccessor.isScanning()}
          disabled={wifiAccessor.isScanning()}
        >
          <Icon iconName="search" />
        </SpinnerButton>
      </div>
    </div>
  );
};
