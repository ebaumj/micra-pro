import { Component, createEffect, createSignal, Show } from 'solid-js';
import {
  Button,
  handleError,
  LineChart,
  Spinner,
  SpinnerButton,
  twColor,
} from '@micra-pro/shared/ui';
import { Spout, StartCoffee } from '@micra-pro/brew-by-weight/data-access';
import { T, useTranslationContext } from '../generated/language-types';
import moment from 'moment';
import { twMerge } from 'tailwind-merge';
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
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const _colorImportHack = 'bg-accent-alt';
  return (
    <div class="flex w-full flex-col items-center justify-center gap-6">
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
        <div class="flex h-[35%] py-4">
          <div class="flex h-full w-1/2 flex-col gap-0 border-r pt-8 pl-8">
            <div class="flex h-full w-full items-center justify-center text-2xl font-bold">
              {flow().toFixed(1)}
            </div>
            <div class="flex h-6 w-full items-center justify-center text-sm">
              <T key="flow" /> [ml/s]
            </div>
          </div>
          <div class="flex h-full w-1/2 flex-col gap-0 border-r pt-8 pr-8">
            <div class="flex h-full w-full items-center justify-center text-2xl font-bold">
              {quantity().toFixed(1)}
            </div>
            <div class="flex h-6 w-full items-center justify-center text-sm">
              <T key="liquid" /> [ml]
            </div>
          </div>
        </div>
        <div class="h-[30%] px-10 py-0">
          <div class="flex h-full w-full items-center justify-center">
            <Show when={!isStarting()}>
              <LineChart
                data={{
                  labels: runtimeData.data.map((d) =>
                    moment.duration(d.time).asSeconds(),
                  ),
                  datasets: [
                    {
                      data: runtimeData.data.map((d) => d.flowOutside),
                      label: t('flow'),
                      pointStyle: false,
                      animation: false,
                      borderColor: twColor('accent'),
                    },
                    {
                      data: flowProfilingAccessor
                        ? runtimeData.data.map((d) => d.flowInside)
                        : [],
                      label: t('flow'),
                      pointStyle: false,
                      animation: false,
                      borderColor: twColor('accent-alt'),
                    },
                    {
                      data: runtimeData.data.map((d) => d.quantity),
                      label: t('liquid'),
                      pointStyle: false,
                      animation: false,
                      borderColor: twColor('accent-variant'),
                      yAxisID: 'y2',
                    },
                  ],
                }}
                options={{
                  maintainAspectRatio: false,
                  scales: {
                    x: {
                      display: false,
                      grid: {
                        display: false,
                      },
                    },
                    y: {
                      display: true,
                      ticks: {
                        color: twColor('accent'),
                      },
                      min: 0,
                      max:
                        Math.max(
                          ...runtimeData.data.map((d) => {
                            if (
                              Number.isNaN(d.flowInside) &&
                              Number.isNaN(d.flowOutside)
                            )
                              return 0;
                            if (
                              Number.isNaN(d.flowInside) &&
                              !Number.isNaN(d.flowOutside)
                            )
                              return d.flowOutside;
                            if (
                              !Number.isNaN(d.flowInside) &&
                              Number.isNaN(d.flowOutside)
                            )
                              return d.flowInside;
                            if (d.flowInside > d.flowOutside)
                              return d.flowInside;
                            return d.flowOutside;
                          }),
                        ) * 1.1,
                      grid: {
                        display: false,
                      },
                    },
                    y2: {
                      axis: 'y',
                      display: true,
                      ticks: {
                        color: twColor('accent-variant'),
                      },
                      position: 'right',
                      min: 0,
                      max:
                        Math.max(
                          ...accessor.dataStore.brewData.map(
                            (d) => d.totalQuantity,
                          ),
                          props.recipe.inCupQuantity,
                        ) * 1.1,
                      grid: {
                        display: false,
                      },
                    },
                  },
                  plugins: {
                    tooltip: {
                      enabled: false,
                    },
                    legend: {
                      display: false,
                    },
                  },
                  elements: {
                    line: {
                      borderWidth: 1,
                    },
                  },
                }}
              />
            </Show>
            <Show when={isStarting()}>
              <Spinner class="h-12 w-12" />
            </Show>
          </div>
        </div>
        <div class="flex h-[35%] py-4">
          <div class="flex h-full w-1/2 flex-col gap-0 border-r pb-8 pl-8">
            <div class="flex h-6 w-full items-center justify-center text-sm">
              <T key="time" /> [s]
            </div>
            <div class="flex h-full w-full items-center justify-center text-2xl font-bold">
              {timeSeconds().toFixed(1)}
            </div>
          </div>
          <div class="flex h-full w-1/2 flex-col gap-0 border-r pr-8 pb-8">
            <div class="flex h-6 w-full items-center justify-center text-sm">
              <T key="target-time" /> [s]
            </div>
            <div class="flex h-full w-full items-center justify-center text-2xl font-bold">
              {moment
                .duration(props.recipe.targetExtractionTime)
                .asSeconds()
                .toFixed(1)}
            </div>
          </div>
        </div>
      </div>
      <Show when={!canClose()}>
        <SpinnerButton
          variant="outline"
          class="border-destructive w-36 p-6 text-lg shadow-xl"
          spinnerClass="h-6"
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
          class="border-primary w-36 p-6 text-lg shadow-xl"
          onClick={() => props.onClose()}
        >
          <T key="close" />
        </Button>
      </Show>
    </div>
  );
};
