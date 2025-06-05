import {
  ScaleSelector,
  ScaleSelectorContextProvider,
} from '@micra-pro/scale-management/feature';
import { CountryFlag, Icon } from '@micra-pro/shared/ui';
import { A } from '@solidjs/router';
import { Component, For, ParentComponent, Show } from 'solid-js';
import { T } from '../generated/language-types';
import { LanguageSelector } from '@micra-pro/shared/ui';
import { Bean, fetchBeansLevel } from '@micra-pro/bean-management/data-access';
import { MainScreenConfig, readMainScreenConfig } from './MainscreenConfigPage';

const BeanButtons: Component<{
  beans: Bean[];
  config: MainScreenConfig;
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
          >
            <div class="col-start-1 col-end-2 row-start-1 row-end-2 h-full">
              <div class="flex h-full w-full flex-col items-center justify-center rounded-lg border text-sm shadow-md">
                <CountryFlag
                  countryCode={btn.bean.properties.countryCode}
                  class="px-6"
                />
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

  const isLoading = () => beans.isLoading() || !config.latest;

  return (
    <Layout>
      <div class="flex h-full w-full items-center justify-center">
        <Show when={isLoading()}>
          <div class="h-full w-full p-4">Main</div>
        </Show>
        <Show when={!isLoading()}>
          <div class="w-full">
            <BeanButtons beans={beans.beans()} config={config.latest!} />
          </div>
        </Show>
      </div>
    </Layout>
  );
}

const Layout: ParentComponent = (props) => {
  return (
    <ScaleSelectorContextProvider>
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
    </ScaleSelectorContextProvider>
  );
};

export default MainScreen;
