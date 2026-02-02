import {
  type Bean as BeanApi,
  type Roastery as RoasteryApi,
  EspressoFieldsFragment,
  V60FieldsFragment,
  RoasteryProperties,
  BeanProperties,
  RecipePropertiesFieldsFragment,
} from './generated/graphql';
import { Accessor } from 'solid-js';

type EspressoData = {
  id: string;
  beanId: string;
  properties: EspressoFieldsFragment;
};

type V60Data = {
  id: string;
  beanId: string;
  properties: V60FieldsFragment;
};

export type Roastery = {
  properties: RoasteryProperties;
  beans: {
    id: string;
    properties: BeanProperties;
    recipes: RecipePropertiesFieldsFragment[];
  }[];
};

export type Bean = {
  id: string;
  properties: BeanProperties;
  roastery: RoasteryProperties;
  recipes: RecipePropertiesFieldsFragment[];
};

export const fetchBeansLevel = (): {
  beans: Accessor<Bean[]>;
  isLoading: Accessor<boolean>;
} => {
  const query = fetch();
  return {
    isLoading: () => false,
    beans: () =>
      query.beans
        .filter((b) => query.roasteries.find((r) => r.id === b.roasteryId))
        .map((b) => ({
          id: b.id,
          properties: b.properties,
          roastery: query.roasteries.find((r) => r.id === b.roasteryId)!
            .properties,
          recipes: query.recipes
            .filter((re) => re.beanId === b.id)
            .map((re) => re.properties),
        })),
  };
};

export const fetchRoasteriesLevel = (): {
  roasteries: Accessor<Roastery[]>;
  isLoading: Accessor<boolean>;
} => {
  const query = fetch();
  return {
    isLoading: () => false,
    roasteries: () =>
      query.roasteries.map((r) => ({
        properties: r.properties,
        beans: query.beans
          .filter((b) => b.roasteryId === r.id)
          .map((b) => ({
            id: b.id,
            properties: b.properties,
            recipes: query.recipes
              .filter((re) => re.beanId === b.id)
              .map((re) => re.properties),
          })),
      })),
  };
};

const fetch = () => {
  const storageRoasteries = localStorage['RoasteriesTable'];
  const storageBeans = localStorage['BeansTable'];
  const storageEspresso = localStorage['EspressoTable'];
  const storageV60 = localStorage['V60Table'];
  let roasteries: RoasteryApi[] = [];
  if (storageRoasteries)
    try {
      roasteries = JSON.parse(storageRoasteries).roasteries as RoasteryApi[];
    } catch {
      // Data Format does not match
    }
  let beans: BeanApi[] = [];
  if (storageBeans)
    try {
      beans = JSON.parse(storageBeans).beans as BeanApi[];
    } catch {
      // Data Format does not match
    }
  let espresso: EspressoData[] = [];
  if (storageEspresso)
    try {
      espresso = JSON.parse(storageEspresso).recipes as EspressoData[];
    } catch {
      // Data Format does not match
    }
  let v60: V60Data[] = [];
  if (storageV60)
    try {
      v60 = JSON.parse(storageV60).recipes as V60Data[];
    } catch {
      // Data Format does not match
    }
  const recipesEspresso = espresso.map((e) => ({
    ...e,
    properties: {
      __typename: 'EspressoProperties',
      properties: e.properties,
    } as RecipePropertiesFieldsFragment,
  }));
  const recipesV60 = v60.map((e) => ({
    ...e,
    properties: {
      __typename: 'V60Properties',
      properties: e.properties,
    } as RecipePropertiesFieldsFragment,
  }));
  return {
    roasteries,
    beans,
    recipes: recipesEspresso.concat(recipesV60),
  };
};
