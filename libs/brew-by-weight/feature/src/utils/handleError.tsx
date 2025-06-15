import {
  Toast,
  ToastContent,
  ToastTitle,
  ToastDescription,
  ToastProgress,
} from '@micra-pro/shared/ui';
import { toaster } from '@kobalte/core/toast';
import { Show } from 'solid-js';
import { T } from '../generated/language-types';

export function handleError(error: { message?: string }) {
  toaster.show((props) => (
    <Toast toastId={props.toastId} variant="destructive">
      <ToastContent>
        <ToastTitle>
          <T key="error" />
        </ToastTitle>
        <ToastDescription>
          <Show when={error.message} fallback={<T key="unknown-error" />}>
            {error.message}
          </Show>
        </ToastDescription>
      </ToastContent>
      <ToastProgress />
    </Toast>
  ));
}
