import { Component, createEffect, createSignal, Show } from 'solid-js';
import { Button, handleError, Icon, SpinnerButton } from '@micra-pro/shared/ui';
import {
  Spout,
  StartCoffeeByTime,
} from '@micra-pro/brew-by-weight/data-access';
import { T, useTranslationContext } from '../generated/language-types';
import moment from 'moment';
import { twMerge } from 'tailwind-merge';
import { useNavigate } from '@solidjs/router';

export const BrewByTimeContent: Component<{
  recipe: {
    beanId: string;
    coffeeQuantity: number;
    grindSetting: number;
    inCupQuantity: number;
    targetExtractionTime: string;
    spout: Spout;
  };
  onClose: () => void;
}> = (props) => {
  const { t } = useTranslationContext();
  const [isStopping, setIsStopping] = createSignal(false);
  const accessor = StartCoffeeByTime({
    // eslint-disable-next-line solid/reactivity
    targetTime: props.recipe.targetExtractionTime,
  });
  const timeSeconds = (): number => {
    switch (accessor.dataStore.state.__typename) {
      case 'Created':
      case 'BrewByTimeProcessStarted':
      case 'NotStarted':
        return 0;
      case 'BrewByTimeProcessRunning':
      case 'BrewByTimeProcessCancelled':
      case 'BrewByTimeProcessFailed':
        return moment.duration(accessor.dataStore.state.totalTime).asSeconds();
      case 'BrewByTimeProcessFinished':
        return moment
          .duration(accessor.dataStore.state.extractionTime)
          .asSeconds();
    }
  };
  const canClose = () => {
    switch (accessor.dataStore.state.__typename) {
      case 'Created':
      case 'BrewByTimeProcessRunning':
      case 'BrewByTimeProcessStarted':
        return false;
      case 'NotStarted':
      case 'BrewByTimeProcessCancelled':
      case 'BrewByTimeProcessFailed':
      case 'BrewByTimeProcessFinished':
        return true;
    }
  };
  const successClass = () => {
    switch (accessor.dataStore.state.__typename) {
      case 'Created':
      case 'BrewByTimeProcessRunning':
      case 'BrewByTimeProcessStarted':
        return null;
      case 'NotStarted':
      case 'BrewByTimeProcessCancelled':
      case 'BrewByTimeProcessFailed':
        return false;
      case 'BrewByTimeProcessFinished':
        return true;
    }
  };
  createEffect(() => {
    switch (accessor.dataStore.state.__typename) {
      case 'Created':
      case 'BrewByTimeProcessFinished':
      case 'BrewByTimeProcessRunning':
      case 'BrewByTimeProcessCancelled':
      case 'BrewByTimeProcessStarted':
        return;
      case 'NotStarted':
        handleError({ title: t('error'), message: t('unknown-error') });
        return;
      case 'BrewByTimeProcessFailed':
        switch (accessor.dataStore.state.exception.__typename) {
          case 'ScaleConnectionFailed':
            handleError({
              title: t('error'),
              message: t('scale-connection-failed'),
            });
            return;
          case 'BrewServiceNotReady':
            handleError({
              title: t('error'),
              message: t('not-ready'),
            });
            return;
          default:
            handleError({ title: t('error'), message: t('unknown-error') });
            return;
        }
    }
  });
  const targetTimeSeconds = () =>
    moment.duration(props.recipe.targetExtractionTime).asSeconds();

  const navigate = useNavigate();
  const edit = () => {
    navigate(`menu/beans?beanId=${props.recipe.beanId}&showEspresso=true`);
  };
  return (
    <div class="flex w-full flex-col items-center justify-center gap-6">
      <div class="relative">
        <div
          class={twMerge(
            'bg-primary text-primary-foreground flex h-80 w-80 flex-col overflow-hidden rounded-full shadow-lg',
            successClass() === true
              ? 'shadow-positive'
              : successClass() === false
                ? 'shadow-destructive'
                : 'shadow-primary-shadow',
          )}
        >
          <div class="flex h-full w-full items-center justify-center">
            <div class="inset-shadow-primary-shadow w-1/3 rounded-lg py-2 inset-shadow-sm">
              <div class="flex w-full items-center justify-center text-2xl font-bold">
                {timeSeconds().toFixed(1)}
              </div>
              <div class="flex w-full items-center justify-center text-sm">
                <T key="time" /> [s]
              </div>
              <div class="flex w-full items-center justify-center text-2xl">
                {targetTimeSeconds().toFixed(1)}
              </div>
            </div>
          </div>
        </div>
        <Show when={canClose()}>
          <Button
            variant="outline"
            class="border-border absolute right-1 bottom-1 flex h-16 w-16 items-center justify-center rounded-full border text-lg shadow-xl"
            onClick={edit}
          >
            <Icon iconName="edit" />
          </Button>
        </Show>
      </div>
      <Show when={!canClose()}>
        <SpinnerButton
          variant="outline"
          class="border-destructive h-12 w-36 text-lg shadow-xl"
          onClick={() => {
            accessor.cancel();
            setIsStopping(true);
          }}
          loading={isStopping()}
        >
          <T key="stop" />
        </SpinnerButton>
      </Show>
      <Show when={canClose()}>
        <Button
          variant="outline"
          class="border-primary h-12 w-36 text-lg shadow-xl"
          onClick={() => props.onClose()}
        >
          <T key="close" />
        </Button>
      </Show>
    </div>
  );
};
