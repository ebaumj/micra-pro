import { Component, createEffect, createSignal, Show } from 'solid-js';
import { Button, handleError, Icon, SpinnerButton } from '@micra-pro/shared/ui';
import { Spout, StartCoffee } from '@micra-pro/brew-by-weight/data-access';
import { T, useTranslationContext } from '../generated/language-types';
import moment from 'moment';
import { twMerge } from 'tailwind-merge';
import { PannelGraph } from './PannelGraph';
import { PannelGauge } from './PannelGauge';
import { usePannelStyle } from './PannelStyleProvider';
import { calculateExtractionTimeResult } from '../utils/extraction-time-result';
import { useExtractionTimeThreshold } from './ExtractionTimeThresholdProvider';
import { useNavigate } from '@solidjs/router';

export const BrewByWeightContent: Component<{
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
  const pannelStyle = usePannelStyle().pannelStyle;
  // eslint-disable-next-line solid/reactivity
  const accessor = StartCoffee(props.recipe);
  const extractionTimeThreshold = useExtractionTimeThreshold();
  const isStarting = () => {
    switch (accessor.dataStore.state.__typename) {
      case 'Created':
      case 'BrewProcessStarted':
        return true;
      case 'NotStarted':
      case 'BrewProcessRunning':
      case 'BrewProcessCancelled':
      case 'BrewProcessFailed':
      case 'BrewProcessFinished':
        return false;
    }
  };
  const flow = (): number => {
    switch (accessor.dataStore.state.__typename) {
      case 'Created':
      case 'BrewProcessStarted':
      case 'NotStarted':
        return 0;
      case 'BrewProcessRunning':
        return accessor.dataStore.state.flow;
      case 'BrewProcessCancelled':
      case 'BrewProcessFailed':
      case 'BrewProcessFinished':
        return accessor.dataStore.state.averageFlow;
    }
  };
  const timeSeconds = (): number => {
    switch (accessor.dataStore.state.__typename) {
      case 'Created':
      case 'BrewProcessStarted':
      case 'NotStarted':
        return 0;
      case 'BrewProcessRunning':
      case 'BrewProcessCancelled':
      case 'BrewProcessFailed':
        return moment.duration(accessor.dataStore.state.totalTime).asSeconds();
      case 'BrewProcessFinished':
        return moment
          .duration(accessor.dataStore.state.extractionTime)
          .asSeconds();
    }
  };
  const quantity = (): number => {
    switch (accessor.dataStore.state.__typename) {
      case 'Created':
      case 'BrewProcessStarted':
      case 'NotStarted':
        return 0;
      case 'BrewProcessRunning':
      case 'BrewProcessCancelled':
      case 'BrewProcessFailed':
      case 'BrewProcessFinished':
        return accessor.dataStore.state.totalQuantity;
    }
  };
  const canClose = () => {
    switch (accessor.dataStore.state.__typename) {
      case 'Created':
      case 'BrewProcessRunning':
      case 'BrewProcessStarted':
        return false;
      case 'NotStarted':
      case 'BrewProcessCancelled':
      case 'BrewProcessFailed':
      case 'BrewProcessFinished':
        return true;
    }
  };
  const successClass = () => {
    switch (accessor.dataStore.state.__typename) {
      case 'Created':
      case 'BrewProcessRunning':
      case 'BrewProcessStarted':
        return null;
      case 'NotStarted':
      case 'BrewProcessCancelled':
      case 'BrewProcessFailed':
        return false;
      case 'BrewProcessFinished':
        return true;
    }
  };
  const extractionTimeResultValid = (): boolean => {
    switch (accessor.dataStore.state.__typename) {
      case 'Created':
      case 'BrewProcessRunning':
      case 'BrewProcessStarted':
      case 'NotStarted':
      case 'BrewProcessCancelled':
      case 'BrewProcessFailed':
        return false;
      case 'BrewProcessFinished':
        return true;
    }
  };
  createEffect(() => {
    switch (accessor.dataStore.state.__typename) {
      case 'Created':
      case 'BrewProcessFinished':
      case 'BrewProcessRunning':
      case 'BrewProcessCancelled':
      case 'BrewProcessStarted':
        return;
      case 'NotStarted':
        handleError({ title: t('error'), message: t('unknown-error') });
        return;
      case 'BrewProcessFailed':
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
          <Show when={pannelStyle() === 'Graph'}>
            <PannelGraph
              brewData={accessor.dataStore.brewData}
              flow={flow()}
              isStarting={isStarting()}
              quantity={quantity()}
              targetTime={targetTimeSeconds()}
              targetQuantity={props.recipe.inCupQuantity}
              time={timeSeconds()}
              extractionTimeResult={calculateExtractionTimeResult(
                extractionTimeResultValid(),
                extractionTimeThreshold,
                timeSeconds(),
                targetTimeSeconds(),
              )}
            />
          </Show>
          <Show when={pannelStyle() === 'Gauge'}>
            <PannelGauge
              flow={flow()}
              isStarting={isStarting()}
              quantity={quantity()}
              targetTime={targetTimeSeconds()}
              targetQuantity={props.recipe.inCupQuantity}
              time={timeSeconds()}
              extractionTimeResult={calculateExtractionTimeResult(
                extractionTimeResultValid(),
                extractionTimeThreshold,
                timeSeconds(),
                targetTimeSeconds(),
              )}
            />
          </Show>
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
