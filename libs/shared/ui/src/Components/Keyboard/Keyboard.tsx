import { Component, JSX } from 'solid-js';
import { Dynamic } from 'solid-js/web';
import { keyboards } from './Keyboards';
import { useKeyboardInternal } from './KeyboardContext';

export const Keyboard: Component<JSX.HTMLAttributes<HTMLDivElement>> = (
  props,
) => {
  const keyboardContext = useKeyboardInternal();

  return (
    <Dynamic
      component={keyboards[keyboardContext.currentLayout()]}
      {...props}
    />
  );
};

export default Keyboard;
