import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogHeader,
  AlertDialogTitle,
  Button,
  Dialog,
  DialogContent,
  handleError,
  Icon,
  Spinner,
  SpinnerButton,
} from '@micra-pro/shared/ui';
import { Component, createSignal, For } from 'solid-js';
import { twMerge } from 'tailwind-merge';
import { useBackupContext } from './BackupContextProvider';
import { T, useTranslationContext } from '../generated/language-types';

const Backup: Component<{
  backup: {
    restore: () => Promise<void>;
    delete: () => Promise<void>;
    timestamp: Date;
  };
  showRestore(process: Promise<void>): void;
}> = (props) => {
  const { t } = useTranslationContext();
  const [deleting, setDeleting] = createSignal(false);
  const deleteBackup = () => {
    setDeleting(true);
    props.backup
      .delete()
      .then(() => setDeleting(false))
      .catch(() => {
        setDeleting(false);
        handleError({ title: t('restore-failed') });
      });
  };
  return (
    <div class="flex h-12 w-full items-center gap-2 rounded-md border px-3">
      <div class="w-4/6">{props.backup.timestamp.toLocaleString()}</div>
      <div class="flex w-1/6 items-center">
        <Button
          class="w-full"
          onClick={() => props.showRestore(props.backup.restore())}
        >
          <Icon iconName="cloud_download" />
        </Button>
      </div>
      <div class="flex w-1/6 items-center">
        <SpinnerButton
          variant="destructive"
          loading={deleting()}
          onClick={deleteBackup}
          class="w-full"
        >
          <Icon iconName="delete" />
        </SpinnerButton>
      </div>
    </div>
  );
};

export const RestoreButton: Component<{
  class?: string;
}> = (props) => {
  const context = useBackupContext();
  const [dialogOpen, setDialogOpen] = createSignal(false);
  const [restoring, setRestoring] = createSignal(false);
  const showRestore = (process: Promise<void>) => {
    setDialogOpen(false);
    setRestoring(true);
    process
      .then(() => location.reload())
      .catch(() => {
        setRestoring(false);
        handleError({ title: 'Restore Failed' });
      });
  };
  return (
    <>
      <Button
        class={twMerge('h-full w-full', props.class)}
        variant="outline"
        disabled={!context.enabled()}
        onClick={() => setDialogOpen(true)}
      >
        <Icon
          iconName="cloud_download"
          class={twMerge('flex items-center text-3xl')}
        />
      </Button>

      <AlertDialog open={restoring()}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>
              <T key="restore-data" />
            </AlertDialogTitle>
          </AlertDialogHeader>
          <AlertDialogDescription>
            <div class="mb-4">
              <T key="restore-data-deatils" />
            </div>
            <Spinner class="h-10 px-1" />
          </AlertDialogDescription>
        </AlertDialogContent>
      </AlertDialog>
      <Dialog open={dialogOpen()} onOpenChange={setDialogOpen}>
        <DialogContent
          class="flex h-96 w-full flex-col"
          onOpenAutoFocus={(e) => e.preventDefault()}
          onInteractOutside={(e) => e.preventDefault()}
        >
          <div class="no-scrollbar flex h-full flex-col gap-2 overflow-scroll px-6 py-2">
            <For each={context.available()}>
              {(backup) => <Backup backup={backup} showRestore={showRestore} />}
            </For>
          </div>
        </DialogContent>
      </Dialog>
    </>
  );
};
