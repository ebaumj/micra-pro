import { EspressoFieldsFragment, V60FieldsFragment } from './generated/graphql';
import { Accessor, createEffect, createSignal } from 'solid-js';
import { v4 as uuid } from 'uuid';

export {
  type Espresso as EspressoProperties,
  type V60 as V60Properties,
} from './generated/graphql';

type EspressoData = {
  id: string;
  beanId: string;
  properties: EspressoFieldsFragment;
};

type EspressoRecipe = EspressoData & {
  update: (properties: EspressoFieldsFragment, callback?: () => void) => void;
  remove: (callback?: () => void) => void;
  isUpdating: Accessor<boolean>;
  isDeleting: Accessor<boolean>;
};

type V60Data = {
  id: string;
  beanId: string;
  properties: V60FieldsFragment;
};

type V60Recipe = V60Data & {
  update: (properties: V60FieldsFragment, callback?: () => void) => void;
  remove: (callback?: () => void) => void;
  isUpdating: Accessor<boolean>;
  isDeleting: Accessor<boolean>;
};

type RecipesAccessor = {
  recipesEspresso: Accessor<EspressoRecipe[]>;
  recipesV60: Accessor<V60Recipe[]>;
  addEspresso: (
    beanId: string,
    properties: EspressoFieldsFragment,
    callback?: () => void,
  ) => void;
  addV60: (
    beanId: string,
    properties: V60FieldsFragment,
    callback?: () => void,
  ) => void;
  isLoading: Accessor<boolean>;
};

export const createRecipesAccessor = (): RecipesAccessor => {
  const [espresso, setEspresso] = createSignal<EspressoData[]>([]);
  const [v60, setV60] = createSignal<V60Data[]>([]);
  const espressoStorage = localStorage['EspressoTable'];
  const v60Storage = localStorage['V60Table'];
  if (espressoStorage)
    try {
      setEspresso(JSON.parse(espressoStorage).recipes);
    } catch {
      // Data Format does not match
    }
  if (v60Storage)
    try {
      setV60(JSON.parse(v60Storage).recipes);
    } catch {
      // Data Format does not match
    }
  createEffect(
    () =>
      (localStorage['EspressoTable'] = JSON.stringify({ recipes: espresso() })),
  );
  createEffect(
    () => (localStorage['V60Table'] = JSON.stringify({ recipes: v60() })),
  );

  return {
    recipesEspresso: () =>
      espresso().map((r) => ({
        id: r.id,
        beanId: r.beanId,
        properties: r.properties,
        update: (properties: EspressoFieldsFragment, callback?: () => void) => {
          setEspresso((list) =>
            list.map((e) => {
              if (e.id === r.id)
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
          setEspresso((list) => list.filter((e) => e.id !== r.id));
          callback?.();
        },
        isUpdating: () => false,
        isDeleting: () => false,
      })) ?? [],
    recipesV60: () =>
      v60().map((r) => ({
        id: r.id,
        beanId: r.beanId,
        properties: r.properties,
        update: (properties: V60FieldsFragment, callback?: () => void) => {
          setV60((list) =>
            list.map((e) => {
              if (e.id === r.id)
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
          setV60((list) => list.filter((e) => e.id !== r.id));
          callback?.();
        },
        isUpdating: () => false,
        isDeleting: () => false,
      })) ?? [],
    addEspresso: (
      beanId: string,
      properties: EspressoFieldsFragment,
      callback?: () => void,
    ) => {
      setEspresso((list) =>
        list.concat([
          {
            id: uuid(),
            properties: JSON.parse(JSON.stringify(properties)),
            beanId,
          },
        ]),
      );
      callback?.();
    },
    addV60: (
      beanId: string,
      properties: V60FieldsFragment,
      callback?: () => void,
    ) => {
      setV60((list) =>
        list.concat([
          {
            id: uuid(),
            properties: JSON.parse(JSON.stringify(properties)),
            beanId,
          },
        ]),
      );
      callback?.();
    },
    isLoading: () => false,
  };
};
