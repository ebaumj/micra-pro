import { mergeProps, ParentComponent, splitProps } from 'solid-js';
import {
  KeyboardContextTypeInternal,
  useKeyboardInternal,
} from '../KeyboardContext';
import { PressAndHoldKey, type PressAndHoldKeyProps } from './PressAndHoldKey';

const InsertChar = (char: string, context: KeyboardContextTypeInternal) => {
  const input = context.input();
  if (input === null) return;

  input.setRangeText(
    char,
    input.selectionStart || 0,
    input.selectionEnd || 0,
    'end',
  );
  input.scrollLeft = input.scrollWidth;
  input.dispatchEvent(new InputEvent('input', { bubbles: true }));
  if (context.activeKeys().shift) context.toggleActive('shift');
};

export const Key: ParentComponent<
  Omit<PressAndHoldKeyProps, 'trigger' | 'autoCapitalize'> & {
    value: string;
    autoCapitalize?: boolean;
  }
> = (props) => {
  const [local, rest] = splitProps(props, [
    'autoCapitalize',
    'children',
    'value',
  ]);
  const merged = mergeProps({ autoCapitalize: true }, local);

  const keyboardContext = useKeyboardInternal();

  const keyValue = () =>
    merged.autoCapitalize &&
    (keyboardContext.activeKeys().shift ||
      keyboardContext.activeKeys()['caps-lock'])
      ? local.value.toUpperCase()
      : local.value;

  return (
    <PressAndHoldKey
      {...rest}
      trigger={() => InsertChar(keyValue(), keyboardContext)}
    >
      {local.children ?? keyValue()}
    </PressAndHoldKey>
  );
};
