import { createMutation } from '../graphql-client/createMutation';
import { createQuery } from '../graphql-client/createQuery';
import {
  DeleteConfigurationDocument,
  DeleteConfigurationMutation,
  DeleteConfigurationMutationVariables,
  ReadConfigurationDocument,
  ReadConfigurationQuery,
  ReadConfigurationQueryVariables,
  WriteConfigurationDocument,
  WriteConfigurationMutation,
  WriteConfigurationMutationVariables,
} from '../generated/graphql';
import { Accessor, createEffect, createSignal } from 'solid-js';

export type ConfigKey =
  | 'MainScreenConfig'
  | 'Language'
  | 'RecipeHub'
  | 'SelectedSpout'
  | 'CleaningReminder'
  | 'BrewByWeightPannel';

export const createConfigAccessor = <T>(
  key: ConfigKey,
): {
  loading: Accessor<boolean>;
  config: Accessor<T | undefined>;
  writeConfig: (config: T) => void;
  removeConfig: () => void;
} => {
  const [loading, setLoading] = createSignal(true);
  const [config, setConfig] = createSignal<T | undefined>();
  const query = createQuery<
    ReadConfigurationQuery,
    ReadConfigurationQueryVariables
  >(ReadConfigurationDocument, () => ({ key: key }));
  const writeMutation = createMutation<
    WriteConfigurationMutation,
    WriteConfigurationMutationVariables
  >(WriteConfigurationDocument);
  const removeMutation = createMutation<
    DeleteConfigurationMutation,
    DeleteConfigurationMutationVariables
  >(DeleteConfigurationDocument);
  createEffect(() => {
    if (query.resource.state === 'ready') {
      const result = query.resource.latest.readConfiguration;
      setConfig((_) => JSON.parse(result) as T);
    }
    if (query.resource.state !== 'pending') setLoading(false);
  });
  const writeConfig = (config: T) => {
    writeMutation({ key: key, value: JSON.stringify(config) }).then((c) => {
      const result = c.writeConfiguration.string;
      if (result) setConfig((_) => JSON.parse(result) as T);
    });
  };
  const removeConfig = () => {
    removeMutation({ key: key }).then((c) => {
      const result = c.deleteConfiguration.string;
      if (result === key) setConfig(undefined);
    });
  };
  return {
    loading,
    config,
    writeConfig,
    removeConfig,
  };
};
