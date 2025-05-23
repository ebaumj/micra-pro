import { ParentComponent, JSX } from 'solid-js';
import { useKeyboardInternal } from '../KeyboardContext';
import BaseKey from './BaseKey';

export const EnterKey: ParentComponent<
  JSX.HTMLAttributes<HTMLButtonElement>
> = (props) => {
  const keyboardContext = useKeyboardInternal();

  return (
    <BaseKey onClick={() => keyboardContext.input()?.blur()} {...props}>
      {props.children}
    </BaseKey>
  );
};
