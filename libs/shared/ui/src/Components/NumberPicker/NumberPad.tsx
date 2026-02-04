import { Component, onMount, JSX, splitProps } from 'solid-js';
import { Button } from '../Button';
import { Icon } from '../Icon';
import { cn } from '../../utils/cn';

export const NumberPad: Component<{
  value?: number;
  onSetValue: (value: number) => void;
  close: () => void;
}> = (props) => {
  let element!: HTMLInputElement;
  onMount(() => (element.value = `${props.value ?? 0}`));
  const getValue = (): number => {
    if (element.value === '') return 0;
    return Number(element.value);
  };
  const confirm = () => {
    const value = getValue();
    if (Number.isNaN(value)) props.close();
    else props.onSetValue(value);
  };
  return (
    <div class="px-6 pt-2">
      <input
        ref={element}
        class="bg-secondary h-12 w-full rounded-lg border p-2 text-right text-xl inset-shadow-xs"
        disabled
      />
      <div class="flex h-16 w-full gap-2 pt-4">
        <NumberKey value="1" refInput={element} />
        <NumberKey value="2" refInput={element} />
        <NumberKey value="3" refInput={element} />
      </div>
      <div class="flex h-16 w-full gap-2 pt-2">
        <NumberKey value="4" refInput={element} />
        <NumberKey value="5" refInput={element} />
        <NumberKey value="6" refInput={element} />
      </div>
      <div class="flex h-16 w-full gap-2 pt-2">
        <NumberKey value="7" refInput={element} />
        <NumberKey value="8" refInput={element} />
        <NumberKey value="9" refInput={element} />
      </div>
      <div class="flex h-16 w-full gap-2 pt-2">
        <NumberKey value="." refInput={element} />
        <NumberKey value="0" refInput={element} />
        <BackspaceKey refInput={element} />
      </div>
      <div class="flex h-14 w-full justify-center pt-4">
        <Button class="h-full w-1/2" onClick={confirm}>
          OK
        </Button>
      </div>
    </div>
  );
};

const NumberKey: Component<
  JSX.ButtonHTMLAttributes<HTMLButtonElement> & {
    refInput?: HTMLInputElement;
    value: string;
  }
> = (props) => {
  const [local, rest] = splitProps(props, ['class']);

  const InsertChar = () => {
    const input = props.refInput;
    if (!input) return;

    input.setRangeText(
      props.value,
      input.selectionStart || 0,
      input.selectionEnd || 0,
      'end',
    );
    input.scrollLeft = input.scrollWidth;
    input.dispatchEvent(new InputEvent('input', { bubbles: true }));
  };

  return (
    <button
      class={cn(
        'border-border bg-background active:bg-secondary grow basis-4 rounded-md border inset-shadow-xs',
        local.class,
      )}
      onClick={InsertChar}
      {...rest}
    >
      {props.value}
    </button>
  );
};

export const BackspaceKey: Component<
  JSX.ButtonHTMLAttributes<HTMLButtonElement> & {
    refInput?: HTMLInputElement;
  }
> = (props) => {
  const [local, rest] = splitProps(props, ['class']);

  const trigger = () => {
    const input = props.refInput;
    if (!input) return;

    const start = input.selectionStart || 0;
    const end = input.selectionEnd || 0;

    if (start === end && start > 0) {
      input.setRangeText('', start - 1, end, 'end');
    } else {
      input.setRangeText('', start, end, 'end');
    }
    input.dispatchEvent(new InputEvent('input', { bubbles: true }));
  };

  return (
    <button
      class={cn(
        'border-border bg-background active:bg-secondary grow basis-4 rounded-md border inset-shadow-xs',
        local.class,
      )}
      onClick={trigger}
      {...rest}
    >
      <Icon iconName="backspace" />
    </button>
  );
};
