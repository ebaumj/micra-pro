import { cn } from '../utils/cn';
import type { ButtonRootProps } from '@kobalte/core/button';
import { Button as ButtonPrimitive } from '@kobalte/core/button';
import type { PolymorphicProps } from '@kobalte/core/polymorphic';
import type { VariantProps } from 'class-variance-authority';
import { cva } from 'class-variance-authority';
import type { ValidComponent } from 'solid-js';
import { splitProps } from 'solid-js';

export const buttonVariants = cva(
  'inline-flex items-center justify-center rounded-md text-sm font-medium transition-[color,background-color,box-shadow] disabled:pointer-events-none disabled:opacity-50',
  {
    variants: {
      variant: {
        default:
          'bg-primary text-primary-foreground shadow-sm active:bg-primary-active',
        destructive:
          'bg-destructive text-destructive-foreground shadow-xs active:bg-destructive/90',
        outline:
          'border border-border bg-background shadow-xs active:bg-secondary active:text-secondary-foreground',
        secondary:
          'bg-secondary text-secondary-foreground shadow-xs active:bg-secondary/80',
        ghost: 'active:bg-secondary active:text-secondary-foreground',
        link: 'text-primary underline-offset-4 active:underline',
      },
      size: {
        default: 'h-9 px-4 py-2',
        sm: 'h-8 rounded-md px-3 text-xs',
        lg: 'h-10 rounded-md px-8',
        icon: 'h-9 w-9',
      },
    },
    defaultVariants: {
      variant: 'default',
      size: 'default',
    },
  },
);

type buttonProps<T extends ValidComponent = 'button'> = ButtonRootProps<T> &
  VariantProps<typeof buttonVariants> & {
    class?: string;
  };

export const Button = <T extends ValidComponent = 'button'>(
  props: PolymorphicProps<T, buttonProps<T>>,
) => {
  const [local, rest] = splitProps(props as buttonProps, [
    'class',
    'variant',
    'size',
  ]);

  return (
    <ButtonPrimitive
      class={cn(
        buttonVariants({
          size: local.size,
          variant: local.variant,
        }),
        local.class,
      )}
      {...rest}
    />
  );
};
