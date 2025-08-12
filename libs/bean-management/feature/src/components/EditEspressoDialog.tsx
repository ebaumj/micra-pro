import { EspressoProperties } from '@micra-pro/bean-management/data-access';
import {
  Button,
  Dialog,
  DialogContent,
  Icon,
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
import moment from 'moment';

const defaults = (): EspressoProperties =>
  JSON.parse(
    JSON.stringify({
      brewTemperature: 93,
      coffeeQuantity: 18,
      grindSetting: 18,
      inCupQuantity: 42,
      targetExtractionTime: moment.duration(26, 'seconds').toISOString(),
    }),
  );

export type EditEspressoDialogContent = {
  properties?: EspressoProperties;
  onSave: (properties: EspressoProperties) => void;
  onRemove?: () => void;
  isSaving?: Accessor<boolean>;
  isRemoving?: Accessor<boolean>;
  onFlowProfileSelect?: () => void;
};

export const EditEspressoDialog: Component<{
  content: EditEspressoDialogContent | undefined;
  onClose: () => void;
}> = (props) => {
  const [store, setStore] = createStore({ properties: defaults() });

  createEffect(() => {
    const newProperties = props.content?.properties;
    if (newProperties) setStore('properties', newProperties);
    else setStore('properties', defaults());
  });

  const targetExtractionTimeSecons = () =>
    moment.duration(store.properties.targetExtractionTime).asSeconds();
  const setTargetExtractionTimeSecons = (value: number) =>
    setStore(
      'properties',
      'targetExtractionTime',
      moment.duration(value, 'seconds').toISOString(),
    );

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
            <div class="bg-background w-1/2">
              <NumberField
                onFocusIn={(e) => e.preventDefault()}
                onRawValueChange={(v) =>
                  setStore('properties', 'coffeeQuantity', v)
                }
                rawValue={store.properties.coffeeQuantity}
                formatOptions={{ style: 'unit', unit: 'gram' }}
                minValue={1}
                maxValue={40}
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
              <T key="grind-setting" />:
            </div>
            <div class="bg-background w-1/2">
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
              <T key="in-cup-quantity" />:
            </div>
            <div class="bg-background w-1/2">
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
            <div class="bg-background w-1/2">
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
          <div class="flex w-full">
            <div class="flex h-full w-1/2 items-center text-sm font-semibold">
              <T key="target-extraction-time" />:
            </div>
            <div class="bg-background w-1/2">
              <NumberField
                onFocusIn={(e) => e.preventDefault()}
                onRawValueChange={setTargetExtractionTimeSecons}
                rawValue={targetExtractionTimeSecons()}
                formatOptions={{ style: 'unit', unit: 'second' }}
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
          <div class="flex w-full pt-4">
            <div class="flex justify-start">
              <Show when={props.content?.onFlowProfileSelect}>
                <Button
                  variant="outline"
                  class="flex h-10 w-10 items-center justify-center p-0"
                  onClick={props.content?.onFlowProfileSelect}
                >
                  <Icon iconName="area_chart" />
                </Button>
              </Show>
            </div>
            <div class="flex w-full justify-end gap-2">
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
                variant="default"
                onClick={() => props.content?.onSave(store.properties)}
                loading={props.content?.isSaving?.() ?? false}
              >
                <T key="save" />
              </SpinnerButton>
            </div>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};
