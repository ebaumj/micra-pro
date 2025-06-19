import { Component, createEffect, createSignal, Show } from 'solid-js';
import {
  Button,
  handleError,
  LineChart,
  Spinner,
  SpinnerButton,
} from '@micra-pro/shared/ui';
import { Spout, StartCoffee } from '@micra-pro/brew-by-weight/data-access';
import { T, useTranslationContext } from '../generated/language-types';
import moment from 'moment';
import { twMerge } from 'tailwind-merge';

export const BrewByWeightContent: Component<{
  recipe: {
    beanId: string;
    coffeeQuantity: number;
    grindSetting: number;
    inCupQuantity: number;
    scaleId: string;
    targetExtractionTime: string;
    spout: Spout;
  };
  onClose: () => void;
}> = (props) => {
  const { t } = useTranslationContext();
  const [isStopping, setIsStopping] = createSignal(false);
  // eslint-disable-next-line solid/reactivity
  const accessor = StartCoffee(props.recipe);
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
  return (
    <div class="flex w-full flex-col items-center justify-center gap-6">
      <div
        class={twMerge(
          'flex h-80 w-80 flex-col overflow-hidden rounded-full bg-slate-600 text-white shadow-lg',
          successClass() === true
            ? 'shadow-green-600'
            : successClass() === false
              ? 'shadow-destructive'
              : 'shadow-slate-400',
        )}
      >
        <div class="flex h-[35%] py-4">
          <div class="flex h-full w-1/2 flex-col gap-0 border-r border-slate-500 pl-8 pt-8">
            <div class="flex h-full w-full items-center justify-center text-2xl font-bold">
              {flow().toFixed(1)}
            </div>
            <div class="flex h-6 w-full items-center justify-center text-sm">
              <T key="flow" /> [ml/s]
            </div>
          </div>
          <div class="flex h-full w-1/2 flex-col gap-0 border-r pr-8 pt-8">
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
                  labels: accessor.dataStore.brewData.map((d) =>
                    moment.duration(d.totalTime).asSeconds(),
                  ),
                  datasets: [
                    {
                      data: accessor.dataStore.brewData.map((d) => d.flow),
                      label: t('flow'),
                      pointStyle: false,
                      animation: false,
                      borderColor: '#FFFFFF',
                    },
                    {
                      data: accessor.dataStore.brewData.map(
                        (d) => d.totalQuantity,
                      ),
                      label: t('liquid'),
                      pointStyle: false,
                      animation: false,
                      borderColor: '#00FFFF',
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
                        color: '#FFFFFF',
                      },
                      min: 0,
                      max:
                        Math.max(
                          ...accessor.dataStore.brewData.map((d) => d.flow),
                        ) * 1.1,
                      grid: {
                        display: false,
                      },
                    },
                    y2: {
                      axis: 'y',
                      display: true,
                      ticks: {
                        color: '#00FFFF',
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
              <Spinner class="h-12 w-12 fill-slate-300" />
            </Show>
          </div>
        </div>
        <div class="flex h-[35%] py-4">
          <div class="flex h-full w-1/2 flex-col gap-0 border-r border-slate-500 pb-8 pl-8">
            <div class="flex h-6 w-full items-center justify-center text-sm">
              <T key="time" /> [s]
            </div>
            <div class="flex h-full w-full items-center justify-center text-2xl font-bold">
              {timeSeconds().toFixed(1)}
            </div>
          </div>
          <div class="flex h-full w-1/2 flex-col gap-0 border-r pb-8 pr-8">
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
          class="w-36 border-destructive p-6 text-lg text-destructive shadow-xl"
          spinnerClass="h-6"
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
          class="w-36 border-slate-600 p-6 text-lg shadow-xl"
          onClick={() => props.onClose()}
        >
          <T key="close" />
        </Button>
      </Show>
    </div>
  );
};
