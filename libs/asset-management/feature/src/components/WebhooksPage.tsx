import { getWebhooksAsync } from '@micra-pro/asset-management/data-access';
import { Select, Spinner } from '@micra-pro/shared/ui';
import { Component, createEffect, createSignal, Show } from 'solid-js';
import QRCode from 'qrcode';

export const WebhooksPage: Component = () => {
  const [webhooks, setWebhooks] = createSignal<
    { name: string; accessUrl: string }[]
  >([]);
  const [isLoading, setIsLoading] = createSignal(true);
  const [selectedHook, setSelectedHook] = createSignal('');
  const [qr, setQr] = createSignal<string | undefined>();
  getWebhooksAsync().then((hooks) => {
    setWebhooks(hooks);
    setSelectedHook(hooks[0]?.name ?? '');
    setIsLoading(false);
  });
  createEffect(() => {
    const hook = webhooks().find((h) => h.name === selectedHook());
    if (hook) QRCode.toDataURL(hook.accessUrl).then(setQr);
    else setQr(undefined);
  });
  return (
    <div class="no-scrollbar h-full w-full overflow-hidden">
      <Show when={isLoading()}>
        <div class="flex h-full w-full items-center justify-center">
          <Spinner class="h-20 w-20" />
        </div>
      </Show>
      <Show when={!isLoading()}>
        <Select
          options={webhooks().map((h) => h.name)}
          displayElement={(val) => val}
          onChange={(val) => {
            setSelectedHook(val);
          }}
          value={selectedHook()}
          contentClass="h-88 overflow-y-scroll no-scrollbar"
        />
        <Show when={qr()}>
          <div class="flex h-full w-full flex-col items-center justify-center">
            <img src={qr()} class="h-64 w-64" />
          </div>
        </Show>
      </Show>
    </div>
  );
};
