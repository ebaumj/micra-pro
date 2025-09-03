import { toaster } from '@kobalte/core/toast';
import {
  Toast,
  ToastContent,
  ToastDescription,
  ToastProgress,
  ToastTitle,
} from '../Components/Toast';

export function makeToast(msg: { title: string; message?: string }) {
  toaster.show((props) => (
    <Toast toastId={props.toastId} variant="default">
      <ToastContent>
        <ToastTitle>{msg.title}</ToastTitle>
        <ToastDescription>{msg.message ?? ''}</ToastDescription>
      </ToastContent>
      <ToastProgress />
    </Toast>
  ));
}
