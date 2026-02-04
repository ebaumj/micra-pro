import { Accessor, createEffect, createSignal } from 'solid-js';

export type ConfigKey =
  | 'MainScreenConfig'
  | 'Language'
  | 'RecipeHub'
  | 'SelectedSpout'
  | 'CleaningReminder'
  | 'BrewByWeightPannel'
  | 'NumberPickerStyle';

export const createConfigAccessor = <T>(
  key: ConfigKey,
): {
  loading: Accessor<boolean>;
  config: Accessor<T | undefined>;
  writeConfig: (config: T) => Promise<any>;
  removeConfig: () => Promise<any>;
} => {
  const [config, setConfig] = createSignal<T | undefined>();
  const [loading, setLoading] = createSignal(true);
  const storage = localStorage['Config' + key];
  if (storage)
    try {
      setConfig(JSON.parse(storage).config);
    } catch {
      // Data does not match
    }
  createEffect(
    () => (localStorage['Config' + key] = JSON.stringify({ config: config() })),
  );
  setTimeout(() => setLoading(false), 20);
  return {
    loading,
    config,
    writeConfig: (config: T) => {
      setConfig((_) => config);
      return Promise.resolve();
    },
    removeConfig: () => {
      setConfig(undefined);
      return Promise.resolve();
    },
  };
};
