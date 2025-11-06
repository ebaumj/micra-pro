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
      activeClass:
        'bg-primary text-primary-foreground active:bg-primary-active',
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
          ? 'bg-muted text-muted-foreground pointer-events-none'
          : '',
      )}
    />
  );
};

export const ShiftKey = CreateActiveKey('shift');
export const CapsLockKey = CreateActiveKey('caps-lock');
export const AltGrKey = CreateActiveKey('alt-gr');
