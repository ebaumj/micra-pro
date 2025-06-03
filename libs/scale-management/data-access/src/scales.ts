import { createMutation, createQuery } from '@micra-pro/shared/utils-ts';
import {
  AddScaleDocument,
  AddScaleMutation,
  AddScaleMutationVariables,
  RemoveScaleDocument,
  RemoveScaleMutation,
  RemoveScaleMutationVariables,
  RenameScaleDocument,
  RenameScaleMutation,
  RenameScaleMutationVariables,
  ScalesDocument,
  ScalesQuery,
  ScalesQueryVariables,
} from './generated/graphql';
import { Accessor, createSignal } from 'solid-js';

type Scale = {
  id: string;
  name: string;
  rename: (name: string) => void;
  remove: () => void;
  isUpdating: Accessor<boolean>;
  isDeleting: Accessor<boolean>;
};

type ScalesAccessor = {
  scales: Accessor<Scale[]>;
  add: (identifier: string, name: string, callback?: () => void) => void;
  isLoading: Accessor<boolean>;
};

export const createScalesAccessor = (): ScalesAccessor => {
  const [updating, setUpdating] = createSignal<string>('');
  const [deleting, setDeleting] = createSignal<string>('');
  const query = createQuery<ScalesQuery, ScalesQueryVariables>(
    ScalesDocument,
    () => ({}),
  );
  const renameMutation = createMutation<
    RenameScaleMutation,
    RenameScaleMutationVariables
  >(RenameScaleDocument);
  const removeMutation = createMutation<
    RemoveScaleMutation,
    RemoveScaleMutationVariables
  >(RemoveScaleDocument);
  const addMutation = createMutation<
    AddScaleMutation,
    AddScaleMutationVariables
  >(AddScaleDocument);
  const rename = (id: string, name: string) => {
    setUpdating(id);
    renameMutation({ scaleId: id, name: name })
      .then((result) => {
        const index =
          query.resource.latest?.scales.findIndex(
            (s) => s.id === result.renameScale.scale?.id,
          ) ?? -1;
        if (index >= 0 && result.renameScale.scale?.name)
          query.setDataStore(
            'scales',
            index,
            'name',
            result.renameScale.scale.name,
          );
      })
      .finally(() => setUpdating(''));
  };
  const remove = (id: string) => {
    setDeleting(id);
    removeMutation({ scaleId: id })
      .then((result) => {
        if (query.resource.latest && result.removeScale.uuid)
          query.setDataStore(
            'scales',
            query.resource.latest?.scales.filter(
              (s) => s.id !== result.removeScale.uuid,
            ),
          );
      })
      .finally(() => setDeleting(''));
  };
  const add = (identifier: string, name: string, callback?: () => void) => {
    addMutation({ scaleIdentifier: identifier, name: name })
      .then((result) => {
        if (!result.addScale.scale) return;
        const index = query.resource.latest?.scales.length ?? -1;
        if (index >= 0)
          query.setDataStore('scales', index, result.addScale.scale);
        else query.setDataStore('scales', [result.addScale.scale]);
      })
      .finally(() => {
        if (callback) callback();
      });
  };
  return {
    scales: () =>
      query.resource.latest?.scales.map((s) => ({
        id: s.id,
        name: s.name,
        rename: (name: string) => rename(s.id, name),
        remove: () => remove(s.id),
        isUpdating: () => updating() === s.id,
        isDeleting: () => deleting() === s.id,
      })) ?? [],
    add: add,
    isLoading: () => query.resource.state !== 'ready',
  };
};
