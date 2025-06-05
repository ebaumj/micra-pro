import { Component, JSX, splitProps } from 'solid-js';
import { cn } from '../../utils/cn';

export const NumberKey: Component<
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
        'grow basis-4 rounded-md border border-input bg-white active:bg-gray-200',
        local.class,
      )}
      onClick={InsertChar}
      {...rest}
    >
      {props.value}
    </button>
  );
};
