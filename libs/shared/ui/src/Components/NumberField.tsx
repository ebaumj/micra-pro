import { cn } from '../utils/cn';
import type {
  NumberFieldDecrementTriggerProps,
  NumberFieldDescriptionProps,
  NumberFieldErrorMessageProps,
  NumberFieldIncrementTriggerProps,
  NumberFieldInputProps,
  NumberFieldLabelProps,
  NumberFieldRootProps,
} from '@kobalte/core/number-field';
import { NumberField as NumberFieldPrimitive } from '@kobalte/core/number-field';
import type { PolymorphicProps } from '@kobalte/core/polymorphic';
import type { ComponentProps, ValidComponent, VoidProps } from 'solid-js';
import { splitProps } from 'solid-js';
import { textfieldLabel } from './TextField';
import {
  NumberPicker,
  NumberPickerContextProvider,
  useNumberPickerContext,
} from './NumberPicker/NumberPicker';
import { useFormControlContext } from '@kobalte/core';

export const NumberFieldHiddenInput = NumberFieldPrimitive.HiddenInput;

type numberFieldLabelProps<T extends ValidComponent = 'div'> =
  NumberFieldLabelProps<T> & {
    class?: string;
  };

export const NumberFieldLabel = <T extends ValidComponent = 'div'>(
  props: PolymorphicProps<T, numberFieldLabelProps<T>>,
) => {
  const [local, rest] = splitProps(props as numberFieldLabelProps, ['class']);

  return (
    <NumberFieldPrimitive.Label
      class={cn(textfieldLabel({ label: true }), local.class)}
      {...rest}
    />
  );
};

type numberFieldDescriptionProps<T extends ValidComponent = 'div'> =
  NumberFieldDescriptionProps<T> & {
    class?: string;
  };

export const NumberFieldDescription = <T extends ValidComponent = 'div'>(
  props: PolymorphicProps<T, numberFieldDescriptionProps<T>>,
) => {
  const [local, rest] = splitProps(props as numberFieldDescriptionProps, [
    'class',
  ]);

  return (
    <NumberFieldPrimitive.Description
      class={cn(
        textfieldLabel({ description: true, label: false }),
        local.class,
      )}
      {...rest}
    />
  );
};

type numberFieldErrorMessageProps<T extends ValidComponent = 'div'> =
  NumberFieldErrorMessageProps<T> & {
    class?: string;
  };

export const NumberFieldErrorMessage = <T extends ValidComponent = 'div'>(
  props: PolymorphicProps<T, numberFieldErrorMessageProps<T>>,
) => {
  const [local, rest] = splitProps(props as numberFieldErrorMessageProps, [
    'class',
  ]);

  return (
    <NumberFieldPrimitive.ErrorMessage
      class={cn(textfieldLabel({ error: true }), local.class)}
      {...rest}
    />
  );
};

type numberFieldProps<T extends ValidComponent = 'div'> =
  NumberFieldRootProps<T> & {
    class?: string;
  };

export const NumberField = <T extends ValidComponent = 'div'>(
  props: PolymorphicProps<T, numberFieldProps<T>>,
) => {
  const [local, rest] = splitProps(props as numberFieldProps, [
    'class',
    'rawValue',
    'onRawValueChange',
    'minValue',
    'maxValue',
    'step',
  ]);
  return (
    <NumberPickerContextProvider>
      <NumberFieldPrimitive
        class={cn('grid gap-1.5', local.class)}
        {...rest}
        rawValue={local.rawValue}
        minValue={local.minValue}
        maxValue={local.maxValue}
        step={local.step}
        onRawValueChange={local.onRawValueChange}
      />
      <NumberPicker
        value={local.rawValue ?? 0}
        onSetValue={(v) => local.onRawValueChange?.(v)}
        min={local.minValue}
        max={local.maxValue}
        step={local.step}
      />
    </NumberPickerContextProvider>
  );
};

export const NumberFieldGroup = (props: ComponentProps<'div'>) => {
  const [local, rest] = splitProps(props, ['class', 'children']);
  let element!: HTMLDivElement;
  const formControlContext = useFormControlContext();
  const numberPickerContext = useNumberPickerContext();

  const openNumberPad = () => {
    if (formControlContext.dataset()['data-disabled'] === undefined) {
      element.focus();
      numberPickerContext.setOpen(true);
    }
  };

  return (
    <div
      ref={element}
      class={cn('relative rounded-md transition-shadow', local.class)}
      {...rest}
    >
      {local.children}
      <div
        onClick={openNumberPad}
        class="absolute top-0 right-10 left-10 h-full bg-none"
      />
    </div>
  );
};

type numberFieldInputProps<T extends ValidComponent = 'input'> =
  NumberFieldInputProps<T> & {
    class?: string;
  };

export const NumberFieldInput = <T extends ValidComponent = 'input'>(
  props: PolymorphicProps<T, VoidProps<numberFieldInputProps<T>>>,
) => {
  const [local, rest] = splitProps(props as numberFieldInputProps, ['class']);

  return (
    <NumberFieldPrimitive.Input
      class={cn(
        'border-border placeholder:text-muted-foreground flex h-9 w-full rounded-md border bg-transparent px-10 py-1 text-center text-sm shadow-xs focus-visible:outline-hidden disabled:cursor-not-allowed disabled:opacity-50',
        local.class,
      )}
      {...rest}
    />
  );
};

type numberFieldDecrementTriggerProps<T extends ValidComponent = 'button'> =
  VoidProps<
    NumberFieldDecrementTriggerProps<T> & {
      class?: string;
    }
  >;

export const NumberFieldDecrementTrigger = <
  T extends ValidComponent = 'button',
>(
  props: PolymorphicProps<T, VoidProps<numberFieldDecrementTriggerProps<T>>>,
) => {
  const [local, rest] = splitProps(props as numberFieldDecrementTriggerProps, [
    'class',
  ]);

  return (
    <NumberFieldPrimitive.DecrementTrigger
      class={cn(
        'absolute top-1/2 left-0 -translate-y-1/2 p-3 disabled:cursor-not-allowed disabled:opacity-20',
        local.class,
      )}
      {...rest}
    >
      <svg
        xmlns="http://www.w3.org/2000/svg"
        class="size-4"
        viewBox="0 0 24 24"
      >
        <path
          fill="none"
          stroke="currentColor"
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M5 12h14"
        />
        <title>Decreasing number</title>
      </svg>
    </NumberFieldPrimitive.DecrementTrigger>
  );
};

type numberFieldIncrementTriggerProps<T extends ValidComponent = 'button'> =
  VoidProps<
    NumberFieldIncrementTriggerProps<T> & {
      class?: string;
    }
  >;

export const NumberFieldIncrementTrigger = <
  T extends ValidComponent = 'button',
>(
  props: PolymorphicProps<T, numberFieldIncrementTriggerProps<T>>,
) => {
  const [local, rest] = splitProps(props as numberFieldIncrementTriggerProps, [
    'class',
  ]);

  return (
    <NumberFieldPrimitive.IncrementTrigger
      class={cn(
        'absolute top-1/2 right-0 -translate-y-1/2 p-3 disabled:cursor-not-allowed disabled:opacity-20',
        local.class,
      )}
      {...rest}
    >
      <svg
        xmlns="http://www.w3.org/2000/svg"
        class="size-4"
        viewBox="0 0 24 24"
      >
        <path
          fill="none"
          stroke="currentColor"
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M12 5v14m-7-7h14"
        />
        <title>Increase number</title>
      </svg>
    </NumberFieldPrimitive.IncrementTrigger>
  );
};
