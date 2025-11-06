import { Component, createSignal } from 'solid-js';
import { twMerge } from 'tailwind-merge';
import { T, useTranslationContext } from '../generated/language-types';
import { fetchRoasteriesLevel } from '@micra-pro/bean-management/data-access';
import { createHistoryAccessor } from '@micra-pro/brew-by-weight/data-access';
import { createScalesAccessor } from '@micra-pro/scale-management/data-access';
import {
  AlertDialog,
  AlertDialogContent,
  PieChart,
  Spinner,
} from '@micra-pro/shared/ui';

export const BrewByWeightStatisticsPage: Component = () => {
  const { t } = useTranslationContext();
  const roasteries = fetchRoasteriesLevel();
  const historyAccessor = createHistoryAccessor();
  const scalesAccessor = createScalesAccessor();
  const isLoading = () =>
    roasteries.isLoading() ||
    historyAccessor.initalLoad() ||
    scalesAccessor.isLoading();
  const [selectedDataTable, setSelectedDataTable] = createSignal<
    'Roasteries' | 'Beans' | 'Scales' | 'Result'
  >('Roasteries');

  const total = (): number => {
    switch (selectedDataTable()) {
      case 'Roasteries':
      case 'Beans':
      case 'Scales':
        return historyAccessor.finished().length;
      case 'Result':
        return (
          historyAccessor.finished().length +
          historyAccessor.cancelled().length +
          historyAccessor.failed().length
        );
    }
  };

  const data = (): { label: string; value: number }[] => {
    switch (selectedDataTable()) {
      case 'Roasteries':
        return roasteries
          .roasteries()
          .map((r) => ({
            label: r.properties.name,
            value: historyAccessor
              .finished()
              .filter((e) => r.beans.find((b) => b.id === e.beanId)).length,
          }))
          .filter((d) => d.value !== 0);
      case 'Beans':
        return roasteries
          .roasteries()
          .flatMap((r) => r.beans)
          .map((b) => ({
            label: b.properties.name,
            value: historyAccessor.finished().filter((e) => e.beanId === b.id)
              .length,
          }))
          .filter((d) => d.value !== 0);
      case 'Scales':
        return scalesAccessor
          .scales()
          .map((s) => ({
            label: s.name,
            value: historyAccessor.finished().filter((e) => e.scaleId === s.id)
              .length,
          }))
          .filter((d) => d.value !== 0);
      case 'Result':
        return [
          { label: t('finished'), value: historyAccessor.finished().length },
          { label: t('cancelled'), value: historyAccessor.cancelled().length },
          { label: t('failed'), value: historyAccessor.failed().length },
        ].filter((d) => d.value !== 0);
    }
  };

  return (
    <div class="no-scrollbar flex h-full w-full flex-col gap-4 overflow-hidden py-4">
      <AlertDialog open={isLoading()}>
        <AlertDialogContent class="flex items-center justify-center p-8">
          <Spinner class="h-20 w-20" />
        </AlertDialogContent>
      </AlertDialog>
      <div class="flex h-12 w-full items-center justify-center">
        <div class="flex w-96 justify-end">
          <div class="flex h-8 w-96 rounded-lg border inset-shadow-sm">
            <div
              class="z-10 flex w-1/4 items-center justify-center"
              onClick={() => setSelectedDataTable('Roasteries')}
            >
              <T key="roasteries" />
            </div>
            <div
              class="z-10 flex w-1/4 items-center justify-center"
              onClick={() => setSelectedDataTable('Beans')}
            >
              <T key="beans" />
            </div>
            <div
              class="z-10 flex w-1/4 items-center justify-center"
              onClick={() => setSelectedDataTable('Scales')}
            >
              <T key="scales" />
            </div>
            <div
              class="z-10 flex w-1/4 items-center justify-center"
              onClick={() => setSelectedDataTable('Result')}
            >
              <T key="result" />
            </div>
          </div>
          <div class="fixed flex h-8 w-96">
            <div
              class={twMerge(
                'bg-secondary w-1/4 rounded-lg inset-shadow-sm transition-transform duration-300',
                selectedDataTable() === 'Roasteries'
                  ? 'translate-x-0'
                  : selectedDataTable() === 'Beans'
                    ? 'translate-x-full'
                    : selectedDataTable() === 'Scales'
                      ? 'translate-x-[200%]'
                      : 'translate-x-[300%]',
              )}
            />
          </div>
        </div>
      </div>
      <div class="flex h-full w-full items-center justify-center">
        <div class="h-64 w-64">
          <PieChart
            options={{
              plugins: {
                tooltip: {
                  enabled: false,
                },
                legend: {
                  display: true,
                  align: 'center',
                  position: 'bottom',
                },
              },
            }}
            data={{
              labels: data().map((d) => `${d.label}: ${d.value}`),
              datasets: [
                {
                  data: data().map((d) => d.value),
                },
              ],
            }}
          />
        </div>
      </div>
      <div class="flex w-full items-center justify-center">
        <div class="rounded-xl border px-4 py-1 inset-shadow-sm">
          <T key="total" />: {total()}
        </div>
      </div>
    </div>
  );
};
