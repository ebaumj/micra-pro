import { createMutation, createQuery } from '@micra-pro/shared/utils-ts';
import {
  AddOrUpdateScaleDocument,
  AddOrUpdateScaleMutation,
  AddOrUpdateScaleMutationVariables,
  RemoveScaleDocument,
  RemoveScaleMutation,
  RemoveScaleMutationVariables,
  ScaleDocument,
  ScaleQuery,
  ScaleQueryVariables,
} from './generated/graphql';
import { Accessor, createSignal } from 'solid-js';

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
  const [deleting, setDeleting] = createSignal(false);
  const query = createQuery<ScaleQuery, ScaleQueryVariables>(
    ScaleDocument,
    () => ({}),
  );
  const removeMutation = createMutation<
    RemoveScaleMutation,
    RemoveScaleMutationVariables
  >(RemoveScaleDocument);
  const addMutation = createMutation<
    AddOrUpdateScaleMutation,
    AddOrUpdateScaleMutationVariables
  >(AddOrUpdateScaleDocument);
  const remove = () => {
    setDeleting(true);
    removeMutation({})
      .then((result) => {
        if (query.resource.latest && result.removeScale.boolean)
          query.setDataStore('scale', false);
      })
      .finally(() => setDeleting(false));
  };
  const add = (
    identifier: string,
    callback?: () => void,
    error?: () => void,
  ) => {
    addMutation({ scaleIdentifier: identifier })
      .then((result) => {
        if (!result.addOrUpdateScale.boolean) return;
        query.setDataStore('scale', result.addOrUpdateScale.boolean);
      })
      .catch(() => error?.())
      .finally(() => {
        if (callback) callback();
      });
  };
  return {
    scale: () =>
      query.resource.latest?.scale
        ? {
            isDeleting: () => deleting(),
            remove: remove,
          }
        : undefined,
    isLoading: () => query.resource.state !== 'ready',
    add,
  };
};
