import { cn } from '../utils/cn';
import type { PolymorphicProps } from '@kobalte/core/polymorphic';
import type {
  SwitchControlProps,
  SwitchRootProps,
  SwitchThumbProps,
} from '@kobalte/core/switch';
import { Switch as SwitchPrimitive } from '@kobalte/core/switch';
import type {
  Accessor,
  ParentProps,
  ValidComponent,
  VoidProps,
} from 'solid-js';
import {
  createContext,
  createEffect,
  createSignal,
  Show,
  splitProps,
  useContext,
} from 'solid-js';
import { Spinner } from './Spinner';

export const SwitchLabel = SwitchPrimitive.Label;
//export const Switch = SwitchPrimitive;
export const SwitchErrorMessage = SwitchPrimitive.ErrorMessage;
export const SwitchDescription = SwitchPrimitive.Description;

type switchlProps<T extends ValidComponent = 'div'> = ParentProps<
  SwitchRootProps<T>
>;

const SwitchLoadingContext = createContext<{ loading: Accessor<boolean> }>();

export const Switch = <T extends ValidComponent = 'input'>(
  props: PolymorphicProps<T, switchlProps<T>>,
) => {
  const [local, rest] = splitProps(props as switchlProps, [
    'checked',
    'onChange',
  ]);
  const [loading, setLoading] = createSignal(false);
  const onChange = (value: boolean) => {
    const cb = local.onChange;
    if (cb) {
      setLoading(true);
      cb(value);
    }
  };
  createEffect(() => {
    if (local.checked) setLoading(false);
    else setLoading(false);
  });
  return (
    <SwitchLoadingContext.Provider value={{ loading }}>
      <SwitchPrimitive checked={local.checked} onChange={onChange} {...rest} />
    </SwitchLoadingContext.Provider>
  );
};

type switchControlProps<T extends ValidComponent = 'input'> = ParentProps<
  SwitchControlProps<T> & { class?: string }
>;

export const SwitchControl = <T extends ValidComponent = 'input'>(
  props: PolymorphicProps<T, switchControlProps<T>>,
) => {
  const [local, rest] = splitProps(props as switchControlProps, [
    'class',
    'children',
  ]);

  return (
    <>
      <SwitchPrimitive.Input class="" />
      <SwitchPrimitive.Control
        class={cn(
          'bg-input data-checked:bg-primary my-1 inline-flex h-5 w-9 shrink-0 cursor-pointer items-center rounded-full border-2 border-transparent shadow-sm transition-[color,background-color,box-shadow] data-disabled:cursor-not-allowed data-disabled:opacity-50',
          local.class,
        )}
        {...rest}
      >
        {local.children}
      </SwitchPrimitive.Control>
    </>
  );
};

type switchThumbProps<T extends ValidComponent = 'div'> = VoidProps<
  SwitchThumbProps<T> & { class?: string }
>;

export const SwitchThumb = <T extends ValidComponent = 'div'>(
  props: PolymorphicProps<T, switchThumbProps<T>>,
) => {
  const [local, rest] = splitProps(props as switchThumbProps, [
    'class',
    'children',
  ]);

  const ctx = useContext(SwitchLoadingContext);

  return (
    <SwitchPrimitive.Thumb
      class={cn(
        'bg-primary data-checked:bg-primary-foreground pointer-events-none block h-4 w-4 translate-x-0 rounded-full shadow-lg ring-0 transition-all data-checked:translate-x-4',
        local.class,
      )}
      {...rest}
    >
      <div class="flex h-full w-full items-center justify-center p-0.5">
        <Show when={ctx?.loading()}>
          <Spinner class="" />
        </Show>
      </div>
    </SwitchPrimitive.Thumb>
  );
};
