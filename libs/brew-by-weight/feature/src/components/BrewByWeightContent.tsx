import { Component, createEffect, createSignal, Show } from 'solid-js';
import { Button, handleError, SpinnerButton } from '@micra-pro/shared/ui';
import { Spout, StartCoffee } from '@micra-pro/brew-by-weight/data-access';
import { T, useTranslationContext } from '../generated/language-types';
import moment from 'moment';
import { twMerge } from 'tailwind-merge';
import { PannelGraph } from './PannelGraph';
import { PannelGauge } from './PannelGauge';
import { usePannelStyle } from './PannelStyleProvider';
import { StartFlowProfiling } from '@micra-pro/flow-profiling/data-access';
import { createStore } from 'solid-js/store';

export const BrewByWeightContent: Component<{
  recipe: {
    beanId: string;
    coffeeQuantity: number;
    grindSetting: number;
    inCupQuantity: number;
    targetExtractionTime: string;
    flowProfile?: {
      startFlow: number;
      dataPoints: { flow: number; time: any }[];
    };
    spout: Spout;
  };
  onClose: () => void;
}> = (props) => {
  const { t } = useTranslationContext();
  const [isStopping, setIsStopping] = createSignal(false);
  const [flowInside, setFlowInside] = createSignal(0);
  const pannelStyle = usePannelStyle().pannelStyle;
  // eslint-disable-next-line solid/reactivity
  const accessor = StartCoffee(props.recipe);
  // eslint-disable-next-line solid/reactivity
  const flowProfilingAccessor = props.recipe.flowProfile
    ? // eslint-disable-next-line solid/reactivity
      StartFlowProfiling(props.recipe.flowProfile)
    : undefined;
  const [runtimeData, setRuntimData] = createStore<{
    data: {
      flowOutside: number;
      flowInside: number;
      quantity: number;
      time: number;
    }[];
  }>({ data: [{ flowOutside: 0, flowInside: 0, quantity: 0, time: 0 }] });
  const addRuntimeDataInside = (flow: number, time: any) => {
    if (
      !runtimeData.data.find((d) => d.flowInside === flow && d.time === time)
    ) {
      const last = runtimeData.data[runtimeData.data.length - 1];
      setRuntimData('data', runtimeData.data.length, {
        flowInside: flow,
        flowOutside: last.flowOutside,
        quantity: last.quantity,
        time: time,
      });
    }
  };
  const addRuntimeDataOutside = (flow: number, quantity: number, time: any) => {
    if (
      !runtimeData.data.find(
        (d) =>
          d.flowOutside === flow && d.time === time && d.quantity === quantity,
      )
    ) {
      const last = runtimeData.data[runtimeData.data.length - 1];
      setRuntimData('data', runtimeData.data.length, {
        flowInside: last.flowInside,
        flowOutside: flow,
        quantity: quantity,
        time: time,
      });
    }
  };
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
  createEffect(() => {
    switch (flowProfilingAccessor?.dataStore.state.__typename) {
      case 'Created':
      case 'FlowProfilingProcessStarted':
      case 'NotStarted':
      case undefined:
        setFlowInside(0);
        break;
      case 'FlowProfilingProcessFailed':
      case 'FlowProfilingProcessCancelled':
      case 'FlowProfilingProcessFinished':
        setFlowInside(flowProfilingAccessor.dataStore.state.averageFlow);
        break;
      case 'FlowProfilingProcessRunning':
        setFlowInside(flowProfilingAccessor.dataStore.state.flow);
        break;
    }
  });
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
  createEffect(() => {
    switch (accessor.dataStore.state.__typename) {
      case 'Created':
      case 'BrewProcessStarted':
        return;
      case 'BrewProcessRunning':
        addRuntimeDataOutside(
          accessor.dataStore.state.flow,
          accessor.dataStore.state.totalQuantity,
          accessor.dataStore.state.totalTime,
        );
        return;
      case 'BrewProcessFinished':
      case 'BrewProcessCancelled':
        flowProfilingAccessor?.cancel();
        return;
      case 'NotStarted':
        handleError({ title: t('error'), message: t('unknown-error') });
        flowProfilingAccessor?.cancel();
        return;
      case 'BrewProcessFailed':
        flowProfilingAccessor?.cancel();
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
  createEffect(() => {
    const state = flowProfilingAccessor?.dataStore.state;
    switch (state?.__typename) {
      case undefined:
      case 'Created':
      case 'FlowProfilingProcessFinished':
      case 'FlowProfilingProcessProfileDone':
      case 'FlowProfilingProcessCancelled':
      case 'FlowProfilingProcessStarted':
        return;
      case 'FlowProfilingProcessRunning':
        addRuntimeDataInside(state.flow, state.totalTime);
        return;
      case 'NotStarted':
        handleError({ title: t('error'), message: t('unknown-error') });
        return;
      case 'FlowProfilingProcessFailed':
        switch (state.exception.__typename) {
          case 'FlowRegulationNotAvailable':
            handleError({
              title: t('error'),
              message: t('flow-profile-not-availble'),
            });
            return;
          case 'FlowProfilingServiceNotReady':
            handleError({
              title: t('error'),
              message: t('flow-profile-not-ready'),
            });
            return;
          default:
            handleError({ title: t('error'), message: t('unknown-error') });
            return;
        }
    }
  });
  return (
    <div class="flex w-full flex-col items-center justify-center gap-6">
      <div class="bg-accent-alt invisible h-0 w-0 overflow-hidden">
        {/* Color import Hack */}
      </div>
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
            brewData={runtimeData.data}
            useFlowProfiling={!!flowProfilingAccessor}
            flow={flow()}
            isStarting={isStarting()}
            quantity={quantity()}
            targetTime={moment
              .duration(props.recipe.targetExtractionTime)
              .asSeconds()}
            targetQuantity={props.recipe.inCupQuantity}
            time={timeSeconds()}
          />
        </Show>
        <Show when={pannelStyle() === 'Gauge'}>
          <PannelGauge
            useFlowProfiling={!!flowProfilingAccessor}
            flowInside={flowInside()}
            flow={flow()}
            isStarting={isStarting()}
            quantity={quantity()}
            targetTime={moment
              .duration(props.recipe.targetExtractionTime)
              .asSeconds()}
            targetQuantity={props.recipe.inCupQuantity}
            time={timeSeconds()}
          />
        </Show>
      </div>
      <Show when={!canClose()}>
        <SpinnerButton
          variant="outline"
          class="border-destructive h-12 w-36 text-lg shadow-xl"
          onClick={() => {
            accessor.cancel();
            flowProfilingAccessor?.cancel();
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
