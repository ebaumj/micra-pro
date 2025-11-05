import { Component, createSignal, ValidComponent } from 'solid-js';
import { fetchRoasteriesLevel } from '@micra-pro/bean-management/data-access';
import { BeanSelectorDialog } from '@micra-pro/bean-management/feature';
import {
  BrewByWeightHistoryEntry,
  createHistoryAccessor,
} from '@micra-pro/brew-by-weight/data-access';
import {
  AlertDialog,
  AlertDialogContent,
  Button,
  Icon,
  Spinner,
} from '@micra-pro/shared/ui';
import { HistoryEntryDetailDialog } from './HistoryEntryDetailDialog';
import { twMerge } from 'tailwind-merge';
import { Dynamic } from 'solid-js/web';
import { createScalesAccessor } from '@micra-pro/scale-management/data-access';
import {
  CancelledDataTable,
  FailedDataTable,
  FinishedDataTable,
} from './DataTable';

export const BrewByWeightHistoryPage: Component = () => {
  const roasteries = fetchRoasteriesLevel();
  const historyAccessor = createHistoryAccessor();
  const scalesAccessor = createScalesAccessor();
  const isLoading = () =>
    roasteries.isLoading() ||
    historyAccessor.initalLoad() ||
    scalesAccessor.isLoading();
  const [deatilEntry, setDetailEntry] =
    createSignal<BrewByWeightHistoryEntry | null>(null);
  const [selectedBean, setSelectedBean] = createSignal({ id: '', name: '' });
  const [beanSelectorOpen, setBeanSelectorOpen] = createSignal(false);
  const [selectedDataTable, setSelectedDataTable] = createSignal<
    'Finished' | 'Cancelled' | 'Failed'
  >('Finished');

  const beanChanged = (id: string) => {
    setSelectedBean({
      id: id,
      name:
        roasteries
          .roasteries()
          .flatMap((r) => r.beans)
          .find((b) => b.id === id)?.properties.name ?? '',
    });
    setBeanSelectorOpen(false);
  };

  const removeEntry = (id?: string) => {
    if (!id) return;
    setDetailEntry(null);
    historyAccessor.remove(id);
  };

  const dateSort = (a: { timestamp: string }, b: { timestamp: string }) =>
    new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime();

  const selectTable = (): ValidComponent => {
    switch (selectedDataTable()) {
      case 'Finished':
        return () => (
          <FinishedDataTable
            selectedBean={selectedBean().id}
            tableEntries={historyAccessor.finished().sort(dateSort)}
            scales={scalesAccessor.scales()}
            onEntrySelect={(id: string) =>
              setDetailEntry(
                historyAccessor.finished().find((e) => e.id === id) ?? null,
              )
            }
          />
        );
      case 'Cancelled':
        return () => (
          <CancelledDataTable
            selectedBean={selectedBean().id}
            tableEntries={historyAccessor.cancelled().sort(dateSort)}
            scales={scalesAccessor.scales()}
            onEntrySelect={(id: string) =>
              setDetailEntry(
                historyAccessor.cancelled().find((e) => e.id === id) ?? null,
              )
            }
          />
        );
      case 'Failed':
        return () => (
          <FailedDataTable
            selectedBean={selectedBean().id}
            tableEntries={historyAccessor.failed().sort(dateSort)}
            scales={scalesAccessor.scales()}
            onEntrySelect={(id: string) =>
              setDetailEntry(
                historyAccessor.failed().find((e) => e.id === id) ?? null,
              )
            }
          />
        );
    }
  };

  return (
    <div class="no-scrollbar h-full w-full overflow-hidden">
      <AlertDialog open={isLoading()}>
        <AlertDialogContent class="flex items-center justify-center p-8">
          <Spinner class="h-20 w-20" />
        </AlertDialogContent>
      </AlertDialog>
      <BeanSelectorDialog
        isOpen={beanSelectorOpen()}
        onClose={() => setBeanSelectorOpen(false)}
        onBeanSelected={beanChanged}
        roasteries={roasteries.roasteries()}
      />
      <HistoryEntryDetailDialog
        entry={deatilEntry()}
        onClose={() => setDetailEntry(null)}
        onRemove={() => removeEntry(deatilEntry()?.id)}
      />
      <div class="flex h-full flex-col">
        <div class="flex h-10 w-full items-center">
          <Button
            variant="outline"
            class="h-8 w-56 inset-shadow-sm"
            onClick={() => setBeanSelectorOpen(true)}
          >
            {selectedBean().name}
          </Button>
          <div class="flex w-full justify-end">
            <div class="flex h-8 w-72 rounded-lg border inset-shadow-sm">
              <div
                class="z-10 flex w-1/3 items-center justify-center"
                onClick={() => setSelectedDataTable('Finished')}
              >
                <Icon iconName="done_all" />
              </div>
              <div
                class="z-10 flex w-1/3 items-center justify-center"
                onClick={() => setSelectedDataTable('Cancelled')}
              >
                <Icon iconName="clear" />
              </div>
              <div
                class="z-10 flex w-1/3 items-center justify-center"
                onClick={() => setSelectedDataTable('Failed')}
              >
                <Icon iconName="error_outline" />
              </div>
            </div>
            <div class="fixed flex h-8 w-72">
              <div
                class={twMerge(
                  'bg-secondary w-1/3 rounded-lg inset-shadow-sm transition-transform duration-300',
                  selectedDataTable() === 'Finished'
                    ? 'translate-x-0'
                    : selectedDataTable() === 'Cancelled'
                      ? 'translate-x-full'
                      : 'translate-x-[200%]',
                )}
              />
            </div>
          </div>
        </div>
        <div class="flex h-88 w-full flex-col py-4">
          <Dynamic component={selectTable()} />
        </div>
        <div class="flex h-10 w-full items-center justify-end">
          <Button
            class="px-8 shadow-xs"
            variant="outline"
            onClick={() => historyAccessor.cleanup()}
          >
            <Icon iconName="cleaning_services" />
          </Button>
        </div>
      </div>
    </div>
  );
};
