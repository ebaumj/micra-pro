import { LineChart, Spinner, twColor } from '@micra-pro/shared/ui';
import { Component, Show } from 'solid-js';
import { T, useTranslationContext } from '../generated/language-types';
import moment from 'moment';

export const PannelGraph: Component<{
  flow: number;
  quantity: number;
  isStarting: boolean;
  time: number;
  targetTime: number;
  targetQuantity: number;
  useFlowProfiling: boolean;
  brewData: {
    flowOutside: number;
    flowInside: number;
    quantity: number;
    time: number;
  }[];
}> = (props) => {
  const { t } = useTranslationContext();
  return (
    <>
      <div class="flex h-[35%] py-4">
        <div class="flex h-full w-1/2 flex-col gap-0 border-r pt-8 pl-8">
          <div class="flex h-full w-full items-center justify-center text-2xl font-bold">
            {props.flow.toFixed(1)}
          </div>
          <div class="flex h-6 w-full items-center justify-center text-sm">
            <T key="flow" /> [ml/s]
          </div>
        </div>
        <div class="flex h-full w-1/2 flex-col gap-0 border-r pt-8 pr-8">
          <div class="flex h-full w-full items-center justify-center text-2xl font-bold">
            {props.quantity.toFixed(1)}
          </div>
          <div class="flex h-6 w-full items-center justify-center text-sm">
            <T key="liquid" /> [ml]
          </div>
        </div>
      </div>
      <div class="h-[30%] px-10 py-0">
        <div class="flex h-full w-full items-center justify-center">
          <Show when={!props.isStarting}>
            <LineChart
              data={{
                labels: props.brewData.map((d) =>
                  moment.duration(d.time).asSeconds(),
                ),
                datasets: [
                  {
                    data: props.brewData.map((d) => d.flowOutside),
                    label: t('flow'),
                    pointStyle: false,
                    animation: false,
                    borderColor: twColor('accent'),
                  },
                  {
                    data: props.useFlowProfiling
                      ? props.brewData.map((d) => d.flowInside)
                      : [],
                    label: t('flow'),
                    pointStyle: false,
                    animation: false,
                    borderColor: twColor('accent-alt'),
                  },
                  {
                    data: props.brewData.map((d) => d.quantity),
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
                        ...props.brewData.map((d) => {
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
                          if (d.flowInside > d.flowOutside) return d.flowInside;
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
                        ...props.brewData.map((d) => d.quantity),
                        props.targetQuantity,
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
          <Show when={props.isStarting}>
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
            {props.time.toFixed(1)}
          </div>
        </div>
        <div class="flex h-full w-1/2 flex-col gap-0 border-r pr-8 pb-8">
          <div class="flex h-6 w-full items-center justify-center text-sm">
            <T key="target-time" /> [s]
          </div>
          <div class="flex h-full w-full items-center justify-center text-2xl font-bold">
            {props.targetTime.toFixed(1)}
          </div>
        </div>
      </div>
    </>
  );
};
