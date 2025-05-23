import { Component, JSX, splitProps } from 'solid-js';
import { twMerge } from 'tailwind-merge';

export const Icon: Component<
  JSX.HTMLAttributes<HTMLSpanElement> & {
    iconName: string;
  }
> = (props) => {
  const [local, rest] = splitProps(props, ['iconName', 'class']);

  return (
    <span
      class={twMerge(
        'inline-block max-h-[1em] max-w-[1em] select-none overflow-hidden align-sub font-material-outlined text-[1.5em] leading-none',
        local.class,
      )}
      {...rest}
    >
      {local.iconName}
    </span>
  );
};
