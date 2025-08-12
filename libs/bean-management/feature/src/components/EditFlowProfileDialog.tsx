import { FlowProfileProperties } from '@micra-pro/bean-management/data-access';
import {
  Dialog,
  DialogContent,
  Icon,
  LineChart,
  LongPressDiv,
  Slider,
  SliderInput,
  SliderThumb,
  SliderTrack,
  SpinnerButton,
  twColor,
} from '@micra-pro/shared/ui';
import moment from 'moment';
import {
  Accessor,
  Component,
  createEffect,
  createSignal,
  For,
  Show,
} from 'solid-js';
import { createStore } from 'solid-js/store';
import { T } from '../generated/language-types';
import { twMerge } from 'tailwind-merge';

const maxFlow = 5;

const defaults = (): FlowProfileProperties =>
  JSON.parse(
    JSON.stringify({
      flowSettings: [],
      startFlow: 1,
    }),
  );

const createPoint = (flow: number, time: number) =>
  JSON.parse(
    JSON.stringify({
      flow: flow,
      time: moment.duration(time, 'seconds').toISOString(),
    }),
  );

export type EditFlowProfileDialogContent = {
  properties?: FlowProfileProperties;
  targetExtractionTime: any;
  onSave: (properties: FlowProfileProperties) => void;
  onRemove?: () => void;
  isSaving?: Accessor<boolean>;
  isRemoving?: Accessor<boolean>;
};

export const EditFlowProfilingDialog: Component<{
  content: EditFlowProfileDialogContent | undefined;
  onClose: () => void;
}> = (props) => {
  let timer: ReturnType<typeof setTimeout> | null = null;
  const [store, setStore] = createStore({ properties: defaults() });

  createEffect(() => {
    const newProperties = props.content?.properties;
    if (newProperties) setStore('properties', newProperties);
    else setStore('properties', defaults());
  });

  const extractionTimeInSeconds = (): number =>
    props.content
      ? moment.duration(props.content.targetExtractionTime).asSeconds()
      : 1;
  const lastFlowSetting = (): number =>
    store.properties.flowSettings.length > 0
      ? store.properties.flowSettings[store.properties.flowSettings.length - 1]
          .flow
      : store.properties.startFlow;

  const asSeconds = (time: any) => moment.duration(time).asSeconds();

  const maxTime = () =>
    Math.max(
      extractionTimeInSeconds(),
      ...store.properties.flowSettings.map((s) => asSeconds(s.time)),
    );

  const chartPoints = (): { flow: number; time: number }[] => [
    { flow: store.properties.startFlow, time: 0 },
    ...store.properties.flowSettings.map((s) => ({
      flow: s.flow,
      time: asSeconds(s.time),
    })),
    { flow: lastFlowSetting(), time: maxTime() },
  ];

  const setTime = (time: number, index: number) =>
    setStore(
      'properties',
      'flowSettings',
      index,
      'time',
      moment.duration(time, 'seconds').toISOString(),
    );

  const [selectedPoint, setSelectedPoint] = createSignal(-1);
  const [deleting, setDeleting] = createSignal(false);

  const currentFlow = () => {
    const point = selectedPoint();
    return point === -1
      ? store.properties.startFlow
      : store.properties.flowSettings[point].flow;
  };

  const updateCurrentFlow = (flow: number) => {
    const point = selectedPoint();
    if (point === -1) setStore('properties', 'startFlow', flow);
    else setStore('properties', 'flowSettings', point, 'flow', flow);
  };

  const addPoint = () => {
    const length = store.properties.flowSettings.length;
    setStore('properties', 'flowSettings', length, createPoint(0, maxTime()));
    setSelectedPoint(length);
  };

  const removePoint = () => {
    const point = selectedPoint();
    setSelectedPoint(-1);
    setStore(
      'properties',
      'flowSettings',
      store.properties.flowSettings.filter((_, i) => i !== point),
    );
    setDeleting(false);
  };

  return (
    <Dialog
      open={!!props.content}
      onOpenChange={(o) => (o ? undefined : props.onClose())}
    >
      <DialogContent
        onOpenAutoFocus={(e) => e.preventDefault()}
        onInteractOutside={(e) => e.preventDefault()}
        class="min-w-[90%]"
      >
        <div class="flex flex-col gap-2 pr-6 pl-2">
          <div class="flex w-full text-lg">
            <div class="w-full pl-4 font-bold">Flow Profile</div>
            <div class="text-base whitespace-nowrap">
              Target Extraction Time: {extractionTimeInSeconds()} s
            </div>
          </div>
          <div class="flex h-72 w-full">
            <div class="flex w-3/4">
              <div class="w-4 py-1.5">
                <Slider
                  orientation="vertical"
                  minValue={0}
                  maxValue={maxFlow}
                  step={0.1}
                  value={[currentFlow()]}
                  onChange={(flows: number[]) => updateCurrentFlow(flows[0])}
                  class="h-full w-full"
                >
                  <SliderTrack class="bg-transparent">
                    <SliderThumb class="-left-3 bg-transparent">
                      <div class="-mt-1">
                        <Icon iconName="navigation" class="rotate-90" />
                      </div>
                      <SliderInput />
                    </SliderThumb>
                  </SliderTrack>
                </Slider>
              </div>
              <div class="bg-primary w-full rounded-md">
                <Chart
                  data={chartPoints()}
                  maxFlow={maxFlow}
                  maxTime={maxTime()}
                />
                <div class="flex h-4 w-full items-center px-1.5">
                  <Slider
                    minValue={0}
                    maxValue={maxTime()}
                    step={0.5}
                    value={store.properties.flowSettings.map((s) =>
                      asSeconds(s.time),
                    )}
                    onChange={(times: number[]) => times.forEach(setTime)}
                    class="w-full"
                  >
                    <SliderTrack class="bg-transparent">
                      <For each={store.properties.flowSettings}>
                        {(_, idx) => (
                          <SliderThumb
                            class={twMerge(
                              'bg-transparent',
                              selectedPoint() !== idx() ? 'opacity-20' : '',
                            )}
                            onPointerDown={() => setSelectedPoint(idx())}
                          >
                            <Icon iconName="navigation" class="-ml-1" />
                            <SliderInput />
                          </SliderThumb>
                        )}
                      </For>
                    </SliderTrack>
                  </Slider>
                </div>
              </div>
              <div class="w-4" />
            </div>
            <div class="no-scrollbar w-1/4 overflow-y-scroll rounded-lg border shadow-lg">
              <div
                class={twMerge(
                  'flex w-full border-b px-2 py-1 text-sm',
                  selectedPoint() === -1 ? 'rounded-lg' : 'opacity-20',
                )}
                onClick={() => setSelectedPoint(-1)}
              >
                <div class="flex w-1/2 justify-center">Start</div>
                <div class="flex w-1/2 justify-center">
                  {store.properties.startFlow} ml/s
                </div>
              </div>
              <For each={store.properties.flowSettings}>
                {(setting, idx) => (
                  <LongPressDiv
                    class={twMerge(
                      'flex w-full border-b px-2 py-1 text-sm',
                      selectedPoint() === idx() ? 'rounded-lg' : 'opacity-20',
                    )}
                    onLongPress={removePoint}
                    onPressStart={() => {
                      setSelectedPoint(idx());
                      timer = setTimeout(() => setDeleting(true), 300);
                    }}
                    onPressEnd={() => {
                      setDeleting(false);
                      if (timer) {
                        clearTimeout(timer);
                        timer = null;
                      }
                    }}
                    delayTimeMs={1300}
                  >
                    <div class="flex w-1/2 justify-center">
                      {asSeconds(setting.time)} s
                    </div>
                    <div class="flex w-1/2 justify-center">
                      {setting.flow} ml/s
                    </div>
                  </LongPressDiv>
                )}
              </For>
              <div class="flex w-full items-center justify-center p-2">
                <div
                  class="rounded-lg border px-3 py-0.5 text-sm shadow-sm"
                  onClick={addPoint}
                >
                  <Icon iconName="add" />
                </div>
              </div>
            </div>
          </div>
          <div class="flex w-full pt-8">
            <div class="flex h-full w-20 items-center justify-start pl-4">
              <Show when={deleting()}>
                <Icon iconName="delete" class="text-destructive" />
                <div class="animate-spin-loader-1s bg-destructive h-6 w-6 rotate-45 rounded-full" />
              </Show>
            </div>
            <div class="flex w-full justify-end gap-2">
              <Show when={props.content?.onRemove}>
                <SpinnerButton
                  class="h-10 w-28"
                  variant="destructive"
                  onClick={() => props.content?.onRemove?.()}
                  loading={props.content?.isRemoving?.() ?? false}
                >
                  <T key="remove" />
                </SpinnerButton>
              </Show>
              <SpinnerButton
                class="h-10 w-28"
                variant="default"
                onClick={() => props.content?.onSave(store.properties)}
                loading={props.content?.isSaving?.() ?? false}
              >
                <T key="save" />
              </SpinnerButton>
            </div>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};

const Chart: Component<{
  data: { flow: number; time: number }[];
  maxTime: number;
  maxFlow: number;
}> = (props) => (
  <LineChart
    data={{
      labels: props.data.map((p) => p.time),
      datasets: [
        {
          data: props.data.map((d) => d.flow),
          pointStyle: false,
          animation: false,
          borderColor: twColor('accent'),
        },
      ],
    }}
    options={{
      maintainAspectRatio: false,
      scales: {
        x: {
          display: false,
          min: 0,
          max: props.maxTime,
          grid: {
            display: false,
          },
          type: 'linear',
        },
        y: {
          display: false,
          min: 0,
          max: props.maxFlow,
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
);
