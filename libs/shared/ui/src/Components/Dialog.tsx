import { cn } from '../utils/cn';
import type {
  DialogContentProps,
  DialogDescriptionProps,
  DialogTitleProps,
} from '@kobalte/core/dialog';
import { Dialog as DialogPrimitive } from '@kobalte/core/dialog';
import type { PolymorphicProps } from '@kobalte/core/polymorphic';
import type { ComponentProps, ParentProps, ValidComponent } from 'solid-js';
import { splitProps } from 'solid-js';
import { useKeyboardInternal } from './Keyboard/KeyboardContext';
import { InteractOutsideEvent } from '@kobalte/core/*';
import { useDialogContext } from './DialogContext';

export const Dialog = DialogPrimitive;
export const DialogTrigger = DialogPrimitive.Trigger;

type dialogContentProps<T extends ValidComponent = 'div'> = ParentProps<
  DialogContentProps<T> & {
    class?: string;
  }
>;

export const DialogContent = <T extends ValidComponent = 'div'>(
  props: PolymorphicProps<T, dialogContentProps<T>>,
) => {
  const [local, rest] = splitProps(props as dialogContentProps, [
    'class',
    'children',
    'onInteractOutside',
  ]);

  const getKeyboardContext = () => {
    try {
      return useKeyboardInternal();
    } catch {
      return null;
    }
  };

  const dialogContext = useDialogContext();
  const keyboardContext = getKeyboardContext();
  let dialogContentRef: HTMLDivElement = null!;
  let dialogOverlayRef: HTMLDivElement = null!;

  const interactOutside = (e: InteractOutsideEvent) => {
    if (local.onInteractOutside) local.onInteractOutside(e);
    if (keyboardContext?.isOpen()) {
      // prevent the dialog from closing when clicking outside the dialog
      // for example when clicking the keyboard or the overlay (backdrop)
      e.preventDefault();

      // when the user clicked on the overlay (backdrop) focus the dialog
      // -> this will close the keyboard
      if (e.target === dialogOverlayRef) {
        dialogContentRef.focus();
      }
    }
  };

  return (
    <DialogPrimitive.Portal mount={dialogContext.mount}>
      <DialogPrimitive.Overlay
        class={cn(
          'bg-background/80 data-expanded:animate-in data-closed:animate-out data-closed:fade-out-0 data-expanded:fade-in-0 fixed inset-0 z-50',
        )}
        {...rest}
        ref={dialogOverlayRef}
      />
      <DialogPrimitive.Content
        class={cn(
          'bg-background data-expanded:animate-in data-closed:animate-out data-closed:fade-out-0 data-expanded:fade-in-0 data-closed:zoom-out-95 data-expanded:zoom-in-95 data-closed:slide-out-to-top-1/2 data-expanded:slide-in-from-top-1/2 fixed top-[50%] left-[50%] z-50 grid w-full max-w-lg translate-x-[-50%] translate-y-[-50%] gap-4 border p-6 shadow-lg data-closed:duration-200 data-expanded:duration-200 sm:rounded-lg md:w-full',
          local.class,
        )}
        {...rest}
        onInteractOutside={interactOutside}
        ref={dialogContentRef}
      >
        {local.children}
        <DialogPrimitive.CloseButton class="ring-offset-background focus:ring-ring absolute top-1 right-1 flex h-10 w-10 items-center justify-center rounded-xs opacity-70 transition-[opacity,box-shadow] hover:opacity-100 focus:ring-[1.5px] focus:ring-offset-2 focus:outline-hidden disabled:pointer-events-none">
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
      </DialogPrimitive.Content>
    </DialogPrimitive.Portal>
  );
};

type dialogTitleProps<T extends ValidComponent = 'h2'> = DialogTitleProps<T> & {
  class?: string;
};

export const DialogTitle = <T extends ValidComponent = 'h2'>(
  props: PolymorphicProps<T, dialogTitleProps<T>>,
) => {
  const [local, rest] = splitProps(props as dialogTitleProps, ['class']);

  return (
    <DialogPrimitive.Title
      class={cn('text-foreground text-lg font-semibold', local.class)}
      {...rest}
    />
  );
};

type dialogDescriptionProps<T extends ValidComponent = 'p'> =
  DialogDescriptionProps<T> & {
    class?: string;
  };

export const DialogDescription = <T extends ValidComponent = 'p'>(
  props: PolymorphicProps<T, dialogDescriptionProps<T>>,
) => {
  const [local, rest] = splitProps(props as dialogDescriptionProps, ['class']);

  return (
    <DialogPrimitive.Description
      class={cn('text-muted-foreground text-sm', local.class)}
      {...rest}
    />
  );
};

export const DialogHeader = (props: ComponentProps<'div'>) => {
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

export const DialogFooter = (props: ComponentProps<'div'>) => {
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
