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
import { createEffect } from 'solid-js';

export type ConfigKey = 'MainScreenConfig' | 'SelectedScale';

export function readConfig<T>(key: ConfigKey): Promise<T> {
  const query = createQuery<
    ReadConfigurationQuery,
    ReadConfigurationQueryVariables
  >(ReadConfigurationDocument, () => ({ key: key }));
  return new Promise((resolve, reject) => {
    createEffect(() => {
      if (query.resource.state !== 'pending') {
        if (query.resource.state !== 'ready') {
          reject();
          return;
        }
        try {
          resolve(JSON.parse(query.resource.latest.readConfiguration) as T);
        } catch {
          reject();
        }
      }
    });
  });
}

export async function writeConfig<T>(key: ConfigKey, config: T): Promise<T> {
  const mutation = createMutation<
    WriteConfigurationMutation,
    WriteConfigurationMutationVariables
  >(WriteConfigurationDocument);
  const result = await mutation({ key: key, value: JSON.stringify(config) });
  if (result.writeConfiguration.string) return Promise.resolve(config);
  return Promise.reject();
}

export async function removeConfig(key: ConfigKey): Promise<ConfigKey> {
  const mutation = createMutation<
    DeleteConfigurationMutation,
    DeleteConfigurationMutationVariables
  >(DeleteConfigurationDocument);
  const result = await mutation({ key: key });
  if (result.deleteConfiguration.string) return Promise.resolve(key);
  return Promise.reject();
}
