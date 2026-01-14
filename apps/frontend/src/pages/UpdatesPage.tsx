import { fetchUpdates } from '@micra-pro/recipe-hub/data-access';
import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogHeader,
  AlertDialogTitle,
  Button,
  handleError,
  Spinner,
} from '@micra-pro/shared/ui';
import { updateAccess } from '@micra-pro/shared/utils-ts';
import { Component, createSignal, For, Show } from 'solid-js';
import { T, useTranslationContext } from '../generated/language-types';

const twoNumbers = (value: number) => (value < 10 ? `0${value}` : `${value}`);
const formatDate = (date: Date) =>
  `${twoNumbers(date.getDate())}.${twoNumbers(date.getMonth() + 1)}.${date.getFullYear()}`;

export const UpdatesPage: Component = () => {
  const { t } = useTranslationContext();
  const accessor = updateAccess();
  const available = fetchUpdates();
  const [installing, setInstalling] = createSignal(false);
  const installUpdate = (link: string, signature: string) => {
    setInstalling(true);
    accessor.installUpdate(link, signature).catch(() => {
      handleError({
        title: t('update-failed-title'),
        message: t('update-failed-messsage'),
      });
      setInstalling(false);
    });
  };
  return (
    <div class="flex h-full w-full flex-col">
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
      <div class="bg-secondary flex min-h-12 w-full items-center gap-4 rounded-md px-2 shadow-md">
        <Show when={accessor.loading()}>
          <div class="flex h-full w-full items-center justify-center">
            <Spinner class="h-6 px-1" />
          </div>
        </Show>
        <Show when={!accessor.loading()}>
          <div class="flex w-1/3 items-center justify-center">
            <T key="current-version" />
          </div>
          <div class="flex w-1/3 items-center justify-center">
            {accessor.currentVersion()}
          </div>
          <div class="flex w-1/3 items-center justify-center" />
        </Show>
      </div>
      <div class="no-scrollbar flex h-full flex-col gap-2 overflow-scroll py-2">
        <Show when={available.loading()}>
          <div class="flex h-full w-full items-center justify-center">
            <Spinner class="h-12 px-1" />
          </div>
        </Show>
        <Show when={!available.loading()}>
          <For each={available.updates()}>
            {(update) => (
              <div class="flex h-12 w-full gap-4 rounded-md border px-2">
                <div class="flex w-1/3 items-center justify-center">
                  {formatDate(update.created_at)}
                </div>
                <div class="flex w-1/3 items-center justify-center">
                  {update.version}
                </div>
                <div class="flex w-1/3 items-center justify-center">
                  <Button
                    class="w-full"
                    disabled={update.version === accessor.currentVersion()}
                    onClick={() => installUpdate(update.link, update.signature)}
                  >
                    <T key="install" />
                  </Button>
                </div>
              </div>
            )}
          </For>
        </Show>
      </div>
    </div>
  );
};
