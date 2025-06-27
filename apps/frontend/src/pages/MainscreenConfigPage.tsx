import { Asset } from '@micra-pro/asset-management/feature';
import {
  fetchRoasteriesLevel,
  Roastery,
} from '@micra-pro/bean-management/data-access';
import { BeanSelectorDialog } from '@micra-pro/bean-management/feature';
import {
  AlertDialog,
  AlertDialogContent,
  CountryFlag,
  Icon,
  LongPressDiv,
  Spinner,
} from '@micra-pro/shared/ui';
import { createConfigAccessor } from '@micra-pro/shared/utils-ts';
import { trackStore } from '@solid-primitives/deep';
import { Component, createEffect, createSignal, For, on, Show } from 'solid-js';
import { createStore } from 'solid-js/store';

export type MainScreenConfig = {
  numberOfButtonsColumns: number;
  numberOfButtonRows: number;
  buttons: {
    beanId: string;
    pos: {
      x: number;
      y: number;
    };
  }[];
};

const defaultConfig: MainScreenConfig = {
  numberOfButtonsColumns: 4,
  numberOfButtonRows: 2,
  buttons: [],
};

const BeanPreview: Component<{
  beanId: string;
  roasteries: Roastery[];
}> = (props) => {
  const beanInfo = () =>
    props.roasteries
      .flatMap((r) =>
        r.beans.map((b) => ({
          name: b.properties.name,
          country: b.properties.countryCode,
          roastery: r.properties.name,
          id: b.id,
          assetId: b.properties.assetId ?? undefined,
        })),
      )
      .find((b) => b.id === props.beanId);
  return (
    <div class="h-full w-full">
      <Show when={beanInfo()}>
        <div class="flex h-full w-full flex-col items-center justify-center text-sm">
          <Show when={beanInfo()!.assetId}>
            <Asset
              class="flex h-16 w-full items-center justify-center object-contain px-2 py-0"
              assetId={beanInfo()!.assetId}
            />
          </Show>
          <Show when={!beanInfo()!.assetId}>
            <CountryFlag countryCode={beanInfo()!.country} class="px-6" />
          </Show>
          <div class="pt-2 font-bold">{beanInfo()!.name}</div>
          <div class="">{beanInfo()!.roastery}</div>
        </div>
      </Show>
      <Show when={!beanInfo()}>
        <div class="flex h-full w-full items-center justify-center">
          <Icon iconName="warning" class="text-destructive" />
        </div>
      </Show>
    </div>
  );
};

export const MainscreenConfigPage: Component<{}> = () => {
  const [currentPosition, setCurrentPosition] = createSignal<
    { x: number; y: number } | undefined
  >();
  const configAccessor = createConfigAccessor('MainScreenConfig');
  const [config, setConfig] = createStore(defaultConfig);
  const roasteries = fetchRoasteriesLevel();
  const [deleting, setDeleting] = createSignal(false);
  const isLoading = () => roasteries.isLoading() || configAccessor.loading();
  createEffect(
    on(
      configAccessor.loading,
      (loading) => {
        if (!loading) {
          const read = configAccessor.config();
          if (read) setConfig(read);
        }
      },
      { defer: true },
    ),
  );
  createEffect(
    () =>
      !configAccessor.loading() &&
      configAccessor.writeConfig(trackStore(config)),
  );
  const tileLongPress = (x: number, y: number) => {
    const index = config.buttons.findIndex(
      (b) => b.pos.x === x && b.pos.y === y,
    );
    if (index >= 0)
      setConfig(
        'buttons',
        config.buttons.filter((_, i) => i !== index),
      );
    setDeleting(false);
  };
  const addOrUpdate = (beanId: string) => {
    const pos = currentPosition();
    if (!pos) return;
    const index = config.buttons.findIndex(
      (b) => b.pos.x === pos.x && b.pos.y === pos.y,
    );
    const entry = { beanId: beanId, pos: { x: pos.x, y: pos.y } };
    if (index >= 0) setConfig('buttons', index, entry);
    else setConfig('buttons', config.buttons.length, entry);
    setCurrentPosition(undefined);
  };
  const beanId = (x: number, y: number) =>
    config.buttons.find((b) => b.pos.x === x && b.pos.y === y)?.beanId;
  return (
    <div class="flex h-full w-full flex-col">
      <AlertDialog open={isLoading()}>
        <AlertDialogContent class="flex items-center justify-center p-8">
          <Spinner class="h-20 w-20" />
        </AlertDialogContent>
      </AlertDialog>
      <BeanSelectorDialog
        isOpen={!!currentPosition()}
        onBeanSelected={addOrUpdate}
        onClose={() => setCurrentPosition(undefined)}
        roasteries={roasteries.roasteries()}
      />
      <div class="flex h-full items-center">
        <div
          class="grid w-full gap-2"
          style={{
            'grid-template-columns': `repeat(${config.numberOfButtonsColumns}, minmax(0, 1fr))`,
            'grid-template-rows': `repeat(${config.numberOfButtonRows}, minmax(0, 1fr))`,
          }}
        >
          <For each={[...Array(config.numberOfButtonsColumns).keys()]}>
            {(x) => (
              <For each={[...Array(config.numberOfButtonRows).keys()]}>
                {(y) => (
                  <div
                    class="grid items-center before:col-start-1 before:col-end-2 before:row-start-1 before:row-end-2 before:block before:pb-[100%]"
                    style={{
                      'grid-row-start': y + 1,
                      'grid-column-start': x + 1,
                    }}
                  >
                    <div class="col-start-1 col-end-2 row-start-1 row-end-2 h-full">
                      <LongPressDiv
                        class="flex h-full w-full items-center justify-center rounded-lg border"
                        onClick={() => setCurrentPosition({ x, y })}
                        onLongPress={() => tileLongPress(x, y)}
                        onPressStart={() => setDeleting(true)}
                        onPressEnd={() => setDeleting(false)}
                        delayTimeMs={1000}
                        maxShortPressTimeMs={300}
                      >
                        <Show when={beanId(x, y)}>
                          <BeanPreview
                            beanId={beanId(x, y)!}
                            roasteries={roasteries.roasteries()}
                          />
                        </Show>
                      </LongPressDiv>
                    </div>
                  </div>
                )}
              </For>
            )}
          </For>
        </div>
      </div>
      <div class="flex h-12 w-full items-center justify-end gap-2 px-2">
        <Show when={deleting()}>
          <Icon iconName="delete" class="text-destructive" />
          <div class="h-6 w-6 rotate-45 animate-spin-loader-1s rounded-full bg-destructive" />
        </Show>
      </div>
    </div>
  );
};
