import { Roastery } from '@micra-pro/bean-management/data-access';
import {
  Dialog,
  DialogContent,
  Icon,
  selectPicturesForMode,
} from '@micra-pro/shared/ui';
import { Component, createEffect, createSignal, For, Show } from 'solid-js';
import { twMerge } from 'tailwind-merge';
import picturesImport from '../generated/pictures-import';

export const BeanSelectorDialog: Component<{
  isOpen: boolean;
  onClose: () => void;
  onBeanSelected: (beanId: string) => void;
  roasteries: Roastery[];
}> = (props) => {
  const pictures = selectPicturesForMode(picturesImport);
  const [selectedRoastery, setSelectedRoastery] = createSignal(-1);
  const selectRoastery = (i: number) =>
    setSelectedRoastery((idx) => (idx === i ? -1 : i));
  const beans = () => props.roasteries[selectedRoastery()]?.beans ?? [];
  createEffect(() => !props.isOpen && setSelectedRoastery(-1));
  return (
    <Dialog
      open={props.isOpen}
      onOpenChange={(o) => (o ? undefined : props.onClose())}
    >
      <DialogContent
        onOpenAutoFocus={(e) => e.preventDefault()}
        onInteractOutside={(e) => e.preventDefault()}
      >
        <div class="flex h-80 w-full px-2 py-4">
          <div class="w-1/2 border-r">
            <div class="no-scrollbar flex h-full flex-col overflow-x-hidden overflow-y-scroll pr-6">
              <div class="w-full border-b" />
              <For each={props.roasteries}>
                {(r, i) => (
                  <div
                    class={twMerge(
                      'flex h-10 border-b',
                      selectedRoastery() === i()
                        ? 'bg-secondary inset-shadow-sm'
                        : '',
                    )}
                    onClick={() => selectRoastery(i())}
                  >
                    <div class="flex h-full w-12 items-center justify-center">
                      <div class="pb-2">
                        <Icon class="text-xl" iconName="folder" />
                      </div>
                    </div>
                    <div class="w-full gap-0 py-1">
                      <div class="flex h-full items-center overflow-hidden text-sm font-bold whitespace-nowrap">
                        {r.properties.name}
                      </div>
                    </div>
                    <div class="flex h-full w-10 items-center justify-center">
                      <Show when={selectedRoastery() === i()}>
                        <div class="pb-0.5">
                          <Icon class="text-2xl" iconName="navigate_next" />
                        </div>
                      </Show>
                    </div>
                  </div>
                )}
              </For>
            </div>
          </div>
          <div class="no-scrollbar flex h-full w-1/2 flex-col overflow-x-hidden overflow-y-scroll pl-6">
            <div class="w-full border-b" />
            <For each={beans()}>
              {(b) => (
                <div
                  class="flex border-b"
                  onClick={() => props.onBeanSelected(b.id)}
                >
                  <div class="flex h-10 w-12 items-center justify-center">
                    <div class="">
                      <img src={pictures().bean} class="h-full w-full p-3" />
                    </div>
                  </div>
                  <div class="w-full gap-0 py-1">
                    <div class="flex h-full items-center overflow-hidden text-sm font-bold whitespace-nowrap">
                      {b.properties.name}
                    </div>
                  </div>
                </div>
              )}
            </For>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};
