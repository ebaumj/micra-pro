import { Component, createEffect, onMount } from 'solid-js';
import { Gauge as GaugeClass, GaugeOptions } from 'gaugeJS';
import { twColor } from '../utils/cssvar';
import { deepMerge, DeepPartial } from '@micra-pro/shared/utils-ts';
import { trackDeep, trackStore } from '@solid-primitives/deep';
import { createStore } from 'solid-js/store';

export const Gauge: Component<{
  value: number;
  maxValue?: number;
  minValue?: number;
  animationSpeed?: number;
  options?: DeepPartial<GaugeOptions>;
  class?: string;
}> = (props) => {
  var canvas!: HTMLCanvasElement;

  const [defaults] = createStore({
    options: {
      angle: 0.15,
      lineWidth: 0.4,
      radiusScale: 1,
      pointer: {
        length: 0.6,
        strokeWidth: 0.03,
        color: twColor('foreground'),
      },
      colorStart: twColor('primary'),
      strokeColor: twColor('secondary'),
    } as GaugeOptions,
    animationSpeed: 32,
    minValue: 0,
    maxValue: 100,
  });

  onMount(() => {
    const gauge = new GaugeClass(canvas);
    createEffect(() => (gauge.maxValue = props.maxValue ?? defaults.maxValue));
    createEffect(() => gauge.setMinValue(props.minValue ?? defaults.minValue));
    createEffect(
      () =>
        (gauge.animationSpeed =
          props.animationSpeed ?? defaults.animationSpeed),
    );
    createEffect(() => gauge.set(props.value));
    createEffect(() =>
      gauge.setOptions(
        props.options
          ? deepMerge(trackStore(defaults.options), trackDeep(props.options))
          : defaults.options,
      ),
    );
  });

  return <canvas ref={canvas} class={props.class} />;
};
