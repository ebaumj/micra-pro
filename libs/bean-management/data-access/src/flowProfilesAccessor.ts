import { createMutation, createQuery } from '@micra-pro/shared/utils-ts';
import {
  AddFlowProfileDocument,
  AddFlowProfileMutation,
  AddFlowProfileMutationVariables,
  FlowProfileProperties,
  FlowProfilesDocument,
  FlowProfilesQuery,
  FlowProfilesQueryVariables,
  RemoveFlowProfileDocument,
  RemoveFlowProfileMutation,
  RemoveFlowProfileMutationVariables,
  UpdateFlowProfileDocument,
  UpdateFlowProfileMutation,
  UpdateFlowProfileMutationVariables,
} from './generated/graphql';
import { Accessor, createSignal } from 'solid-js';

export { type FlowProfileProperties } from './generated/graphql';

type FlowProfile = {
  id: string;
  recipeId: string;
  properties: FlowProfileProperties;
  update: (properties: FlowProfileProperties, callback?: () => void) => void;
  remove: (callback?: () => void) => void;
  isUpdating: Accessor<boolean>;
  isDeleting: Accessor<boolean>;
};

type FlowProfilesAccessor = {
  flowProfiles: Accessor<FlowProfile[]>;
  add: (
    recipeId: string,
    properties: FlowProfileProperties,
    callback?: () => void,
  ) => void;
  isLoading: Accessor<boolean>;
};

export const createFlowProfilesAccessor = (): FlowProfilesAccessor => {
  const [updating, setUpdating] = createSignal<string>('');
  const [deleting, setDeleting] = createSignal<string>('');
  const query = createQuery<FlowProfilesQuery, FlowProfilesQueryVariables>(
    FlowProfilesDocument,
    () => ({}),
  );
  const updateMutation = createMutation<
    UpdateFlowProfileMutation,
    UpdateFlowProfileMutationVariables
  >(UpdateFlowProfileDocument);
  const removeMutation = createMutation<
    RemoveFlowProfileMutation,
    RemoveFlowProfileMutationVariables
  >(RemoveFlowProfileDocument);
  const addMutation = createMutation<
    AddFlowProfileMutation,
    AddFlowProfileMutationVariables
  >(AddFlowProfileDocument);
  const update = (
    id: string,
    properties: FlowProfileProperties,
    callback?: () => void,
  ) => {
    setUpdating(id);
    updateMutation({ profileId: id, properties: properties })
      .then((result) => {
        const index =
          query.resource.latest?.flowProfiles.findIndex(
            (b) => b.id === result.updateFlowProfile.flowProfile?.id,
          ) ?? -1;
        if (index >= 0 && result.updateFlowProfile.flowProfile?.properties)
          query.setDataStore(
            'flowProfiles',
            index,
            'properties',
            result.updateFlowProfile.flowProfile.properties,
          );
      })
      .finally(() => {
        setUpdating('');
        callback?.();
      });
  };
  const remove = (id: string, callback?: () => void) => {
    setDeleting(id);
    removeMutation({ profileId: id })
      .then((result) => {
        if (query.resource.latest && result.removeFlowProfile.uuid)
          query.setDataStore(
            'flowProfiles',
            query.resource.latest?.flowProfiles.filter(
              (b) => b.id !== result.removeFlowProfile.uuid,
            ),
          );
      })
      .finally(() => {
        setDeleting('');
        callback?.();
      });
  };
  const add = (
    recipeId: string,
    properties: FlowProfileProperties,
    callback?: () => void,
  ) => {
    addMutation({ recipeId: recipeId, properties: properties })
      .then((result) => {
        if (!result.addFlowProfile.flowProfile) return;
        const index = query.resource.latest?.flowProfiles.length ?? -1;
        if (index >= 0)
          query.setDataStore(
            'flowProfiles',
            index,
            result.addFlowProfile.flowProfile,
          );
        else
          query.setDataStore('flowProfiles', [
            result.addFlowProfile.flowProfile,
          ]);
      })
      .finally(() => {
        if (callback) callback();
      });
  };
  return {
    flowProfiles: () =>
      query.resource.latest?.flowProfiles.map((b) => ({
        id: b.id,
        recipeId: b.recipeId,
        properties: b.properties,
        update: (properties: FlowProfileProperties, callback?: () => void) =>
          update(b.id, properties, callback),
        remove: (callback?: () => void) => remove(b.id, callback),
        isUpdating: () => updating() === b.id,
        isDeleting: () => deleting() === b.id,
      })) ?? [],
    add: add,
    isLoading: () => query.resource.state !== 'ready',
  };
};
