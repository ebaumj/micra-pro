import { createQuery } from '@micra-pro/shared/utils-ts';
import {
  BeanProperties,
  BeansDocument,
  BeansQuery,
  BeansQueryVariables,
  FlowProfilePropertiesFieldsFragment,
  FlowProfilesDocument,
  FlowProfilesQuery,
  FlowProfilesQueryVariables,
  RecipePropertiesFieldsFragment,
  RecipesDocument,
  RecipesQuery,
  RecipesQueryVariables,
  RoasteriesDocument,
  RoasteriesQuery,
  RoasteriesQueryVariables,
  RoasteryProperties,
} from './generated/graphql';
import { Accessor, createEffect } from 'solid-js';

type Recipe = RecipePropertiesFieldsFragment & {
  flowProfile?: FlowProfilePropertiesFieldsFragment;
};

export type Roastery = {
  properties: RoasteryProperties;
  beans: {
    id: string;
    properties: BeanProperties;
    recipes: Recipe[];
  }[];
};

export type Bean = {
  id: string;
  properties: BeanProperties;
  roastery: RoasteryProperties;
  recipes: Recipe[];
};

export const fetchBeansLevel = (): {
  beans: Accessor<Bean[]>;
  isLoading: Accessor<boolean>;
} => {
  const query = fetch();
  return {
    isLoading: () =>
      query.roasteries.resource.state === 'pending' ||
      query.beans.resource.state === 'pending' ||
      query.recipes.resource.state === 'pending' ||
      query.flowProfiles.resource.state === 'pending',
    beans: () =>
      query.roasteries.resource.latest &&
      query.beans.resource.latest &&
      query.recipes.resource.latest &&
      query.flowProfiles.resource.latest
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
                .map((re) => ({
                  ...re.properties,
                  flowProfile:
                    query.flowProfiles.resource.latest!.flowProfiles.find(
                      (p) => p.recipeId === re.id,
                    )?.properties,
                })),
            }))
        : [],
  };
};

export const fetchRoasteriesLevel = (): {
  roasteries: Accessor<Roastery[]>;
  isLoading: Accessor<boolean>;
} => {
  const query = fetch();
  createEffect(() => console.log(query.flowProfiles.resource.latest));
  return {
    isLoading: () =>
      query.roasteries.resource.state === 'pending' ||
      query.beans.resource.state === 'pending' ||
      query.recipes.resource.state === 'pending' ||
      query.flowProfiles.resource.state === 'pending',
    roasteries: () =>
      query.roasteries.resource.latest &&
      query.beans.resource.latest &&
      query.recipes.resource.latest &&
      query.flowProfiles.resource.latest
        ? query.roasteries.resource.latest.roasteries.map((r) => ({
            properties: r.properties,
            beans: query.beans.resource
              .latest!.beans.filter((b) => b.roasteryId === r.id)
              .map((b) => ({
                id: b.id,
                properties: b.properties,
                recipes: query.recipes.resource
                  .latest!.recipes.filter((re) => re.beanId === b.id)
                  .map((re) => ({
                    ...re.properties,
                    flowProfile:
                      query.flowProfiles.resource.latest!.flowProfiles.find(
                        (p) => p.recipeId === re.id,
                      )?.properties,
                  })),
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
  flowProfiles: createQuery<FlowProfilesQuery, FlowProfilesQueryVariables>(
    FlowProfilesDocument,
    () => ({}),
  ),
});
