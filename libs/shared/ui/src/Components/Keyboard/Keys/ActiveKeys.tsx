import { JSX, mergeProps, ParentComponent, splitProps } from 'solid-js';
import { useKeyboardInternal, ActiveKeysType } from '../KeyboardContext';
import BaseKey from './BaseKey';
import { cn } from '../../../utils/cn';

const CreateActiveKey: (activeType: ActiveKeysType) => ParentComponent<
  Omit<JSX.ButtonHTMLAttributes<HTMLButtonElement>, 'onClick'> & {
    activeClass?: string;
    disabledClass?: string;
  }
> = (activeType: ActiveKeysType) => (props) => {
  const [local, rest] = splitProps(props, [
    'activeClass',
    'disabledClass',
    'class',
  ]);
  const merged = mergeProps(
    {
      activeClass: 'bg-gray-600 text-gray-100 active:bg-gray-500',
    },
    local,
  );
  const keyboardContext = useKeyboardInternal();

  return (
    <BaseKey
      {...rest}
      onClick={() =>
        keyboardContext.canToggleActive(activeType) &&
        keyboardContext.toggleActive(activeType)
      }
      class={cn(
        merged.class,
        keyboardContext.activeKeys()[activeType] ? merged.activeClass : '',
        !keyboardContext.canToggleActive(activeType)
          ? 'pointer-events-none bg-gray-300 text-gray-200 active:bg-gray-300'
          : '',
      )}
    />
  );
};

export const ShiftKey = CreateActiveKey('shift');
export const CapsLockKey = CreateActiveKey('caps-lock');
export const AltGrKey = CreateActiveKey('alt-gr');
