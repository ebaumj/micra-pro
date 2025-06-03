import { createMutation, createQuery } from '@micra-pro/shared/utils-ts';
import {
  AddEspressoRecipeDocument,
  AddEspressoRecipeMutation,
  AddEspressoRecipeMutationVariables,
  AddV60RecipeDocument,
  AddV60RecipeMutation,
  AddV60RecipeMutationVariables,
  EspressoFieldsFragment,
  RecipesDocument,
  RecipesQuery,
  RecipesQueryVariables,
  RemoveRecipeDocument,
  RemoveRecipeMutation,
  RemoveRecipeMutationVariables,
  UpdateEspressoRecipeDocument,
  UpdateEspressoRecipeMutation,
  UpdateEspressoRecipeMutationVariables,
  UpdateV60RecipeDocument,
  UpdateV60RecipeMutation,
  UpdateV60RecipeMutationVariables,
  V60FieldsFragment,
} from './generated/graphql';
import { Accessor, createSignal } from 'solid-js';

export {
  type Espresso as EspressoProperties,
  type V60 as V60Properties,
} from './generated/graphql';

type EspressoRecipe = {
  id: string;
  beanId: string;
  properties: EspressoFieldsFragment;
  update: (properties: EspressoFieldsFragment, callback?: () => void) => void;
  remove: (callback?: () => void) => void;
  isUpdating: Accessor<boolean>;
  isDeleting: Accessor<boolean>;
};

type V60Recipe = {
  id: string;
  beanId: string;
  properties: V60FieldsFragment;
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
  const [updating, setUpdating] = createSignal<string>('');
  const [deleting, setDeleting] = createSignal<string>('');
  const query = createQuery<RecipesQuery, RecipesQueryVariables>(
    RecipesDocument,
    () => ({}),
  );
  const updateEspressoMutation = createMutation<
    UpdateEspressoRecipeMutation,
    UpdateEspressoRecipeMutationVariables
  >(UpdateEspressoRecipeDocument);
  const updateV60Mutation = createMutation<
    UpdateV60RecipeMutation,
    UpdateV60RecipeMutationVariables
  >(UpdateV60RecipeDocument);
  const removeMutation = createMutation<
    RemoveRecipeMutation,
    RemoveRecipeMutationVariables
  >(RemoveRecipeDocument);
  const addEspressoMutation = createMutation<
    AddEspressoRecipeMutation,
    AddEspressoRecipeMutationVariables
  >(AddEspressoRecipeDocument);
  const addV60Mutation = createMutation<
    AddV60RecipeMutation,
    AddV60RecipeMutationVariables
  >(AddV60RecipeDocument);
  const remove = (id: string, callback?: () => void) => {
    setDeleting(id);
    removeMutation({ recipeId: id })
      .then((result) => {
        if (query.resource.latest && result.removeRecipe.uuid)
          query.setDataStore(
            'recipes',
            query.resource.latest?.recipes.filter(
              (r) => r.id !== result.removeRecipe.uuid,
            ),
          );
      })
      .finally(() => {
        setDeleting('');
        callback?.();
      });
  };
  const updateEspresso = (
    id: string,
    properties: EspressoFieldsFragment,
    callback?: () => void,
  ) => {
    setUpdating(id);
    updateEspressoMutation({ properties: properties, recipeId: id })
      .then((result) => {
        const index =
          query.resource.latest?.recipes.findIndex(
            (r) => r.id === result.updateEspressoRecipe.recipe?.id,
          ) ?? -1;
        if (
          index >= 0 &&
          result.updateEspressoRecipe.recipe?.properties.properties
        )
          query.setDataStore(
            'recipes',
            index,
            'properties',
            'properties',
            result.updateEspressoRecipe.recipe.properties.properties,
          );
      })
      .finally(() => {
        setUpdating('');
        callback?.();
      });
  };
  const updateV60 = (
    id: string,
    properties: V60FieldsFragment,
    callback?: () => void,
  ) => {
    setUpdating(id);
    updateV60Mutation({ properties: properties, recipeId: id })
      .then((result) => {
        const index =
          query.resource.latest?.recipes.findIndex(
            (r) => r.id === result.updateV60Recipe.recipe?.id,
          ) ?? -1;
        if (index >= 0 && result.updateV60Recipe.recipe?.properties.properties)
          query.setDataStore(
            'recipes',
            index,
            'properties',
            'properties',
            result.updateV60Recipe.recipe.properties.properties,
          );
      })
      .finally(() => {
        setUpdating('');
        callback?.();
      });
  };
  const addEspresso = (
    beanId: string,
    properties: EspressoFieldsFragment,
    callback?: () => void,
  ) => {
    addEspressoMutation({ beanId: beanId, properties: properties })
      .then((result) => {
        if (!result.addEspressoRecipe.recipe) return;
        const index = query.resource.latest?.recipes.length ?? -1;
        if (index >= 0)
          query.setDataStore('recipes', index, result.addEspressoRecipe.recipe);
        else query.setDataStore('recipes', [result.addEspressoRecipe.recipe]);
      })
      .finally(() => {
        if (callback) callback();
      });
  };
  const addV60 = (
    beanId: string,
    properties: V60FieldsFragment,
    callback?: () => void,
  ) => {
    addV60Mutation({ beanId: beanId, properties: properties })
      .then((result) => {
        if (!result.addV60Recipe.recipe) return;
        const index = query.resource.latest?.recipes.length ?? -1;
        if (index >= 0)
          query.setDataStore('recipes', index, result.addV60Recipe.recipe);
        else query.setDataStore('recipes', [result.addV60Recipe.recipe]);
      })
      .finally(() => {
        if (callback) callback();
      });
  };
  const recipes = () =>
    query.resource.latest?.recipes.map((r) => {
      const accessor = {
        id: r.id,
        beanId: r.beanId,
        remove: (callback?: () => void) => remove(r.id, callback),
        isUpdating: () => updating() === r.id,
        isDeleting: () => deleting() === r.id,
      };
      switch (r.properties.__typename) {
        case 'EspressoProperties':
          return {
            __typename: 'EspressoProperties',
            ...accessor,
            properties: r.properties.properties,
            update: (
              properties: EspressoFieldsFragment,
              callback?: () => void,
            ) => updateEspresso(r.id, properties, callback),
          };
        case 'V60Properties':
          return {
            __typename: 'V60Properties',
            ...accessor,
            properties: r.properties.properties,
            update: (properties: V60FieldsFragment, callback?: () => void) =>
              updateV60(r.id, properties, callback),
          };
      }
    }) ?? [];
  const isEspresso = (
    recipe: (EspressoRecipe | V60Recipe) & { __typename: string },
  ): recipe is EspressoRecipe & { __typename: string } =>
    recipe.__typename === 'EspressoProperties';
  const isV60 = (
    recipe: (EspressoRecipe | V60Recipe) & { __typename: string },
  ): recipe is V60Recipe & { __typename: string } =>
    recipe.__typename === 'V60Properties';
  return {
    recipesEspresso: () => recipes().filter(isEspresso),
    recipesV60: () => recipes().filter(isV60),
    addEspresso: addEspresso,
    addV60: addV60,
    isLoading: () => query.resource.state !== 'ready',
  };
};
