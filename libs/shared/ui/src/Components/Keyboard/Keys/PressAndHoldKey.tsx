import { ParentComponent, splitProps, onCleanup, JSX } from 'solid-js';
import BaseKey from './BaseKey';

const initialLongPressDelayMs = 600;
const longPressIntervalMs = 100;

export type PressAndHoldKeyProps = Omit<
  JSX.ButtonHTMLAttributes<HTMLButtonElement>,
  'onPointerDown' | 'onPointerUp' | 'onPointerLeave'
> & {
  trigger: () => void;
};

export const PressAndHoldKey: ParentComponent<PressAndHoldKeyProps> = (
  props,
) => {
  const [local, rest] = splitProps(props, ['trigger']);

  let timer: ReturnType<typeof setTimeout> | null = null;

  const keyDown = () => {
    local.trigger();

    timer = setTimeout(() => {
      timer = setInterval(() => {
        local.trigger();
      }, longPressIntervalMs);
    }, initialLongPressDelayMs - longPressIntervalMs);
  };

  const cleanup = () => timer !== null && clearTimeout(timer);

  onCleanup(cleanup);

  return (
    <BaseKey
      onPointerDown={keyDown}
      onPointerUp={cleanup}
      onPointerLeave={cleanup}
      onPointerCancel={cleanup}
      {...rest}
    />
  );
};
