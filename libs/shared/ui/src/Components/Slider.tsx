import { cn } from '../utils/cn';
import type {
  SliderFillProps,
  SliderInputProps,
  SliderLabelProps,
  SliderRootProps,
  SliderThumbProps,
  SliderTrackProps,
  SliderValueLabelProps,
} from '@kobalte/core/slider';
import { Slider as SliderPrimitive } from '@kobalte/core/slider';
import type { PolymorphicProps } from '@kobalte/core/polymorphic';
import type { ParentProps, ValidComponent, VoidProps } from 'solid-js';
import {
  createContext,
  createEffect,
  Show,
  splitProps,
  useContext,
} from 'solid-js';

const SliderContext = createContext<{
  orientation: 'vertical' | 'horizontal';
}>();

type sliderRootProps<T extends ValidComponent = 'div'> = ParentProps<
  SliderRootProps<T> & {
    class?: string;
  }
>;

const SliderRoot = <T extends ValidComponent = 'div'>(
  props: PolymorphicProps<T, sliderRootProps<T>>,
) => {
  const [local, rest] = splitProps(props as sliderRootProps, ['class']);
  const ctx = useContext(SliderContext);
  if (!ctx) throw new Error('Slider Context not found!');
  createEffect(() => (ctx.orientation = props.orientation ?? 'horizontal'));
  return (
    <SliderPrimitive
      class={cn(
        'relative flex touch-none flex-col items-center select-none',
        local.class,
      )}
      {...rest}
    />
  );
};

export const Slider = <T extends ValidComponent = 'div'>(
  props: PolymorphicProps<T, sliderRootProps<T>>,
) => {
  return (
    <SliderContext.Provider
      // eslint-disable-next-line solid/reactivity
      value={{ orientation: props.orientation ?? 'horizontal' }}
    >
      <SliderRoot {...props} />
    </SliderContext.Provider>
  );
};

type sliderTrackProps<T extends ValidComponent = 'div'> = ParentProps<
  SliderTrackProps<T> & {
    class?: string;
  }
>;

export const SliderTrack = <T extends ValidComponent = 'div'>(
  props: PolymorphicProps<T, sliderTrackProps<T>>,
) => {
  const [local, rest] = splitProps(props as sliderTrackProps, ['class']);
  const ctx = useContext(SliderContext);
  if (!ctx) throw new Error('Slider Context not found!');
  return (
    <SliderPrimitive.Track
      class={cn(
        'bg-border relative rounded-full',
        ctx.orientation === 'horizontal' ? 'h-2 w-full' : 'h-full w-2',
        local.class,
      )}
      {...rest}
    />
  );
};

type sliderLabelProps<T extends ValidComponent = 'div'> = VoidProps<
  SliderLabelProps<T> & {
    class?: string;
  }
>;

export const SliderLabel = <T extends ValidComponent = 'div'>(
  props: PolymorphicProps<T, sliderLabelProps<T>>,
) => {
  const [local, rest] = splitProps(props as sliderLabelProps, ['class']);
  return (
    <SliderPrimitive.Label
      class={cn('flex w-full justify-between', local.class)}
      {...rest}
    />
  );
};

type sliderValueLabelProps<T extends ValidComponent = 'div'> = VoidProps<
  SliderValueLabelProps<T> & {
    class?: string;
  }
>;

export const SliderValueLabel = <T extends ValidComponent = 'div'>(
  props: PolymorphicProps<T, sliderValueLabelProps<T>>,
) => {
  const [local, rest] = splitProps(props as sliderValueLabelProps, ['class']);
  return <SliderPrimitive.ValueLabel class={cn('', local.class)} {...rest} />;
};

type sliderFillProps<T extends ValidComponent = 'div'> = VoidProps<
  SliderFillProps<T> & {
    class?: string;
  }
>;

export const SliderFill = <T extends ValidComponent = 'div'>(
  props: PolymorphicProps<T, sliderFillProps<T>>,
) => {
  const [local, rest] = splitProps(props as sliderFillProps, ['class']);
  const ctx = useContext(SliderContext);
  if (!ctx) throw new Error('Slider Context not found!');
  return (
    <>
      <Show when={ctx.orientation === 'horizontal'}>
        <SliderPrimitive.Fill
          class={cn('absolute h-full rounded-full', local.class)}
          {...rest}
        />
      </Show>
      <Show when={ctx.orientation === 'vertical'}>
        <SliderPrimitive.Fill
          class={cn('absolute w-full rounded-full', local.class)}
          {...rest}
        />
      </Show>
    </>
  );
};

type sliderThumbProps<T extends ValidComponent = 'span'> = ParentProps<
  SliderThumbProps<T> & {
    class?: string;
  }
>;

export const SliderThumb = <T extends ValidComponent = 'span'>(
  props: PolymorphicProps<T, sliderThumbProps<T>>,
) => {
  const [local, rest] = splitProps(props as sliderThumbProps, ['class']);
  const ctx = useContext(SliderContext);
  if (!ctx) throw new Error('Slider Context not found!');
  return (
    <>
      <Show when={ctx.orientation === 'horizontal'}>
        <SliderPrimitive.Thumb
          class={cn(
            'focus:shadown-md -top-1 block h-4 w-4 rounded-full',
            local.class,
          )}
          {...rest}
        />
      </Show>
      <Show when={ctx.orientation === 'vertical'}>
        <SliderPrimitive.Thumb
          class={cn(
            'focus:shadown-md -left-1 block h-4 w-4 rounded-full',
            local.class,
          )}
          {...rest}
        />
      </Show>
    </>
  );
};

type sliderInputProps = VoidProps<
  SliderInputProps & {
    class?: string;
  }
>;

export const SliderInput = <T extends ValidComponent = 'input'>(
  props: PolymorphicProps<T, sliderInputProps>,
) => {
  const [local, rest] = splitProps(props as sliderInputProps, ['class']);
  return <SliderPrimitive.Input class={cn('', local.class)} {...rest} />;
};
