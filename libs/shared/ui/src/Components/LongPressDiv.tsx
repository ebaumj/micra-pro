import { JSX, onCleanup, ParentComponent, splitProps } from 'solid-js';

const longPressDelayTimeMs = 600;

type LongPressDivProps = Omit<
  JSX.HTMLAttributes<HTMLDivElement>,
  'onPointerDown' | 'onPointerUp' | 'onPointerLeave'
> & {
  onClick?: () => void;
  onLongPress?: () => void;
};

export const LongPressDiv: ParentComponent<LongPressDivProps> = (props) => {
  const [local, rest] = splitProps(props, ['onClick', 'onLongPress']);
  let timer: ReturnType<typeof setTimeout> | null = null;

  const keyDown = () => {
    timer = setTimeout(() => {
      timer = null;
      local.onLongPress?.();
    }, longPressDelayTimeMs);
  };

  const keyUp = () => {
    if (timer) local.onClick?.();
    cleanup();
  };

  const cleanup = () => timer !== null && clearTimeout(timer);

  onCleanup(cleanup);

  return (
    <div
      onPointerDown={keyDown}
      onPointerUp={keyUp}
      onPointerLeave={cleanup}
      onPointerCancel={cleanup}
      {...rest}
    >
      {props.children}
    </div>
  );
};
