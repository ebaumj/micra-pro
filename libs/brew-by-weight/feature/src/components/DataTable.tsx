import { Component, For } from 'solid-js';
import {
  HistoryEntryProcessCancelled,
  HistoryEntryProcessFailed,
  HistoryEntryProcessFinished,
  Spout,
} from '@micra-pro/brew-by-weight/data-access';
import { KeysWithoutProperties, T } from '../generated/language-types';
import moment from 'moment';

const padTwoDigits = (num: number) => {
  return num.toString().padStart(2, '0');
};

const dateFormat = (date: string) => {
  const obj = new Date(date);
  return `${padTwoDigits(obj.getDate())}.${padTwoDigits(obj.getMonth() + 1)}.${obj.getFullYear()} ${padTwoDigits(obj.getHours())}:${padTwoDigits(obj.getMinutes())}`;
};

const spoutTransation = (spout: Spout): KeysWithoutProperties => {
  switch (spout) {
    case Spout.Double:
      return 'spout-double';
    case Spout.Naked:
      return 'spout-naked';
    case Spout.Single:
      return 'spout-single';
  }
};

const timeStampSeconds = (time: string) =>
  moment.duration(time).asSeconds().toFixed(1);

export const FinishedDataTable: Component<{
  selectedBean: string;
  tableEntries: HistoryEntryProcessFinished[];
  onEntrySelect: (id: string) => void;
}> = (props) => {
  return (
    <div class="no-scrollbar flex h-full flex-col overflow-x-scroll rounded-md border text-sm inset-shadow-sm">
      <div class="block h-full w-[200%] overflow-hidden">
        <div class="flex h-8 w-full border-b font-bold">
          <div class="flex min-w-36 items-center border-r px-2">
            <T key="time" />
          </div>
          <div class="flex min-w-28 items-center border-r px-2">
            <T key="spout" />
          </div>
          <div class="flex min-w-28 items-center border-r px-2">
            <T key="flow" />
          </div>
          <div class="flex min-w-28 items-center border-r px-2">
            <T key="target" />
          </div>
          <div class="flex min-w-28 items-center border-r px-2">
            <T key="liquid" />
          </div>
          <div class="flex min-w-28 items-center border-r px-2">
            <T key="target" />
          </div>
          <div class="flex min-w-28 items-center border-r px-2">
            <T key="extraction-time" />
          </div>
          <div class="flex min-w-28 items-center border-r px-2">
            <T key="grind" />
          </div>
          <div class="flex min-w-24 items-center border-r px-2">
            <T key="coffee-quantity" />
          </div>
        </div>
        <div class="no-scrollbar flex h-full w-full flex-col overflow-x-hidden overflow-y-scroll">
          <For
            each={props.tableEntries.filter(
              (e) => e.beanId === props.selectedBean,
            )}
          >
            {(b) => (
              <div
                class="flex min-h-6 border-b"
                onClick={() => props.onEntrySelect(b.id)}
              >
                <div class="flex min-w-36 items-center border-r px-2">
                  {dateFormat(b.timestamp)}
                </div>
                <div class="flex min-w-28 items-center border-r px-2">
                  <T key={spoutTransation(b.spout)} />
                </div>
                <div class="flex min-w-28 items-center justify-center border-r">
                  {b.averageFlow.toFixed(1)} ml/s
                </div>
                <div class="flex min-w-28 items-center justify-center border-r">
                  {b.inCupQuantity.toFixed(1)} g
                </div>
                <div class="flex min-w-28 items-center justify-center border-r">
                  {b.totalQuantity.toFixed(1)} g
                </div>
                <div class="flex min-w-28 items-center justify-center border-r">
                  {timeStampSeconds(b.targetExtractionTime)} s
                </div>
                <div class="flex min-w-28 items-center justify-center border-r">
                  {timeStampSeconds(b.extractionTime)} s
                </div>
                <div class="flex min-w-28 items-center justify-center border-r">
                  {b.grindSetting.toFixed(1)}
                </div>
                <div class="flex min-w-24 items-center justify-center border-r">
                  {b.coffeeQuantity.toFixed(1)} g
                </div>
              </div>
            )}
          </For>
        </div>
      </div>
    </div>
  );
};

export const CancelledDataTable: Component<{
  selectedBean: string;
  tableEntries: HistoryEntryProcessCancelled[];
  onEntrySelect: (id: string) => void;
}> = (props) => {
  return (
    <div class="no-scrollbar flex h-full flex-col overflow-x-scroll rounded-md border text-sm inset-shadow-sm">
      <div class="block h-full w-[200%] overflow-hidden">
        <div class="flex h-8 w-full border-b font-bold">
          <div class="flex min-w-36 items-center border-r px-2">
            <T key="time" />
          </div>
          <div class="flex min-w-28 items-center border-r px-2">
            <T key="spout" />
          </div>
          <div class="flex min-w-28 items-center border-r px-2">
            <T key="flow" />
          </div>
          <div class="flex min-w-28 items-center border-r px-2">
            <T key="target" />
          </div>
          <div class="flex min-w-28 items-center border-r px-2">
            <T key="liquid" />
          </div>
          <div class="flex min-w-28 items-center border-r px-2">
            <T key="target" />
          </div>
          <div class="flex min-w-28 items-center border-r px-2">
            <T key="total-time" />
          </div>
          <div class="flex min-w-28 items-center border-r px-2">
            <T key="grind" />
          </div>
          <div class="flex min-w-24 items-center border-r px-2">
            <T key="coffee-quantity" />
          </div>
        </div>
        <div class="no-scrollbar flex h-full w-full flex-col overflow-x-hidden overflow-y-scroll">
          <For
            each={props.tableEntries.filter(
              (e) => e.beanId === props.selectedBean,
            )}
          >
            {(b) => (
              <div
                class="flex min-h-6 border-b"
                onClick={() => props.onEntrySelect(b.id)}
              >
                <div class="flex min-w-36 items-center border-r px-2">
                  {dateFormat(b.timestamp)}
                </div>
                <div class="flex min-w-28 items-center border-r px-2">
                  <T key={spoutTransation(b.spout)} />
                </div>
                <div class="flex min-w-28 items-center justify-center border-r">
                  {b.averageFlow.toFixed(1)} ml/s
                </div>
                <div class="flex min-w-28 items-center justify-center border-r">
                  {b.inCupQuantity.toFixed(1)} g
                </div>
                <div class="flex min-w-28 items-center justify-center border-r">
                  {b.totalQuantity.toFixed(1)} g
                </div>
                <div class="flex min-w-28 items-center justify-center border-r">
                  {timeStampSeconds(b.targetExtractionTime)} s
                </div>
                <div class="flex min-w-28 items-center justify-center border-r">
                  {timeStampSeconds(b.totalTime)} s
                </div>
                <div class="flex min-w-28 items-center justify-center border-r">
                  {b.grindSetting.toFixed(1)}
                </div>
                <div class="flex min-w-24 items-center justify-center border-r">
                  {b.coffeeQuantity.toFixed(1)} g
                </div>
              </div>
            )}
          </For>
        </div>
      </div>
    </div>
  );
};

export const FailedDataTable: Component<{
  selectedBean: string;
  tableEntries: HistoryEntryProcessFailed[];
  onEntrySelect: (id: string) => void;
}> = (props) => {
  return (
    <div class="no-scrollbar flex h-full flex-col overflow-x-scroll rounded-md border text-sm inset-shadow-sm">
      <div class="block h-full w-[200%] overflow-hidden">
        <div class="flex h-8 w-full border-b font-bold">
          <div class="flex min-w-36 items-center border-r px-2">
            <T key="time" />
          </div>
          <div class="flex min-w-28 items-center border-r px-2">
            <T key="spout" />
          </div>
          <div class="flex min-w-48 items-center border-r px-2">
            <T key="error-type" />
          </div>
          <div class="flex min-w-24 items-center border-r px-2">
            <T key="flow" />
          </div>
          <div class="flex min-w-24 items-center border-r px-2">
            <T key="target" />
          </div>
          <div class="flex min-w-24 items-center border-r px-2">
            <T key="liquid" />
          </div>
          <div class="flex min-w-24 items-center border-r px-2">
            <T key="target" />
          </div>
          <div class="flex min-w-24 items-center border-r px-2">
            <T key="total-time" />
          </div>
          <div class="flex min-w-24 items-center border-r px-2">
            <T key="grind" />
          </div>
        </div>
        <div class="no-scrollbar flex h-full w-full flex-col overflow-x-hidden overflow-y-scroll">
          <For
            each={props.tableEntries.filter(
              (e) => e.beanId === props.selectedBean,
            )}
          >
            {(b) => (
              <div
                class="flex min-h-6 border-b"
                onClick={() => props.onEntrySelect(b.id)}
              >
                <div class="flex min-w-36 items-center border-r px-2">
                  {dateFormat(b.timestamp)}
                </div>
                <div class="flex min-w-28 items-center border-r px-2">
                  <T key={spoutTransation(b.spout)} />
                </div>
                <div class="flex min-w-48 items-center overflow-hidden border-r px-2 whitespace-nowrap">
                  {b.errorType}
                </div>
                <div class="flex min-w-24 items-center justify-center border-r">
                  {b.averageFlow.toFixed(1)} ml/s
                </div>
                <div class="flex min-w-24 items-center justify-center border-r">
                  {b.inCupQuantity.toFixed(1)} g
                </div>
                <div class="flex min-w-24 items-center justify-center border-r">
                  {b.totalQuantity.toFixed(1)} g
                </div>
                <div class="flex min-w-24 items-center justify-center border-r">
                  {timeStampSeconds(b.targetExtractionTime)} s
                </div>
                <div class="flex min-w-24 items-center justify-center border-r">
                  {timeStampSeconds(b.totalTime)} s
                </div>
                <div class="flex min-w-24 items-center justify-center border-r">
                  {b.grindSetting.toFixed(1)}
                </div>
              </div>
            )}
          </For>
        </div>
      </div>
    </div>
  );
};
