import { ScaleSelector } from '@micra-pro/scale-management/feature';
import { CountryFlag, Icon } from '@micra-pro/shared/ui';
import { A } from '@solidjs/router';
import { Component, createSignal, For, ParentComponent, Show } from 'solid-js';
import { T } from '../generated/language-types';
import { LanguageSelector } from '@micra-pro/shared/ui';
import {
  Bean,
  EspressoProperties,
  fetchBeansLevel,
} from '@micra-pro/bean-management/data-access';
import { MainScreenConfig, readMainScreenConfig } from './MainscreenConfigPage';
import { Asset } from '@micra-pro/asset-management/feature';
import { RecipePannel } from '@micra-pro/bean-management/feature';
import { BrewByWeightPannel } from '@micra-pro/brew-by-weight/feature';
import { useSelectedScaleContext } from '@micra-pro/scale-management/feature';

const BeanButtons: Component<{
  beans: Bean[];
  config: MainScreenConfig;
  onButtonClick: (button: Bean) => void;
}> = (props) => {
  const buttons = () =>
    props.config.buttons
      .map((btn) => ({
        bean: props.beans.find((b) => b.id === btn.beanId),
        pos: btn.pos,
      }))
      .filter((b) => b.bean)
      .map((b) => ({ bean: b.bean!, pos: b.pos }));
  return (
    <div
      class="mx-8 grid gap-8"
      style={{
        'grid-template-columns': `repeat(${props.config.numberOfButtonsColumns}, minmax(0, 1fr))`,
        'grid-template-rows': `repeat(${props.config.numberOfButtonRows}, minmax(0, 1fr))`,
      }}
    >
      <For each={buttons()}>
        {(btn) => (
          <div
            class="grid aspect-square items-center before:col-start-1 before:col-end-2 before:row-start-1 before:row-end-2 before:block before:pb-[100%]"
            style={{
              'grid-row-start': btn.pos.y + 1,
              'grid-column-start': btn.pos.x + 1,
            }}
            onClick={() => props.onButtonClick(btn.bean)}
          >
            <div class="col-start-1 col-end-2 row-start-1 row-end-2 h-full">
              <div class="flex h-full w-full flex-col items-center justify-center rounded-lg border text-sm shadow-md">
                <Show when={btn.bean.properties.assetId}>
                  <Asset
                    class="h-24 w-32 object-contain"
                    assetId={btn.bean.properties.assetId ?? undefined}
                  />
                </Show>
                <Show when={!btn.bean.properties.assetId}>
                  <CountryFlag
                    countryCode={btn.bean.properties.countryCode}
                    class="px-6"
                  />
                </Show>
                <div class="pt-2 font-bold">{btn.bean.properties.name}</div>
                <div class="">{btn.bean.roastery.name}</div>
              </div>
            </div>
          </div>
        )}
      </For>
    </div>
  );
};

function MainScreen() {
  const beans = fetchBeansLevel();
  const config = readMainScreenConfig();
  const scales = useSelectedScaleContext();

  const isLoading = () => beans.isLoading() || !config.latest;

  const [selectedBean, setSelectedBean] = createSignal<string | null>(null);

  const startEspressoBrewing = (beanId: string, recipe: EspressoProperties) => {
    const scale = scales.selectedScale();
    if (scale)
      setRecipe({
        beanId: beanId,
        coffeeQuantity: recipe.coffeeQuantity,
        grindSetting: recipe.grindSetting,
        inCupQuantity: recipe.inCupQuantity,
        scaleId: scale,
        targetExtractionTime: recipe.targetExtractionTime,
      });
    setSelectedBean(null);
  };

  const [recipe, setRecipe] = createSignal<{
    beanId: string;
    coffeeQuantity: number;
    grindSetting: number;
    inCupQuantity: number;
    scaleId: string;
    targetExtractionTime: string;
  } | null>(null);

  return (
    <Layout>
      <div class="flex h-full w-full items-center justify-center">
        <Show when={isLoading()}>
          <div class="h-full w-full p-4">Main</div>
        </Show>
        <Show when={!isLoading()}>
          <RecipePannel
            beans={beans.beans()}
            onClose={() => setSelectedBean(null)}
            beanId={selectedBean()}
            startEspressoBrewing={startEspressoBrewing}
            scale={scales.selectedScale()}
          />
          <BrewByWeightPannel
            recipe={recipe()}
            onClose={() => setRecipe(null)}
          />
          <div class="w-full">
            <BeanButtons
              beans={beans.beans()}
              config={config.latest!}
              onButtonClick={(b) => setSelectedBean(b.id)}
            />
          </div>
        </Show>
      </div>
    </Layout>
  );
}

const Layout: ParentComponent = (props) => {
  return (
    <div class="relative h-full w-full">
      <A href="/menu" class="absolute z-10 ml-3 mt-1 active:opacity-50">
        <Icon iconName="menu" class="text-5xl" />
      </A>
      <div class="absolute flex h-full w-full flex-col">
        <div class="flex h-16 w-full items-center gap-2 pl-20 pr-2 shadow-md">
          <div class="w-full" />
          <div class="text-sm">
            <T key="scale" />:
          </div>
          <ScaleSelector class="w-64" />
          <LanguageSelector class="w-40" />
        </div>
        <div class="h-full w-full">{props.children}</div>
      </div>
    </div>
  );
};

export default MainScreen;
