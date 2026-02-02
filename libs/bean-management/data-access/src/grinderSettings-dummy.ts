import { Accessor, createEffect, createSignal } from 'solid-js';

type Setting<T> = {
  value: Accessor<T>;
  setValue: (newValue: T) => void;
};

export const getGrinderSettingsAccessor = (): {
  offset: Setting<number>;
} => {
  const [value, setValue] = createSignal(0);
  const storage = localStorage['GrinderSettings'];
  if (storage)
    try {
      setValue(JSON.parse(storage).offset);
    } catch {
      // Data Format does not match
    }
  createEffect(
    () =>
      (localStorage['GrinderSettings'] = JSON.stringify({ offset: value() })),
  );
  return {
    offset: {
      value,
      setValue,
    },
  };
};
