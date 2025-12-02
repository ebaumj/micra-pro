import { JSX, onCleanup, ParentComponent, splitProps } from 'solid-js';

const longPressDelayTimeMs = 600;

type LongPressDivProps = Omit<
  JSX.HTMLAttributes<HTMLDivElement>,
  'onPointerDown' | 'onPointerUp' | 'onPointerLeave'
> & {
  onClick?: () => void;
  onLongPress?: () => void;
  onPressStart?: () => void;
  onPressEnd?: () => void;
  delayTimeMs?: number;
  maxShortPressTimeMs?: number;
};

export const LongPressDiv: ParentComponent<LongPressDivProps> = (props) => {
  const [local, rest] = splitProps(props, [
    'class',
    'onClick',
    'onLongPress',
    'onPressStart',
    'onPressEnd',
    'delayTimeMs',
    'maxShortPressTimeMs',
  ]);
  let timer: ReturnType<typeof setTimeout> | null = null;
  let timer2: ReturnType<typeof setTimeout> | null = null;

  const delayTime = () => local.delayTimeMs ?? longPressDelayTimeMs;

  const keyDown = () => {
    local.onPressStart?.();
    timer = setTimeout(() => {
      timer = null;
      local.onLongPress?.();
    }, delayTime());
    if (local.maxShortPressTimeMs)
      timer2 = setTimeout(() => {
        timer2 = null;
      }, local.maxShortPressTimeMs);
  };

  const keyUp = () => {
    local.onPressEnd?.();
    if (timer && (timer2 || !local.maxShortPressTimeMs)) local.onClick?.();
    cleanup();
  };

  const cancel = () => {
    local.onPressEnd?.();
    cleanup();
  };

  const cleanup = () => timer !== null && clearTimeout(timer);

  onCleanup(cleanup);

  return (
    <div
      onPointerDown={keyDown}
      onPointerUp={keyUp}
      onPointerLeave={cancel}
      onPointerCancel={cancel}
      class={local.class}
      {...rest}
    >
      {props.children}
    </div>
  );
};
