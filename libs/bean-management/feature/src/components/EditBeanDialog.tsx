import { BeanProperties } from '@micra-pro/bean-management/data-access';
import {
  Dialog,
  DialogContent,
  SpinnerButton,
  TextField,
  TextFieldRoot,
} from '@micra-pro/shared/ui';
import { AssetSelector } from '@micra-pro/asset-management/feature';
import { Accessor, Component, createEffect, Show } from 'solid-js';
import { createStore } from 'solid-js/store';
import { T } from '../generated/language-types';

const defaults = (): BeanProperties =>
  JSON.parse(
    JSON.stringify({
      countryCode: '',
      name: '',
    }),
  );

export type EditBeanDialogContent = {
  properties?: BeanProperties;
  onSave: (properties: BeanProperties) => void;
  onRemove?: () => void;
  isSaving?: Accessor<boolean>;
  isRemoving?: Accessor<boolean>;
};

export const EditBeanDialog: Component<{
  content: EditBeanDialogContent | undefined;
  onClose: () => void;
}> = (props) => {
  const [store, setStore] = createStore({ properties: defaults() });

  createEffect(() => {
    const newProperties = props.content?.properties;
    if (newProperties) setStore('properties', newProperties);
    else setStore('properties', defaults());
  });

  return (
    <Dialog
      open={!!props.content}
      onOpenChange={(o) => (o ? undefined : props.onClose())}
    >
      <DialogContent
        onOpenAutoFocus={(e) => e.preventDefault()}
        onInteractOutside={(e) => e.preventDefault()}
      >
        <div class="flex flex-col gap-2 px-6">
          <div class="flex w-full justify-center pb-2">
            <AssetSelector
              class="flex h-36 w-36 items-center rounded-md border object-contain p-2 shadow-sm"
              onIdChange={(id) => setStore('properties', 'assetId', id)}
              assetId={store.properties.assetId ?? undefined}
              onRemove={() => setStore('properties', 'assetId', null)}
            />
          </div>
          <div class="flex w-full">
            <div class="flex h-full w-1/4 items-center text-sm font-semibold">
              <T key="name" />:
            </div>
            <TextFieldRoot
              onChange={(name) => setStore('properties', 'name', name)}
              class="w-full bg-white"
            >
              <TextField value={store.properties.name} />
            </TextFieldRoot>
          </div>
          <div class="flex w-full">
            <div class="flex h-full w-1/4 items-center text-sm font-semibold">
              <T key="country-code" />:
            </div>
            <TextFieldRoot
              onChange={(countryCode) =>
                setStore('properties', 'countryCode', countryCode)
              }
              class="w-full bg-white"
            >
              <TextField value={store.properties.countryCode} />
            </TextFieldRoot>
          </div>
          <div class="flex w-full justify-end gap-2 pt-4">
            <Show when={props.content?.onRemove}>
              <SpinnerButton
                class="h-10 w-36"
                variant="destructive"
                onClick={() => props.content?.onRemove?.()}
                loading={props.content?.isRemoving?.()}
              >
                <T key="remove" />
              </SpinnerButton>
            </Show>
            <SpinnerButton
              class="h-10 w-36"
              spinnerClass="p-2 h-8 w-8"
              variant="default"
              onClick={() => props.content?.onSave(store.properties)}
              loading={props.content?.isSaving?.()}
            >
              <T key="save" />
            </SpinnerButton>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};
