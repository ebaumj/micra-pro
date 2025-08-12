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
  Switch,
  SwitchControl,
  SwitchThumb,
} from '@micra-pro/shared/ui';
import { Component, createEffect, createSignal, Show } from 'solid-js';
import { Dynamic } from 'solid-js/web';
import { T } from '../generated/language-types';
import { JSX } from 'solid-js/jsx-runtime';
import moment from 'moment';
import picturesImport from '../generated/pictures-import';
import { twMerge } from 'tailwind-merge';
import { IsFlowProfilingAvailable } from '@micra-pro/flow-profiling/data-access';

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
  startEspressoBrewing: (
    beanId: string,
    recipe: EspressoProperties,
    flowProfile?: {
      startFlow: number;
      dataPoints: { flow: number; time: any }[];
    },
  ) => void;
  onClose: () => void;
}> = (props) => {
  const pictures = selectPicturesForMode(picturesImport);
  const flowProfileAvailable = IsFlowProfilingAvailable();
  const [recipe, setRecipe] = createSignal<
    | ({
        type: 'Espresso';
        flowProfile?: {
          startFlow: number;
          dataPoints: { flow: number; time: any }[];
        };
      } & EspressoProperties)
    | ({ type: 'V60' } & V60Properties)
    | null
  >(null);
  const [useFlowProfiling, setUseFlowProfiling] = createSignal(true);
  createEffect(() => {
    if (!props.beanId) setRecipe(null);
  });
  const selectedBean = () =>
    props.beans.find((b) => b.id === props.beanId) ?? null;

  const espresso = () =>
    selectedBean()?.recipes.find((r) => r.__typename == 'EspressoProperties');
  const v60 = () =>
    selectedBean()?.recipes.find((r) => r.__typename == 'V60Properties');

  createEffect(() => {
    const espressoRecipe = espresso();
    const v60Recipe = v60();
    if (espressoRecipe && !v60Recipe)
      setRecipe({
        type: 'Espresso',
        flowProfile: parseFlowProfile(espressoRecipe.flowProfile),
        ...espressoRecipe.properties,
      });
    if (v60Recipe && !espressoRecipe)
      setRecipe({ type: 'V60', ...v60Recipe.properties });
    if (!v60Recipe && !espressoRecipe) props.onClose();
  });

  const parseFlowProfile = (profile?: {
    startFlow: number;
    flowSettings: { flow: number; time: any }[];
  }) =>
    profile
      ? {
          startFlow: profile.startFlow,
          dataPoints: profile.flowSettings,
        }
      : undefined;

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
      props.startEspressoBrewing(
        props.beanId,
        rec,
        useFlowProfiling() && flowProfileAvailable.isAvailable()
          ? rec.flowProfile
          : undefined,
      );
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
                {(rec) => (
                  <Button
                    variant="outline"
                    class="flex h-14 w-full justify-center py-2"
                    onClick={() =>
                      setRecipe({
                        type: 'Espresso',
                        flowProfile: parseFlowProfile(rec().flowProfile),
                        ...rec().properties,
                      })
                    }
                  >
                    <img
                      src={pictures().espresso}
                      class={'h-full object-contain'}
                    />
                  </Button>
                )}
              </Show>
              <Show when={v60()?.properties}>
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
            <Show when={recipe()?.type === 'Espresso'}>
              <div class="flex w-full justify-center gap-4 py-2">
                <Button
                  variant="default"
                  class="flex h-12 w-48 items-center justify-center gap-2 rounded-xl shadow-lg"
                  onClick={start}
                >
                  <Icon iconName="play_arrow" />
                  <T key="start-espresso" />
                </Button>
                <Show
                  when={
                    espresso()?.flowProfile &&
                    flowProfileAvailable.isAvailable()
                  }
                >
                  <div
                    class={twMerge(
                      'flex h-12 items-center justify-center gap-2 rounded-lg border px-4',
                    )}
                  >
                    <Icon
                      iconName="area_chart"
                      class={useFlowProfiling() ? '' : 'opacity-50'}
                    />
                    <Switch
                      checked={useFlowProfiling()}
                      onChange={setUseFlowProfiling}
                      class="flex h-full items-center"
                    >
                      <SwitchControl>
                        <SwitchThumb />
                      </SwitchControl>
                    </Switch>
                  </div>
                </Show>
              </div>
            </Show>
          </div>
        </DialogContent>
      </Dialog>
    </>
  );
};
