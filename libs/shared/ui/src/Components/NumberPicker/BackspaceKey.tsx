import { Component, JSX, splitProps } from 'solid-js';
import { cn } from '../../utils/cn';
import { Icon } from '../Icon';

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
        'grow basis-4 rounded-md border border-input bg-white active:bg-gray-200',
        local.class,
      )}
      onClick={trigger}
      {...rest}
    >
      <Icon iconName="backspace" />
    </button>
  );
};
