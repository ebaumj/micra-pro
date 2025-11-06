import { BrewByWeightHistoryEntry } from '@micra-pro/brew-by-weight/data-access';
import {
  Button,
  Dialog,
  DialogContent,
  Icon,
  LineChart,
  twColor,
} from '@micra-pro/shared/ui';
import moment from 'moment';
import { Component, Show } from 'solid-js';
import { T, useTranslationContext } from '../generated/language-types';

export const HistoryEntryDetailDialog: Component<{
  onClose: () => void;
  onRemove: () => void;
  entry: BrewByWeightHistoryEntry | null;
}> = (props) => {
  const { t } = useTranslationContext();
  const iconName = (): string => {
    switch (props.entry?.__typename) {
      case 'HistoryEntryProcessFinished':
        return 'done_all';
      case 'HistoryEntryProcessCancelled':
        return 'clear';
      case 'HistoryEntryProcessFailed':
        return 'error_outline';
      case undefined:
        return '';
    }
  };
  const totalTime = () => {
    switch (props.entry?.__typename) {
      case 'HistoryEntryProcessFinished':
        return moment
          .duration(
            props.entry.runtimeData[props.entry.runtimeData.length - 1]
              .totalTime,
          )
          .asSeconds();
      case 'HistoryEntryProcessCancelled':
        return moment.duration(props.entry.totalTime).asSeconds();
      case 'HistoryEntryProcessFailed':
        return moment.duration(props.entry.totalTime).asSeconds();
      case undefined:
        return 0;
    }
  };
  return (
    <Dialog
      open={!!props.entry}
      onOpenChange={(o) => (o ? undefined : props.onClose())}
    >
      <DialogContent
        onOpenAutoFocus={(e) => e.preventDefault()}
        onInteractOutside={(e) => e.preventDefault()}
      >
        <div class="flex flex-col gap-2 px-6">
          <div class="flex h-8 w-full items-center justify-start gap-8">
            <div class="rounded-lg px-2 py-1 inset-shadow-sm">
              <Icon iconName={iconName()} />
            </div>
            <div class="flex w-full justify-end pr-8">
              <div class="rounded-lg px-2 py-1 inset-shadow-sm">
                <div class="flex items-center gap-4">
                  <Icon iconName="access_time" />
                  <span>{totalTime().toFixed(1)} s</span>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="h-64 w-full">
          <Show when={props.entry?.runtimeData}>
            {(data) => (
              <Show when={data().length > 2}>
                <div class="bg-primary text-primary-foreground h-full w-full -translate-x-4 gap-2 rounded-md p-4 pb-10 shadow-xl">
                  <LineChart
                    data={{
                      labels: data().map((d) =>
                        moment.duration(d.totalTime).asSeconds(),
                      ),
                      datasets: [
                        {
                          data: data().map((d) => d.flow),
                          label: t('flow'),
                          pointStyle: false,
                          animation: false,
                          borderColor: twColor('accent'),
                        },
                        {
                          data: data().map((d) => d.totalQuantity),
                          label: t('liquid'),
                          pointStyle: false,
                          animation: false,
                          borderColor: twColor('accent-variant'),
                          yAxisID: 'y2',
                        },
                      ],
                    }}
                    options={{
                      maintainAspectRatio: false,
                      scales: {
                        x: {
                          display: false,
                          grid: {
                            display: false,
                          },
                        },
                        y: {
                          display: true,
                          ticks: {
                            color: twColor('accent'),
                          },
                          min: 0,
                          max: Math.max(...data().map((d) => d.flow)) * 1.1,
                          grid: {
                            display: false,
                          },
                        },
                        y2: {
                          axis: 'y',
                          display: true,
                          ticks: {
                            color: twColor('accent-variant'),
                          },
                          position: 'right',
                          min: 0,
                          max:
                            Math.max(...data().map((d) => d.totalQuantity)) *
                            1.1,
                          grid: {
                            display: false,
                          },
                        },
                      },
                      plugins: {
                        tooltip: {
                          enabled: false,
                        },
                        legend: {
                          display: false,
                        },
                      },
                      elements: {
                        line: {
                          borderWidth: 1,
                        },
                      },
                    }}
                  />
                  <div class="flex h-10 items-center justify-center gap-8">
                    <div class="text-accent">
                      <T key="flow" />
                    </div>
                    <div class="text-accent-variant">
                      <T key="liquid" />
                    </div>
                  </div>
                </div>
              </Show>
            )}
          </Show>
          <Show when={(props.entry?.runtimeData?.length ?? 0) <= 2}>
            <div class="flex h-full w-full items-center justify-center text-2xl opacity-50">
              <Icon iconName="broken_image" />
            </div>
          </Show>
        </div>
        <div class="flex flex-col gap-2 px-6">
          <div class="flex h-12 w-full items-center justify-center">
            <Button
              variant="destructive"
              class="w-36"
              onClick={() => props.onRemove()}
            >
              <Icon iconName="delete" />
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};
