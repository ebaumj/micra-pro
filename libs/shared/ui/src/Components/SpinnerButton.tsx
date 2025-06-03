import { ButtonRootProps } from '@kobalte/core/button';
import { PolymorphicProps } from '@kobalte/core/polymorphic';
import { VariantProps } from 'class-variance-authority';
import { JSXElement, Show, splitProps, ValidComponent } from 'solid-js';
import { Button, buttonVariants } from './Button';
import { Spinner } from './Spinner';

type spinnerButtonProps<T extends ValidComponent = 'button'> =
  ButtonRootProps<T> &
    VariantProps<typeof buttonVariants> & {
      class?: string;
      spinnerClass?: string;
      loading?: boolean;
      children?: JSXElement;
    };

export const SpinnerButton = <T extends ValidComponent = 'button'>(
  props: PolymorphicProps<T, spinnerButtonProps<T>>,
) => {
  const [local, rest] = splitProps(props as spinnerButtonProps, [
    'class',
    'variant',
    'size',
    'loading',
    'children',
    'spinnerClass',
  ]);

  return (
    <Button
      variant={local.variant}
      size={local.size}
      class={local.class}
      {...rest}
    >
      <Show when={local.loading}>
        <Spinner class={local.spinnerClass} />
      </Show>
      <Show when={!local.loading}>{local.children}</Show>
    </Button>
  );
};
