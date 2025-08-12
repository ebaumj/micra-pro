import { Gauge, Spinner, twColor } from '@micra-pro/shared/ui';
import { Component, Show } from 'solid-js';
import { T } from '../generated/language-types';
import { twMerge } from 'tailwind-merge';

const WeightTolerance = 1;
const WeightOverhead = 4 * WeightTolerance;
const MaxFlow = 5;

export const PannelGauge: Component<{
  flow: number;
  quantity: number;
  isStarting: boolean;
  time: number;
  targetTime: number;
  targetQuantity: number;
  flowInside: number;
  useFlowProfiling: boolean;
}> = (props) => {
  const flow = () => (props.useFlowProfiling ? props.flowInside : props.flow);
  return (
    <div class="relative flex h-full w-full flex-col items-center justify-center">
      <div class="absolute flex h-full w-full">
        <div class="flex w-1/3 flex-col items-center justify-center px-2">
          <Show when={!props.isStarting}>
            <div class="inset-shadow-primary-shadow w-full rounded-lg inset-shadow-sm">
              <div class="flex w-full items-center justify-center text-2xl font-bold">
                {props.quantity.toFixed(1)}
              </div>
              <div class="flex w-full items-center justify-center text-sm">
                <T key="liquid" /> [ml]
              </div>
              <div class="flex w-full items-center justify-center text-2xl">
                {props.targetQuantity.toFixed(1)}
              </div>
            </div>
          </Show>
        </div>
        <div class="flex w-1/3 items-end">
          <div
            class={twMerge(
              'w-full justify-center',
              props.useFlowProfiling ? 'pb-10' : 'pb-14',
            )}
          >
            <Show when={!props.isStarting}>
              <div class="inset-shadow-primary-shadow w-full rounded-lg py-1 inset-shadow-sm">
                <Show when={props.useFlowProfiling}>
                  <div class="flex w-full items-center justify-center text-xs">
                    <T key="inside" />
                  </div>
                  <div class="flex w-full items-center justify-center text-2xl font-bold">
                    {props.flowInside.toFixed(1)}
                  </div>
                  <div class="flex w-full items-center justify-center text-xs">
                    <T key="outside" />
                  </div>
                </Show>
                <div class="flex w-full items-center justify-center text-2xl font-bold">
                  {props.flow.toFixed(1)}
                </div>
                <div class="flex w-full items-center justify-center text-sm">
                  <T key="flow" /> [ml/s]
                </div>
              </div>
            </Show>
            <Show when={props.isStarting}>
              <Spinner class="flex h-12 w-full justify-center" />
            </Show>
          </div>
        </div>
        <div class="flex w-1/3 flex-col items-center justify-center px-2">
          <Show when={!props.isStarting}>
            <div class="inset-shadow-primary-shadow w-full rounded-lg inset-shadow-sm">
              <div class="flex w-full items-center justify-center text-2xl font-bold">
                {props.time.toFixed(1)}
              </div>
              <div class="flex w-full items-center justify-center text-sm">
                <T key="time" /> [s]
              </div>
              <div class="flex w-full items-center justify-center text-2xl">
                {props.targetTime.toFixed(1)}
              </div>
            </div>
          </Show>
        </div>
      </div>
      <div class="flex h-full w-full flex-col">
        <Gauge
          animationSpeed={127}
          maxValue={props.targetQuantity + WeightOverhead}
          minValue={0}
          options={{
            lineWidth: 0.25,
            pointer: {
              color: twColor('accent'),
            },
            staticZones: [
              {
                strokeStyle: twColor('positive'),
                min: props.targetQuantity - WeightTolerance,
                max: props.targetQuantity + WeightTolerance,
              },
              {
                strokeStyle: twColor('primary-foreground'),
                min: 0,
                max: props.targetQuantity - WeightTolerance,
              },
              {
                strokeStyle: twColor('primary-foreground'),
                min: props.targetQuantity + WeightTolerance,
                max: props.targetQuantity + WeightOverhead,
              },
            ],
          }}
          value={props.quantity}
        />
        <Gauge
          class="scale-y-[-1]"
          animationSpeed={127}
          maxValue={MaxFlow}
          minValue={0}
          options={{
            lineWidth: 0.1,
            limitMax: true,
            limitMin: true,
            staticZones: [
              {
                strokeStyle: twColor('accent'),
                min: 0,
                max: flow(),
                height: 1.0,
              },
              {
                strokeStyle: twColor('accent'),
                min: flow(),
                max: flow() + 0.05,
                height: 1.4,
              },
              {
                strokeStyle: twColor('primary-foreground'),
                min: flow() + 0.05,
                max: MaxFlow,
                height: 1.0,
              },
            ],
            pointer: {
              strokeWidth: 0.0,
              color: twColor('accent'),
              length: 0.1,
            },
          }}
          value={props.flow}
        />
      </div>
    </div>
  );
};
