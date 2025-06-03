import { Component } from 'solid-js';
import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogHeader,
  AlertDialogTitle,
} from './AlertDialog';
import { Icon } from '@micra-pro/shared/ui';

export type ErrorDialogProps = {
  open: boolean;
  title: string;
  description: string;
};

export const ErrorDialog: Component<ErrorDialogProps> = (props) => {
  return (
    <AlertDialog open={props.open}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>
            <div class="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-destructive text-xl font-extralight text-destructive-foreground">
              <Icon iconName="error" class="" />
            </div>
            <span>{props.title}</span>
          </AlertDialogTitle>
          <AlertDialogDescription>{props.description}</AlertDialogDescription>
        </AlertDialogHeader>
      </AlertDialogContent>
    </AlertDialog>
  );
};

export default ErrorDialog;
