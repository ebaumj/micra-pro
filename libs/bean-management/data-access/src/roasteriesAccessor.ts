import { createMutation, createQuery } from '@micra-pro/shared/utils-ts';
import {
  AddRoasteryDocument,
  AddRoasteryMutation,
  AddRoasteryMutationVariables,
  RemoveRoasteryDocument,
  RemoveRoasteryMutation,
  RemoveRoasteryMutationVariables,
  RoasteriesDocument,
  RoasteriesQuery,
  RoasteriesQueryVariables,
  RoasteryProperties,
  UpdateRoasteryDocument,
  UpdateRoasteryMutation,
  UpdateRoasteryMutationVariables,
} from './generated/graphql';
import { Accessor, createSignal } from 'solid-js';

export { type RoasteryProperties } from './generated/graphql';

type Roastery = {
  id: string;
  properties: RoasteryProperties;
  update: (properties: RoasteryProperties, callback?: () => void) => void;
  remove: (callback?: () => void) => void;
  isUpdating: Accessor<boolean>;
  isDeleting: Accessor<boolean>;
};

type RoasteriesAccessor = {
  roasteries: Accessor<Roastery[]>;
  add: (properties: RoasteryProperties, callback?: () => void) => void;
  isLoading: Accessor<boolean>;
};

export const createRoasteriesAccessor = (): RoasteriesAccessor => {
  const [updating, setUpdating] = createSignal<string>('');
  const [deleting, setDeleting] = createSignal<string>('');
  const query = createQuery<RoasteriesQuery, RoasteriesQueryVariables>(
    RoasteriesDocument,
    () => ({}),
  );
  const updateMutation = createMutation<
    UpdateRoasteryMutation,
    UpdateRoasteryMutationVariables
  >(UpdateRoasteryDocument);
  const removeMutation = createMutation<
    RemoveRoasteryMutation,
    RemoveRoasteryMutationVariables
  >(RemoveRoasteryDocument);
  const addMutation = createMutation<
    AddRoasteryMutation,
    AddRoasteryMutationVariables
  >(AddRoasteryDocument);
  const update = (
    id: string,
    properties: RoasteryProperties,
    callback?: () => void,
  ) => {
    setUpdating(id);
    updateMutation({ roasteryId: id, properties: properties })
      .then((result) => {
        const index =
          query.resource.latest?.roasteries.findIndex(
            (r) => r.id === result.updateRoastery.roastery?.id,
          ) ?? -1;
        if (index >= 0 && result.updateRoastery.roastery?.properties)
          query.setDataStore(
            'roasteries',
            index,
            'properties',
            result.updateRoastery.roastery.properties,
          );
      })
      .finally(() => {
        setUpdating('');
        callback?.();
      });
  };
  const remove = (id: string, callback?: () => void) => {
    setDeleting(id);
    removeMutation({ roasteryId: id })
      .then((result) => {
        if (query.resource.latest && result.removeRoastery.uuid)
          query.setDataStore(
            'roasteries',
            query.resource.latest?.roasteries.filter(
              (r) => r.id !== result.removeRoastery.uuid,
            ),
          );
      })
      .finally(() => {
        setDeleting('');
        callback?.();
      });
  };
  const add = (properties: RoasteryProperties, callback?: () => void) => {
    addMutation({ properties: properties })
      .then((result) => {
        if (!result.addRoastery.roastery) return;
        const index = query.resource.latest?.roasteries.length ?? -1;
        if (index >= 0)
          query.setDataStore('roasteries', index, result.addRoastery.roastery);
        else query.setDataStore('roasteries', [result.addRoastery.roastery]);
      })
      .finally(() => {
        if (callback) callback();
      });
  };
  return {
    roasteries: () =>
      query.resource.latest?.roasteries.map((r) => ({
        id: r.id,
        properties: r.properties,
        update: (properties: RoasteryProperties, callback?: () => void) =>
          update(r.id, properties, callback),
        remove: (callback?: () => void) => remove(r.id, callback),
        isUpdating: () => updating() === r.id,
        isDeleting: () => deleting() === r.id,
      })) ?? [],
    add: add,
    isLoading: () => query.resource.state !== 'ready',
  };
};
