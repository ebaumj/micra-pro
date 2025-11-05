import {
  Bean,
  EspressoProperties,
  V60Properties,
} from '@micra-pro/bean-management/data-access';
import {
  Button,
  CountryFlag,
  Dialog,
  DialogContent,
  Icon,
  selectPicturesForMode,
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
} from '@micra-pro/shared/ui';
import { Component, createEffect, createSignal, Show } from 'solid-js';
import { Dynamic } from 'solid-js/web';
import { T } from '../generated/language-types';
import { JSX } from 'solid-js/jsx-runtime';
import moment from 'moment';
import picturesImport from '../generated/pictures-import';

const NumberField: Component<{
  value: number;
  title: JSX.Element;
  unit: string;
  class?: string;
}> = (props) => (
  <div class={props.class}>
    <div class="flex w-full justify-center text-base">{props.title}</div>
    <div class="flex h-full w-full justify-center text-2xl font-bold">
      {props.value} {props.unit}
    </div>
  </div>
);

const EspressoParams: Component<{
  recipe: EspressoProperties;
  class?: string;
}> = (props) => (
  <div class={props.class}>
    <div class="w-full">
      <div class="grid grid-cols-2 justify-center gap-4">
        <NumberField
          class="rounded-full border p-2 shadow-lg"
          title={<T key="grind-setting" />}
          unit=""
          value={props.recipe.grindSetting}
        />
        <NumberField
          class="rounded-full border p-2 shadow-lg"
          title={<T key="coffee-quantity" />}
          unit="g"
          value={props.recipe.coffeeQuantity}
        />
        <NumberField
          class="rounded-full border p-2 shadow-lg"
          title={<T key="in-cup-quantity" />}
          unit="ml"
          value={props.recipe.inCupQuantity}
        />
        <NumberField
          class="rounded-full border p-2 shadow-lg"
          title={<T key="brew-temperature" />}
          unit="°C"
          value={props.recipe.brewTemperature}
        />
        <div class="col-span-2 flex justify-center">
          <NumberField
            class="w-1/2 rounded-full border p-2 shadow-lg"
            title={<T key="target-extraction-time" />}
            unit="s"
            value={moment.duration(props.recipe.targetExtractionTime).seconds()}
          />
        </div>
      </div>
    </div>
  </div>
);

const V60Params: Component<{ recipe: V60Properties; class?: string }> = (
  props,
) => (
  <div class={props.class}>
    <div class="w-full">
      <div class="grid grid-cols-2 justify-center gap-4">
        <NumberField
          class="rounded-full border p-2 shadow-lg"
          title={<T key="grind-setting" />}
          unit=""
          value={props.recipe.grindSetting}
        />
        <NumberField
          class="rounded-full border p-2 shadow-lg"
          title={<T key="coffee-quantity" />}
          unit="g"
          value={props.recipe.coffeeQuantity}
        />
        <NumberField
          class="rounded-full border p-2 shadow-lg"
          title={<T key="in-cup-quantity" />}
          unit="ml"
          value={props.recipe.inCupQuantity}
        />
        <NumberField
          class="rounded-full border p-2 shadow-lg"
          title={<T key="brew-temperature" />}
          unit="°C"
          value={props.recipe.brewTemperature}
        />
      </div>
    </div>
  </div>
);

export const RecipePannel: Component<{
  beans: Bean[];
  beanId: string | null;
  startEspressoBrewing: (beanId: string, recipe: EspressoProperties) => void;
  onClose: () => void;
  scale?: string;
}> = (props) => {
  const pictures = selectPicturesForMode(picturesImport);
  const [recipe, setRecipe] = createSignal<
    | ({ type: 'Espresso' } & EspressoProperties)
    | ({ type: 'V60' } & V60Properties)
    | null
  >(null);
  createEffect(() => {
    if (!props.beanId) setRecipe(null);
  });
  const selectedBean = () =>
    props.beans.find((b) => b.id === props.beanId) ?? null;

  const espresso = () =>
    selectedBean()?.recipes.find((r) => r.__typename == 'EspressoProperties')
      ?.properties;
  const v60 = () =>
    selectedBean()?.recipes.find((r) => r.__typename == 'V60Properties')
      ?.properties;

  createEffect(() => {
    const espressoProps = espresso();
    const v60Props = v60();
    if (espressoProps && !v60Props)
      setRecipe({ type: 'Espresso', ...espressoProps });
    if (v60Props && !espressoProps) setRecipe({ type: 'V60', ...v60Props });
    if (!v60Props && !espressoProps) props.onClose();
  });

  const selectParams = (): Component<{ class?: string }> => {
    const rec = recipe();
    switch (rec?.type) {
      case 'Espresso':
        return (props) => <EspressoParams class={props.class} recipe={rec} />;
      case 'V60':
        return (props) => <V60Params class={props.class} recipe={rec} />;
      case undefined:
        return (props) => <div class={props.class} />;
    }
  };
  const start = () => {
    const rec = recipe();
    if (props.beanId && rec?.type === 'Espresso')
      props.startEspressoBrewing(props.beanId, rec);
  };
  return (
    <>
      <Sheet
        open={!!props.beanId && !recipe()}
        onOpenChange={(o) => (o ? undefined : props.onClose())}
      >
        <SheetContent onOpenAutoFocus={(e) => e.preventDefault()} class="w-80">
          <SheetHeader>
            <SheetTitle>
              <T key="select-brew-mehtod" />
            </SheetTitle>
            <SheetDescription class="flex flex-col gap-4 pt-4">
              <Show when={espresso()}>
                {(props) => (
                  <Button
                    variant="outline"
                    class="flex h-14 w-full justify-center py-2"
                    onClick={() => setRecipe({ type: 'Espresso', ...props() })}
                  >
                    <img
                      src={pictures().espresso}
                      class={'h-full object-contain'}
                    />
                  </Button>
                )}
              </Show>
              <Show when={v60()}>
                {(props) => (
                  <Button
                    variant="outline"
                    class="flex h-14 w-full justify-center py-2"
                    onClick={() => setRecipe({ type: 'V60', ...props() })}
                  >
                    <img src={pictures().v60} class={'h-full object-contain'} />
                  </Button>
                )}
              </Show>
            </SheetDescription>
          </SheetHeader>
        </SheetContent>
      </Sheet>
      <Dialog
        open={!!recipe()}
        onOpenChange={(o) => (o ? undefined : props.onClose())}
      >
        <DialogContent
          onOpenAutoFocus={(e) => e.preventDefault()}
          onInteractOutside={(e) => e.preventDefault()}
          class="px-0 py-4"
        >
          <div class="flex w-full flex-col gap-4">
            <div class="flex h-16 w-full items-center px-6 pb-4 shadow-lg">
              <div class="flex h-full w-[60%] items-center overflow-hidden">
                <CountryFlag
                  countryCode={selectedBean()?.properties.countryCode ?? ''}
                  class="mr-4 h-8"
                />
                <p class="flex w-full text-xl font-bold whitespace-nowrap">
                  {selectedBean()?.properties.name}
                </p>
              </div>
              <div class="flex h-full w-[50%] flex-col overflow-hidden px-2">
                <p class="flex h-full w-full items-center text-base font-bold whitespace-nowrap">
                  {selectedBean()?.roastery.name}
                </p>
                <p class="flex h-full w-full items-center text-sm whitespace-nowrap">
                  {selectedBean()?.roastery.location}
                </p>
              </div>
            </div>
            <Dynamic component={selectParams()} class="w-full px-6" />
            <Show when={recipe()?.type === 'Espresso' && props.scale}>
              <div class="flex w-full justify-center py-2">
                <Button
                  variant="default"
                  class="flex h-12 w-48 items-center justify-center gap-2 rounded-xl shadow-lg"
                  onClick={start}
                >
                  <Icon iconName="play_arrow" />
                  <T key="start-espresso" />
                </Button>
              </div>
            </Show>
          </div>
        </DialogContent>
      </Dialog>
    </>
  );
};
