import { createMutation, createQuery } from '@micra-pro/shared/utils-ts';
import {
  AddBeanDocument,
  AddBeanMutation,
  AddBeanMutationVariables,
  BeanProperties,
  BeansDocument,
  BeansQuery,
  BeansQueryVariables,
  RemoveBeanDocument,
  RemoveBeanMutation,
  RemoveBeanMutationVariables,
  UpdateBeanDocument,
  UpdateBeanMutation,
  UpdateBeanMutationVariables,
} from './generated/graphql';
import { Accessor, createSignal } from 'solid-js';

export { type BeanProperties } from './generated/graphql';

type Bean = {
  id: string;
  roasteryId: string;
  properties: BeanProperties;
  update: (properties: BeanProperties, callback?: () => void) => void;
  remove: (callback?: () => void) => void;
  isUpdating: Accessor<boolean>;
  isDeleting: Accessor<boolean>;
};

type BeansAccessor = {
  beans: Accessor<Bean[]>;
  add: (
    roasteryId: string,
    properties: BeanProperties,
    callback?: () => void,
  ) => void;
  isLoading: Accessor<boolean>;
};

export const createBeansAccessor = (): BeansAccessor => {
  const [updating, setUpdating] = createSignal<string>('');
  const [deleting, setDeleting] = createSignal<string>('');
  const query = createQuery<BeansQuery, BeansQueryVariables>(
    BeansDocument,
    () => ({}),
  );
  const updateMutation = createMutation<
    UpdateBeanMutation,
    UpdateBeanMutationVariables
  >(UpdateBeanDocument);
  const removeMutation = createMutation<
    RemoveBeanMutation,
    RemoveBeanMutationVariables
  >(RemoveBeanDocument);
  const addMutation = createMutation<AddBeanMutation, AddBeanMutationVariables>(
    AddBeanDocument,
  );
  const update = (
    id: string,
    properties: BeanProperties,
    callback?: () => void,
  ) => {
    setUpdating(id);
    updateMutation({ beanId: id, properties: properties })
      .then((result) => {
        const index =
          query.resource.latest?.beans.findIndex(
            (b) => b.id === result.updateBean.bean?.id,
          ) ?? -1;
        if (index >= 0 && result.updateBean.bean?.properties)
          query.setDataStore(
            'beans',
            index,
            'properties',
            result.updateBean.bean.properties,
          );
      })
      .finally(() => {
        setUpdating('');
        callback?.();
      });
  };
  const remove = (id: string, callback?: () => void) => {
    setDeleting(id);
    removeMutation({ beanId: id })
      .then((result) => {
        if (query.resource.latest && result.removeBean.uuid)
          query.setDataStore(
            'beans',
            query.resource.latest?.beans.filter(
              (b) => b.id !== result.removeBean.uuid,
            ),
          );
      })
      .finally(() => {
        setDeleting('');
        callback?.();
      });
  };
  const add = (
    roasteryId: string,
    properties: BeanProperties,
    callback?: () => void,
  ) => {
    addMutation({ roasteryId: roasteryId, properties: properties })
      .then((result) => {
        if (!result.addBean.bean) return;
        const index = query.resource.latest?.beans.length ?? -1;
        if (index >= 0) query.setDataStore('beans', index, result.addBean.bean);
        else query.setDataStore('beans', [result.addBean.bean]);
      })
      .finally(() => {
        if (callback) callback();
      });
  };
  return {
    beans: () =>
      query.resource.latest?.beans.map((b) => ({
        id: b.id,
        roasteryId: b.roasteryId,
        properties: b.properties,
        update: (properties: BeanProperties, callback?: () => void) =>
          update(b.id, properties, callback),
        remove: (callback?: () => void) => remove(b.id, callback),
        isUpdating: () => updating() === b.id,
        isDeleting: () => deleting() === b.id,
      })) ?? [],
    add: add,
    isLoading: () => query.resource.state !== 'ready',
  };
};
