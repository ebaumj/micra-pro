import { RoasteryProperties } from '@micra-pro/bean-management/data-access';
import {
  Dialog,
  DialogContent,
  SpinnerButton,
  TextField,
  TextFieldRoot,
} from '@micra-pro/shared/ui';
import { Accessor, Component, createEffect, Show } from 'solid-js';
import { createStore } from 'solid-js/store';
import { T } from '../generated/language-types';

const defaults = (): RoasteryProperties =>
  JSON.parse(
    JSON.stringify({
      location: '',
      name: '',
    }),
  );

export type EditRoasteryDialogContent = {
  properties?: RoasteryProperties;
  onSave: (properties: RoasteryProperties) => void;
  onRemove?: () => void;
  isSaving?: Accessor<boolean>;
  isRemoving?: Accessor<boolean>;
};

export const EditRoasteryDialog: Component<{
  content: EditRoasteryDialogContent | undefined;
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
          <div class="flex w-full">
            <div class="flex h-full w-1/4 items-center text-sm font-semibold">
              <T key="name" />:
            </div>
            <TextFieldRoot
              onChange={(name) => setStore('properties', 'name', name)}
              class="bg-background w-full"
            >
              <TextField value={store.properties.name} />
            </TextFieldRoot>
          </div>
          <div class="flex w-full">
            <div class="flex h-full w-1/4 items-center text-sm font-semibold">
              <T key="location" />:
            </div>
            <TextFieldRoot
              onChange={(location) =>
                setStore('properties', 'location', location)
              }
              class="bg-background w-full"
            >
              <TextField value={store.properties.location} />
            </TextFieldRoot>
          </div>
          <div class="flex w-full justify-end gap-2 pt-4">
            <Show when={props.content?.onRemove}>
              <SpinnerButton
                class="h-10 w-28"
                variant="destructive"
                onClick={() => props.content?.onRemove?.()}
                loading={props.content?.isRemoving?.()}
              >
                <T key="remove" />
              </SpinnerButton>
            </Show>
            <SpinnerButton
              class="h-10 w-28"
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
