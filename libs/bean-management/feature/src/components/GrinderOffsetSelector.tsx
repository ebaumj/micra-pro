import { getGrinderSettingsAccessor } from '@micra-pro/bean-management/data-access';
import {
  NumberField,
  NumberFieldDecrementTrigger,
  NumberFieldGroup,
  NumberFieldIncrementTrigger,
  NumberFieldInput,
} from '@micra-pro/shared/ui';
import { Component } from 'solid-js';
import { twMerge } from 'tailwind-merge';

export const GrinderOffsetSelector: Component<{
  class?: string;
  onChanged?: () => void;
}> = (props) => {
  // eslint-disable-next-line solid/reactivity
  const accessor = getGrinderSettingsAccessor(props.onChanged);
  return (
    <div class={twMerge(props.class, 'flex items-center justify-center p-2')}>
      <NumberField
        onFocusIn={(e) => e.preventDefault()}
        onRawValueChange={(v) => {
          accessor.offset.setValue(v);
        }}
        rawValue={accessor.offset.value()}
        formatOptions={{ style: 'decimal' }}
        minValue={-50}
        maxValue={50}
        step={0.5}
      >
        <NumberFieldGroup class="flex items-center justify-center">
          <NumberFieldDecrementTrigger aria-label="Decrement" />
          <NumberFieldInput />
          <NumberFieldIncrementTrigger aria-label="Increment" />
        </NumberFieldGroup>
      </NumberField>
    </div>
  );
};
