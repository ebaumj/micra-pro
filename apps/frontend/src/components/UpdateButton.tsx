import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogHeader,
  AlertDialogTitle,
  Button,
  Dialog,
  DialogContent,
  DialogTitle,
  handleError,
  Icon,
  Spinner,
} from '@micra-pro/shared/ui';
import { Component, createSignal, Show } from 'solid-js';
import { twMerge } from 'tailwind-merge';
import { useUpdateContext } from './UpdateContextProvider';
import { T, useTranslationContext } from '../generated/language-types';

export const UpdateButton: Component<{
  class?: string;
}> = (props) => {
  const { t } = useTranslationContext();
  const [dialogOpen, setDialogOpen] = createSignal(false);
  const updateContext = useUpdateContext();
  const [installing, setInstalling] = createSignal(false);
  const installUpdate = (installer: () => Promise<void>) => {
    setInstalling(true);
    setDialogOpen(false);
    installer().catch(() => {
      handleError({
        title: t('update-failed-title'),
        message: t('update-failed-messsage'),
      });
      setInstalling(false);
    });
  };
  return (
    <>
      <AlertDialog open={installing()}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>
              <T key="installing-update" />
            </AlertDialogTitle>
          </AlertDialogHeader>
          <AlertDialogDescription>
            <div class="mb-4">
              <T key="installing-update-deatils" />
            </div>
            <Spinner class="h-10 px-1" />
          </AlertDialogDescription>
        </AlertDialogContent>
      </AlertDialog>
      <Button
        class={twMerge('h-full w-full', props.class)}
        variant="outline"
        onClick={() => setDialogOpen(true)}
      >
        <Icon
          iconName="deployed_code_update"
          class={twMerge(
            'flex items-center text-3xl',
            updateContext.newVersion() ? 'animate-pulse' : '',
          )}
        />
      </Button>
      <Dialog open={dialogOpen()} onOpenChange={setDialogOpen}>
        <DialogContent
          class="flex h-40 max-w-80 flex-col"
          onOpenAutoFocus={(e) => e.preventDefault()}
          onInteractOutside={(e) => e.preventDefault()}
        >
          <DialogTitle>Software Version</DialogTitle>
          <div class="flex h-full w-full justify-center">
            <div class="flex h-full w-1/3 items-center justify-center rounded-md border shadow-inner">
              {updateContext.currentVersion()}
            </div>
            <Show when={updateContext.newVersion()}>
              {(v) => (
                <>
                  <div class="flex h-full w-1/3 items-center justify-center px-3 py-2">
                    <Button
                      class="flex h-full w-full items-center justify-center"
                      onClick={() => installUpdate(v().install)}
                    >
                      <Icon
                        iconName="settings_b_roll"
                        class="flex items-center justify-center text-2xl"
                      />
                    </Button>
                  </div>
                  <div class="flex h-full w-1/3 items-center justify-center rounded-md border shadow-inner">
                    {v().version}
                  </div>
                </>
              )}
            </Show>
          </div>
        </DialogContent>
      </Dialog>
    </>
  );
};
