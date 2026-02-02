import { Accessor, createEffect, createSignal } from 'solid-js';

type Scale = {
  remove: () => void;
  isDeleting: Accessor<boolean>;
};

type ScalesAccessor = {
  scale: Accessor<Scale | undefined>;
  add: (identifier: string, callback?: () => void, error?: () => void) => void;
  isLoading: Accessor<boolean>;
};

export const createScalesAccessor = (): ScalesAccessor => {
  const [scale, setScale] = createSignal(false);
  const storage = localStorage['Scale'];
  if (storage) setScale(storage);
  createEffect(() => (localStorage['Scale'] = scale()));
  return {
    scale: () =>
      scale()
        ? {
            remove: () => setScale(false),
            isDeleting: () => false,
          }
        : undefined,
    isLoading: () => false,
    add: (_: string, callback?: () => void, _1?: () => void) => {
      setScale(true);
      callback?.();
    },
  };
};
