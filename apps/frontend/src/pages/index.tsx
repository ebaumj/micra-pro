import { Button, CountryFlag, Icon } from '@micra-pro/shared/ui';
import { A } from '@solidjs/router';
import { Component, createSignal, For, ParentComponent, Show } from 'solid-js';
import { LanguageSelector } from '@micra-pro/shared/ui';
import {
  Bean,
  EspressoProperties,
  fetchBeansLevel,
} from '@micra-pro/bean-management/data-access';
import { MainScreenConfig } from './MainscreenConfigPage';
import { Asset } from '@micra-pro/asset-management/feature';
import { RecipePannel } from '@micra-pro/bean-management/feature';
import {
  BrewByWeightPannel,
  SpoutSelector,
} from '@micra-pro/brew-by-weight/feature';
import { createConfigAccessor } from '@micra-pro/shared/utils-ts';
import { SettingsButton } from '../components/SettingsButton';

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
              <div class="bg-card text-card-foreground flex h-full w-full flex-col items-center justify-center rounded-lg border text-sm shadow-md">
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
  const configAccessor =
    createConfigAccessor<MainScreenConfig>('MainScreenConfig');

  const isLoading = () => beans.isLoading() || !configAccessor.config();

  const [selectedBean, setSelectedBean] = createSignal<string | null>(null);

  const startEspressoBrewing = (beanId: string, recipe: EspressoProperties) => {
    setRecipe({
      beanId: beanId,
      coffeeQuantity: recipe.coffeeQuantity,
      grindSetting: recipe.grindSetting,
      inCupQuantity: recipe.inCupQuantity,
      targetExtractionTime: recipe.targetExtractionTime,
    });
    setSelectedBean(null);
  };

  const [recipe, setRecipe] = createSignal<{
    beanId: string;
    coffeeQuantity: number;
    grindSetting: number;
    inCupQuantity: number;
    targetExtractionTime: string;
  } | null>(null);

  return (
    <Layout refetch={beans.refetch}>
      <div class="flex h-full w-full items-center justify-center">
        <Show when={!isLoading()}>
          <RecipePannel
            beans={beans.beans()}
            onClose={() => setSelectedBean(null)}
            beanId={selectedBean()}
            startEspressoBrewing={startEspressoBrewing}
          />
          <BrewByWeightPannel
            recipe={recipe()}
            onClose={() => setRecipe(null)}
          />
          <div class="w-full">
            <BeanButtons
              beans={beans.beans()}
              config={configAccessor.config()!}
              onButtonClick={(b) => setSelectedBean(b.id)}
            />
          </div>
        </Show>
      </div>
    </Layout>
  );
}

const Layout: ParentComponent<{ refetch?: () => void }> = (props) => {
  return (
    <div class="relative h-full w-full">
      <A href="/menu" class="absolute z-10 mt-1 ml-3 active:opacity-50">
        <Icon iconName="menu" class="text-5xl" />
      </A>
      <div class="absolute flex h-full w-full flex-col">
        <div class="flex h-16 w-full items-center gap-2 pr-2 pl-20 shadow-md">
          <div class="w-full" />
          <SpoutSelector class="w-36 min-w-36" />
          <LanguageSelector class="w-40" />
          <Button
            variant="outline"
            class="w-24"
            onClick={() => location.reload()}
          >
            <Icon iconName="refresh" />
          </Button>
          <SettingsButton class="w-24" onSettingChanged={props.refetch} />
        </div>
        <div class="h-full w-full">{props.children}</div>
      </div>
    </div>
  );
};

export default MainScreen;
