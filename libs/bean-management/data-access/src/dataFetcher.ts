import { createQuery } from '@micra-pro/shared/utils-ts';
import {
  BeanProperties,
  BeansDocument,
  BeansQuery,
  BeansQueryVariables,
  RecipePropertiesFieldsFragment,
  RecipesDocument,
  RecipesQuery,
  RecipesQueryVariables,
  RoasteriesDocument,
  RoasteriesQuery,
  RoasteriesQueryVariables,
  RoasteryProperties,
} from './generated/graphql';
import { Accessor } from 'solid-js';

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
  refetch: () => void;
} => {
  const query = fetch();
  return {
    isLoading: () =>
      query.roasteries.resource.state === 'pending' ||
      query.beans.resource.state === 'pending' ||
      query.recipes.resource.state === 'pending',
    beans: () =>
      query.roasteries.resource.latest &&
      query.beans.resource.latest &&
      query.recipes.resource.latest
        ? query.beans.resource.latest.beans
            .filter((b) =>
              query.roasteries.resource.latest?.roasteries.find(
                (r) => r.id === b.roasteryId,
              ),
            )
            .map((b) => ({
              id: b.id,
              properties: b.properties,
              roastery: query.roasteries.resource.latest!.roasteries.find(
                (r) => r.id === b.roasteryId,
              )!.properties,
              recipes: query.recipes.resource
                .latest!.recipes.filter((re) => re.beanId === b.id)
                .map((re) => re.properties),
            }))
        : [],
    refetch: () => {
      query.roasteries.resourceActions.refetch();
      query.beans.resourceActions.refetch();
      query.recipes.resourceActions.refetch();
    },
  };
};

export const fetchRoasteriesLevel = (): {
  roasteries: Accessor<Roastery[]>;
  isLoading: Accessor<boolean>;
} => {
  const query = fetch();
  return {
    isLoading: () =>
      query.roasteries.resource.state === 'pending' ||
      query.beans.resource.state === 'pending' ||
      query.recipes.resource.state === 'pending',
    roasteries: () =>
      query.roasteries.resource.latest &&
      query.beans.resource.latest &&
      query.recipes.resource.latest
        ? query.roasteries.resource.latest.roasteries.map((r) => ({
            properties: r.properties,
            beans: query.beans.resource
              .latest!.beans.filter((b) => b.roasteryId === r.id)
              .map((b) => ({
                id: b.id,
                properties: b.properties,
                recipes: query.recipes.resource
                  .latest!.recipes.filter((re) => re.beanId === b.id)
                  .map((re) => re.properties),
              })),
          }))
        : [],
  };
};

const fetch = () => ({
  roasteries: createQuery<RoasteriesQuery, RoasteriesQueryVariables>(
    RoasteriesDocument,
    () => ({}),
  ),
  beans: createQuery<BeansQuery, BeansQueryVariables>(
    BeansDocument,
    () => ({}),
  ),
  recipes: createQuery<RecipesQuery, RecipesQueryVariables>(
    RecipesDocument,
    () => ({}),
  ),
});
