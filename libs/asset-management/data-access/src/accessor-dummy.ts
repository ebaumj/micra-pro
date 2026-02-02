import { Accessor } from 'solid-js';
import moment from 'moment';

export type AssetsAccessor = {
  pollAsset: (id: string) => void;
  assets: Accessor<
    {
      id: string;
      path: string;
    }[]
  >;
  create: () => Promise<{ id: string; uploadPath: string }>;
  unfinished: Accessor<{ id: string; uploadPath: string }[]>;
  isPolling: (id: string) => Accessor<boolean>;
};

export const createAssetAccessor = (_: moment.Duration): AssetsAccessor => {
  return {
    assets: () => [],
    create: () =>
      new Promise<{ id: string; uploadPath: string }>((resolve) =>
        resolve({ id: '', uploadPath: 'NOT SUPPORTED' }),
      ),
    unfinished: () => [],
    pollAsset: (_: string) => undefined,
    isPolling: (_: string) => () => false,
  };
};
