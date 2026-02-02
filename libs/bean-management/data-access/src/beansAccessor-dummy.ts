import { BeanProperties } from './generated/graphql';
import { Accessor, createEffect, createSignal } from 'solid-js';
import { v4 as uuid } from 'uuid';

export { type BeanProperties } from './generated/graphql';

type BeanData = {
  id: string;
  roasteryId: string;
  properties: BeanProperties;
};

type Bean = BeanData & {
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
  const [beans, setBeans] = createSignal<BeanData[]>([]);
  const storage = localStorage['BeansTable'];
  if (storage)
    try {
      setBeans(JSON.parse(storage).beans);
    } catch {
      // Data Format does not match
    }
  createEffect(
    () => (localStorage['BeansTable'] = JSON.stringify({ beans: beans() })),
  );

  return {
    beans: () =>
      beans().map((b) => ({
        id: b.id,
        roasteryId: b.roasteryId,
        properties: b.properties,
        update: (properties: BeanProperties, callback?: () => void) => {
          setBeans((list) =>
            list.map((e) => {
              if (e.id === b.id)
                return {
                  ...e,
                  properties: JSON.parse(JSON.stringify(properties)),
                };
              return e;
            }),
          );
          callback?.();
        },
        remove: (callback?: () => void) => {
          setBeans((list) => list.filter((e) => e.id !== b.id));
          callback?.();
        },
        isUpdating: () => false,
        isDeleting: () => false,
      })) ?? [],
    add: (
      roasteryId: string,
      properties: BeanProperties,
      callback?: () => void,
    ) => {
      setBeans((list) =>
        list.concat([
          {
            id: uuid(),
            properties: JSON.parse(JSON.stringify(properties)),
            roasteryId,
          },
        ]),
      );
      callback?.();
    },
    isLoading: () => false,
  };
};
