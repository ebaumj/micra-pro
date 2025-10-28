import { cn } from '../utils/cn';
import type {
  AlertDialogCloseButtonProps,
  AlertDialogContentProps,
  AlertDialogDescriptionProps,
  AlertDialogTitleProps,
} from '@kobalte/core/alert-dialog';
import { AlertDialog as AlertDialogPrimitive } from '@kobalte/core/alert-dialog';
import type { PolymorphicProps } from '@kobalte/core/polymorphic';
import type { ComponentProps, ParentProps, ValidComponent } from 'solid-js';
import { splitProps } from 'solid-js';
import { buttonVariants } from './Button';
import { useKeyboardInternal } from './Keyboard/KeyboardContext';
import { useDialogContext } from './DialogContext';
import { InteractOutsideEvent } from '@kobalte/core/*';

export const AlertDialog = AlertDialogPrimitive;
export const AlertDialogTrigger = AlertDialogPrimitive.Trigger;

type alertDialogContentProps<T extends ValidComponent = 'div'> = ParentProps<
  AlertDialogContentProps<T> & {
    class?: string;
  }
>;

export const AlertDialogContent = <T extends ValidComponent = 'div'>(
  props: PolymorphicProps<T, alertDialogContentProps<T>>,
) => {
  const [local, rest] = splitProps(props as alertDialogContentProps, [
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
    <AlertDialogPrimitive.Portal mount={dialogContext.mount}>
      <AlertDialogPrimitive.Overlay
        class={cn(
          'bg-background/80 fixed inset-0 z-50',
          'data-[expanded]:animate-in data-[expanded]:fade-in-0',
          'data-[closed]:animate-out data-[closed]:fade-out-0',
        )}
        ref={dialogOverlayRef}
      />
      <AlertDialogPrimitive.Content
        class={cn(
          'data-[expanded]:animate-in data-[expanded]:fade-in-0 data-[expanded]:zoom-in-95 data-[expanded]:slide-in-from-left-1/2 data-[expanded]:slide-in-from-top-[48%] data-expanded:duration-200',
          'data-[closed]:animate-out data-[closed]:fade-out-0 data-[closed]:zoom-out-95 data-[closed]:slide-out-to-left-1/2 data-[closed]:slide-out-to-top-[48%] data-closed:duration-200',
          'bg-background fixed top-[50%] left-[50%] z-50 grid w-full max-w-lg translate-x-[-50%] translate-y-[-50%] gap-4 rounded-lg border p-6 text-center shadow-lg outline-hidden',
          local.class,
        )}
        {...rest}
        onInteractOutside={interactOutside}
        ref={dialogContentRef}
      >
        {local.children}
      </AlertDialogPrimitive.Content>
    </AlertDialogPrimitive.Portal>
  );
};

export const AlertDialogHeader = (props: ComponentProps<'div'>) => {
  const [local, rest] = splitProps(props, ['class']);

  return <div class={cn('flex flex-col space-y-2', local.class)} {...rest} />;
};

export const AlertDialogFooter = (props: ComponentProps<'div'>) => {
  const [local, rest] = splitProps(props, ['class']);

  return (
    <div
      class={cn('flex flex-row justify-end space-x-2', local.class)}
      {...rest}
    />
  );
};

type alertDialogTitleProps<T extends ValidComponent = 'h2'> =
  AlertDialogTitleProps<T> & {
    class?: string;
  };

export const AlertDialogTitle = <T extends ValidComponent = 'h2'>(
  props: PolymorphicProps<T, alertDialogTitleProps<T>>,
) => {
  const [local, rest] = splitProps(props as alertDialogTitleProps, ['class']);

  return (
    <AlertDialogPrimitive.Title
      class={cn('text-xl font-semibold', local.class)}
      {...rest}
    />
  );
};

type alertDialogDescriptionProps<T extends ValidComponent = 'p'> =
  AlertDialogDescriptionProps<T> & {
    class?: string;
  };

export const AlertDialogDescription = <T extends ValidComponent = 'p'>(
  props: PolymorphicProps<T, alertDialogDescriptionProps<T>>,
) => {
  const [local, rest] = splitProps(props as alertDialogDescriptionProps, [
    'class',
  ]);

  return (
    <AlertDialogPrimitive.Description
      class={cn('text-muted-foreground', local.class)}
      {...rest}
    />
  );
};

type alertDialogCloseProps<T extends ValidComponent = 'button'> =
  AlertDialogCloseButtonProps<T> & {
    class?: string;
  };

export const AlertDialogClose = <T extends ValidComponent = 'button'>(
  props: PolymorphicProps<T, alertDialogCloseProps<T>>,
) => {
  const [local, rest] = splitProps(props as alertDialogCloseProps, ['class']);

  return (
    <AlertDialogPrimitive.CloseButton
      class={cn(
        buttonVariants({
          variant: 'outline',
        }),
        'mt-0',
        local.class,
      )}
      {...rest}
    />
  );
};

export const AlertDialogAction = <T extends ValidComponent = 'button'>(
  props: PolymorphicProps<T, alertDialogCloseProps<T>>,
) => {
  const [local, rest] = splitProps(props as alertDialogCloseProps, ['class']);

  return (
    <AlertDialogPrimitive.CloseButton
      class={cn(buttonVariants(), local.class)}
      {...rest}
    />
  );
};
