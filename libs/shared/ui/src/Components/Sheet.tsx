import { cn } from '../utils/cn';
import type {
  DialogContentProps,
  DialogDescriptionProps,
  DialogTitleProps,
} from '@kobalte/core/dialog';
import { Dialog as DialogPrimitive } from '@kobalte/core/dialog';
import type { PolymorphicProps } from '@kobalte/core/polymorphic';
import type { VariantProps } from 'class-variance-authority';
import { cva } from 'class-variance-authority';
import type { ComponentProps, ParentProps, ValidComponent } from 'solid-js';
import { mergeProps, Show, splitProps } from 'solid-js';
import { useDialogContext } from './DialogContext';

export const Sheet = DialogPrimitive;
export const SheetTrigger = DialogPrimitive.Trigger;

export const sheetVariants = cva(
  'fixed z-50 gap-4 bg-background p-6 shadow-lg transition ease-in-out data-expanded:animate-in data-closed:animate-out data-expanded:duration-200 data-closed:duration-200',
  {
    variants: {
      side: {
        top: 'inset-x-0 top-0 border-b data-closed:slide-out-to-top data-expanded:slide-in-from-top',
        bottom:
          'inset-x-0 bottom-0 border-t data-closed:slide-out-to-bottom data-expanded:slide-in-from-bottom',
        left: 'inset-y-0 left-0 h-full w-3/4 border-r data-closed:slide-out-to-left data-expanded:slide-in-from-left sm:max-w-sm',
        right:
          'inset-y-0 right-0 h-full w-3/4 border-l data-closed:slide-out-to-right data-expanded:slide-in-from-right sm:max-w-sm',
      },
    },
    defaultVariants: {
      side: 'right',
    },
  },
);

type sheetContentProps<T extends ValidComponent = 'div'> = ParentProps<
  DialogContentProps<T> &
    VariantProps<typeof sheetVariants> & {
      class?: string;
      closeButton?: boolean;
    }
>;

export const SheetContent = <T extends ValidComponent = 'div'>(
  props: PolymorphicProps<T, sheetContentProps<T>>,
) => {
  const merge = mergeProps<sheetContentProps<T>[]>({ side: 'right' }, props);
  const [local, rest] = splitProps(merge as sheetContentProps, [
    'class',
    'children',
    'side',
    'closeButton',
  ]);
  const dialogContext = useDialogContext();

  return (
    <DialogPrimitive.Portal mount={dialogContext.mount}>
      <DialogPrimitive.Overlay
        class={cn(
          'bg-background/80 data-expanded:animate-in data-closed:animate-out data-closed:fade-out-0 data-expanded:fade-in-0 fixed inset-0 z-50',
        )}
      />
      <DialogPrimitive.Content
        class={sheetVariants({ side: local.side, class: local.class })}
        {...rest}
      >
        {local.children}
        <Show when={local.closeButton !== false}>
          <DialogPrimitive.CloseButton class="ring-offset-background focus:ring-ring absolute top-4 right-4 rounded-xs opacity-70 transition-[opacity,box-shadow] focus:ring-[1.5px] focus:ring-offset-2 focus:outline-hidden active:opacity-100 disabled:pointer-events-none">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 24 24"
              class="h-4 w-4"
            >
              <path
                fill="none"
                stroke="currentColor"
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M18 6L6 18M6 6l12 12"
              />
              <title>Close</title>
            </svg>
          </DialogPrimitive.CloseButton>
        </Show>
      </DialogPrimitive.Content>
    </DialogPrimitive.Portal>
  );
};

type sheetTitleProps<T extends ValidComponent = 'h2'> = DialogTitleProps<T> & {
  class?: string;
};

export const SheetTitle = <T extends ValidComponent = 'h2'>(
  props: PolymorphicProps<T, sheetTitleProps<T>>,
) => {
  const [local, rest] = splitProps(props as sheetTitleProps, ['class']);

  return (
    <DialogPrimitive.Title
      class={cn('text-foreground text-lg font-semibold', local.class)}
      {...rest}
    />
  );
};

type sheetDescriptionProps<T extends ValidComponent = 'p'> =
  DialogDescriptionProps<T> & {
    class?: string;
  };

export const SheetDescription = <T extends ValidComponent = 'p'>(
  props: PolymorphicProps<T, sheetDescriptionProps<T>>,
) => {
  const [local, rest] = splitProps(props as sheetDescriptionProps, ['class']);

  return (
    <DialogPrimitive.Description
      class={cn('text-muted-foreground text-sm', local.class)}
      {...rest}
    />
  );
};

export const SheetHeader = (props: ComponentProps<'div'>) => {
  const [local, rest] = splitProps(props, ['class']);

  return (
    <div
      class={cn(
        'flex flex-col space-y-2 text-center sm:text-left',
        local.class,
      )}
      {...rest}
    />
  );
};

export const SheetFooter = (props: ComponentProps<'div'>) => {
  const [local, rest] = splitProps(props, ['class']);

  return (
    <div
      class={cn(
        'flex flex-col-reverse sm:flex-row sm:justify-end sm:space-x-2',
        local.class,
      )}
      {...rest}
    />
  );
};
