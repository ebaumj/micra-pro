import { toaster } from '@kobalte/core/toast';
import {
  Toast,
  ToastContent,
  ToastDescription,
  ToastProgress,
  ToastTitle,
} from '../Components/Toast';

export function handleError(error: { title: string; message?: string }) {
  toaster.show((props) => (
    <Toast toastId={props.toastId} variant="destructive">
      <ToastContent>
        <ToastTitle>{error.title}</ToastTitle>
        <ToastDescription>{error.message ?? ''}</ToastDescription>
      </ToastContent>
      <ToastProgress />
    </Toast>
  ));
}
