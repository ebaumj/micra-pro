import { ParentComponent } from 'solid-js';
import { PressAndHoldKey, type PressAndHoldKeyProps } from './PressAndHoldKey';
import { useKeyboardInternal } from '../KeyboardContext';

export const BackspaceKey: ParentComponent<
  Omit<PressAndHoldKeyProps, 'trigger'>
> = (props) => {
  const keyboardContext = useKeyboardInternal();

  const trigger = () => {
    const input = keyboardContext.input();
    if (input === null) return;

    const start = input.selectionStart || 0;
    const end = input.selectionEnd || 0;

    if (start === end && start > 0) {
      input.setRangeText('', start - 1, end, 'end');
    } else {
      input.setRangeText('', start, end, 'end');
    }
    input.dispatchEvent(new InputEvent('input', { bubbles: true }));
  };

  return <PressAndHoldKey trigger={trigger} {...props} />;
};
