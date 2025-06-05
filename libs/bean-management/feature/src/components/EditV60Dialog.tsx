import { V60Properties } from '@micra-pro/bean-management/data-access';
import {
  Dialog,
  DialogContent,
  NumberField,
  NumberFieldDecrementTrigger,
  NumberFieldGroup,
  NumberFieldIncrementTrigger,
  NumberFieldInput,
  SpinnerButton,
} from '@micra-pro/shared/ui';
import { Accessor, Component, createEffect, Show } from 'solid-js';
import { createStore } from 'solid-js/store';
import { T } from '../generated/language-types';

const defaults = (): V60Properties =>
  JSON.parse(
    JSON.stringify({
      brewTemperature: 92,
      coffeeQuantity: 25,
      grindSetting: 75,
      inCupQuantity: 350,
    }),
  );

export type EditV60DialogContent = {
  properties?: V60Properties;
  onSave: (properties: V60Properties) => void;
  onRemove?: () => void;
  isSaving?: Accessor<boolean>;
  isRemoving?: Accessor<boolean>;
};

export const EditV60Dialog: Component<{
  content: EditV60DialogContent | undefined;
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
            <div class="flex h-full w-1/2 items-center text-sm font-semibold">
              <T key="coffee-quantity" />:
            </div>
            <div class="w-1/2 bg-white">
              <NumberField
                onFocusIn={(e) => e.preventDefault()}
                onRawValueChange={(v) =>
                  setStore('properties', 'coffeeQuantity', v)
                }
                rawValue={store.properties.coffeeQuantity}
                formatOptions={{ style: 'unit', unit: 'gram' }}
                minValue={1}
                maxValue={40}
                step={0.1}
              >
                <NumberFieldGroup>
                  <NumberFieldDecrementTrigger aria-label="Decrement" />
                  <NumberFieldInput />
                  <NumberFieldIncrementTrigger aria-label="Increment" />
                </NumberFieldGroup>
              </NumberField>
            </div>
          </div>
          <div class="flex w-full">
            <div class="flex h-full w-1/2 items-center text-sm font-semibold">
              <T key="grind-setting" />:
            </div>
            <div class="w-1/2 bg-white">
              <NumberField
                onFocusIn={(e) => e.preventDefault()}
                onRawValueChange={(v) =>
                  setStore('properties', 'grindSetting', v)
                }
                rawValue={store.properties.grindSetting}
                formatOptions={{ style: 'decimal' }}
                minValue={0}
                maxValue={100}
                step={0.5}
              >
                <NumberFieldGroup>
                  <NumberFieldDecrementTrigger aria-label="Decrement" />
                  <NumberFieldInput />
                  <NumberFieldIncrementTrigger aria-label="Increment" />
                </NumberFieldGroup>
              </NumberField>
            </div>
          </div>
          <div class="flex w-full">
            <div class="flex h-full w-1/2 items-center text-sm font-semibold">
              <T key="water-quantity" />:
            </div>
            <div class="w-1/2 bg-white">
              <NumberField
                onFocusIn={(e) => e.preventDefault()}
                onRawValueChange={(v) =>
                  setStore('properties', 'inCupQuantity', v)
                }
                rawValue={store.properties.inCupQuantity}
                formatOptions={{ style: 'unit', unit: 'milliliter' }}
                minValue={10}
                maxValue={60}
                step={1}
              >
                <NumberFieldGroup>
                  <NumberFieldDecrementTrigger aria-label="Decrement" />
                  <NumberFieldInput />
                  <NumberFieldIncrementTrigger aria-label="Increment" />
                </NumberFieldGroup>
              </NumberField>
            </div>
          </div>
          <div class="flex w-full">
            <div class="flex h-full w-1/2 items-center text-sm font-semibold">
              <T key="brew-temperature" />:
            </div>
            <div class="w-1/2 bg-white">
              <NumberField
                onFocusIn={(e) => e.preventDefault()}
                onRawValueChange={(v) =>
                  setStore('properties', 'brewTemperature', v)
                }
                rawValue={store.properties.brewTemperature}
                formatOptions={{ style: 'unit', unit: 'celsius' }}
                minValue={80}
                maxValue={100}
                step={1}
              >
                <NumberFieldGroup>
                  <NumberFieldDecrementTrigger aria-label="Decrement" />
                  <NumberFieldInput />
                  <NumberFieldIncrementTrigger aria-label="Increment" />
                </NumberFieldGroup>
              </NumberField>
            </div>
          </div>
          <div class="flex w-full justify-end gap-2 pt-4">
            <Show when={props.content?.onRemove}>
              <SpinnerButton
                class="h-10 w-28"
                variant="destructive"
                onClick={() => props.content?.onRemove?.()}
                loading={props.content?.isRemoving?.() ?? false}
              >
                <T key="remove" />
              </SpinnerButton>
            </Show>
            <SpinnerButton
              class="h-10 w-28"
              spinnerClass="p-2 h-8 w-8"
              variant="default"
              onClick={() => props.content?.onSave(store.properties)}
              loading={props.content?.isSaving?.() ?? false}
            >
              <T key="save" />
            </SpinnerButton>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};
