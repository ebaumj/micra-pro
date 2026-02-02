export {
  type BrewByWeightHistoryFieldsFragment as BrewByWeightHistoryEntry,
  type HistoryEntryProcessFinished,
  type HistoryEntryProcessCancelled,
  type HistoryEntryProcessFailed,
} from './generated/graphql';

import { createEffect, createSignal } from 'solid-js';
import {
  type BrewByWeightHistoryFieldsFragment as BrewByWeightHistoryEntry,
  type HistoryEntryProcessFinished,
  type HistoryEntryProcessCancelled,
  type HistoryEntryProcessFailed,
} from './generated/graphql';
import { v4 as uuid } from 'uuid';

export const addHistoryFinishedInternal = (
  value: Omit<HistoryEntryProcessFinished, 'id' | 'timestamp'>,
) => {
  const data: HistoryEntryProcessFinished & BrewByWeightHistoryEntry = {
    ...value,
    __typename: 'HistoryEntryProcessFinished',
    timestamp: new Date().toISOString(),
    id: uuid(),
  };
  let items = [] as (HistoryEntryProcessFinished & BrewByWeightHistoryEntry)[];
  try {
    items = JSON.parse(localStorage['FinishedHistoryTable']).entries;
  } catch {
    // Data does not match
  }
  localStorage['FinishedHistoryTable'] = JSON.stringify({
    entries: items.concat([data]),
  });
};

export const addHistoryCancelledInternal = (
  value: Omit<HistoryEntryProcessCancelled, 'id' | 'timestamp'>,
) => {
  const data: HistoryEntryProcessCancelled & BrewByWeightHistoryEntry = {
    ...value,
    __typename: 'HistoryEntryProcessCancelled',
    timestamp: new Date().toISOString(),
    id: uuid(),
  };
  let items = [] as (HistoryEntryProcessCancelled & BrewByWeightHistoryEntry)[];
  try {
    items = JSON.parse(localStorage['CancelledHistoryTable']).entries;
  } catch {
    // Data does not match
  }
  localStorage['CancelledHistoryTable'] = JSON.stringify({
    entries: items.concat([data]),
  });
};

export const createHistoryAccessor = () => {
  const [finished, setFinished] = createSignal<
    (HistoryEntryProcessFinished & BrewByWeightHistoryEntry)[]
  >([]);
  const finishedStorage = localStorage['FinishedHistoryTable'];
  if (finishedStorage) {
    try {
      setFinished(JSON.parse(finishedStorage).entries);
    } catch {
      // Data does not match
    }
  }
  createEffect(
    () =>
      (localStorage['FinishedHistoryTable'] = JSON.stringify({
        entries: finished(),
      })),
  );
  const [cancelled, setCancelled] = createSignal<
    (HistoryEntryProcessCancelled & BrewByWeightHistoryEntry)[]
  >([]);
  const cancelledStorage = localStorage['CancelledHistoryTable'];
  if (cancelledStorage) {
    try {
      setCancelled(JSON.parse(cancelledStorage).entries);
    } catch {
      // Data does not match
    }
  }
  createEffect(
    () =>
      (localStorage['CancelledHistoryTable'] = JSON.stringify({
        entries: cancelled(),
      })),
  );
  return {
    finished: finished,
    cancelled: cancelled,
    failed: () =>
      [] as (HistoryEntryProcessFailed & BrewByWeightHistoryEntry)[],
    cleanup: () => {
      setFinished([]);
      setCancelled([]);
      return Promise.resolve();
    },
    remove: (id: string) => {
      setFinished((l) => l.filter((e) => e.id !== id));
      setCancelled((l) => l.filter((e) => e.id !== id));
      return Promise.resolve();
    },
    initalLoad: () => false,
  };
};
