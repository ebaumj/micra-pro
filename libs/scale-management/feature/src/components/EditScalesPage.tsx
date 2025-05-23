import { createScalesAccessor } from '@micra-pro/scale-management/data-access';
import {
  AlertDialog,
  AlertDialogContent,
  Button,
  Dialog,
  DialogContent,
  Icon,
  Spinner,
  SpinnerButton,
  TextField,
  TextFieldRoot,
} from '@micra-pro/shared/ui';
import { Component, createSignal, For } from 'solid-js';
import { ScanScalesDialog } from './ScanScalesDialog';

export const EditScalesPage: Component = () => {
  const scalesAccessor = createScalesAccessor();
  const scales = () =>
    scalesAccessor.scales().map((s) => {
      const [name, setName] = createSignal(s.name);
      return {
        name: name,
        setName,
        save: () => {
          s.rename(name());
        },
        remove: s.remove,
        canSave: () => s.name !== name(),
        isUpdating: s.isUpdating,
        isDeleting: s.isDeleting,
      };
    });

  const [scanDialog, setScanDialog] = createSignal(false);

  const addDevice = (identifier: string, name: string) => {
    scalesAccessor.add(identifier, name, () => setScanDialog(false));
  };

  return (
    <div class="flex h-full flex-col">
      <AlertDialog open={scalesAccessor.isLoading()}>
        <AlertDialogContent class="flex items-center justify-center p-8">
          <Spinner class="h-20 w-20" />
        </AlertDialogContent>
      </AlertDialog>
      <Dialog open={scanDialog()} onOpenChange={(o) => setScanDialog(o)}>
        <DialogContent
          onOpenAutoFocus={(e) => e.preventDefault()}
          onInteractOutside={(e) => e.preventDefault()}
        >
          <ScanScalesDialog
            isOpen={scanDialog()}
            close={() => setScanDialog(false)}
            addDevice={addDevice}
          />
        </DialogContent>
      </Dialog>
      <div class="no-scrollbar flex h-full flex-col gap-2 overflow-scroll">
        <For each={scales()}>
          {(scale) => (
            <div class="flex w-full gap-4 rounded-lg border bg-slate-50 p-2 shadow-sm">
              <div class="flex h-full w-full items-center">
                <TextFieldRoot
                  onChange={(name) => scale.setName(name)}
                  class="w-full bg-white"
                >
                  <TextField value={scale.name()} />
                </TextFieldRoot>
              </div>
              <div class="flex h-full items-center">
                <SpinnerButton
                  class="h-10 w-10 bg-white p-0"
                  spinnerClass="p-2"
                  variant="outline"
                  onClick={scale.save}
                  disabled={!scale.canSave()}
                  loading={scale.isUpdating()}
                >
                  <Icon iconName="save" />
                </SpinnerButton>
              </div>
              <div class="flex h-full items-center">
                <SpinnerButton
                  class="h-10 w-10 bg-white p-0"
                  variant="outline"
                  onClick={scale.remove}
                  loading={scale.isDeleting()}
                >
                  <Icon iconName="delete" />
                </SpinnerButton>
              </div>
            </div>
          )}
        </For>
      </div>
      <div class="flex justify-end pt-4">
        <Button
          class="flex h-14 w-14 items-center justify-center shadow-md"
          variant="outline"
          onClick={() => setScanDialog(true)}
        >
          <Icon iconName="bluetooth_searching" />
        </Button>
      </div>
    </div>
  );
};
