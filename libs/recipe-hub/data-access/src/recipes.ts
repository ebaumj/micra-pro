import { useRecipeHubClient } from '@micra-pro/recipe-hub/client';
import {
  EspressoRecipe,
  V60Recipe,
} from '@micra-pro/recipe-hub/data-definition';
import { Accessor, createResource } from 'solid-js';

type WithUsername<T> = Omit<T, 'userId'> & { username: string };

export const fetchRecipes = () => {
  const client = useRecipeHubClient();
  const [espressoResource] = createResource<WithUsername<EspressoRecipe>[]>(
    async () =>
      (await (
        await client.apiCall('api/recipes/espresso')
      ).json()) as WithUsername<EspressoRecipe>[],
  );
  const [v60Resource] = createResource<WithUsername<V60Recipe>[]>(
    async () =>
      (await (
        await client.apiCall('api/recipes/v60')
      ).json()) as WithUsername<V60Recipe>[],
  );
  return {
    espresso: () => espressoResource.latest ?? [],
    v60: () => v60Resource.latest ?? [],
    loading: () =>
      espressoResource.state === 'pending' || v60Resource.state === 'pending',
  };
};

export const createRecipesAccessor = (userId: Accessor<string>) => {
  const client = useRecipeHubClient();
  const [espressoResource, espressoActions] = createResource<EspressoRecipe[]>(
    async () =>
      (await (
        await client.apiCall('api/recipes/espresso')
      ).json()) as EspressoRecipe[],
  );
  const [v60Resource, v60Actions] = createResource<V60Recipe[]>(
    async () =>
      (await (await client.apiCall('api/recipes/v60')).json()) as V60Recipe[],
  );
  const removeEspresso = async (id: string): Promise<boolean> => {
    try {
      const result = await (
        await client.apiCall(`api/recipes/espresso/${id}`, 'DELETE')
      ).json();
      if (result.success === true) {
        espressoActions.mutate((r) => r?.filter((e) => e.id !== id));
        return true;
      }
      return false;
    } catch {
      return false;
    }
  };
  const removeV60 = async (id: string): Promise<boolean> => {
    try {
      const result = await (
        await client.apiCall(`api/recipes/v60/${id}`, 'DELETE')
      ).json();
      if (result.success === true) {
        v60Actions.mutate((r) => r?.filter((e) => e.id !== id));
        return true;
      }
      return false;
    } catch {
      return false;
    }
  };
  const addEspresso = async (
    recipe: Omit<EspressoRecipe, 'id' | 'userId'>,
  ): Promise<boolean> => {
    try {
      const result = (await (
        await client.apiCall(
          `api/recipes/espresso`,
          'POST',
          JSON.stringify({ ...recipe, userId: userId() }),
        )
      ).json()) as EspressoRecipe;
      espressoActions.mutate((r) => r?.concat(result));
      return true;
    } catch {
      return false;
    }
  };
  const addV60 = async (
    recipe: Omit<V60Recipe, 'id' | 'userId'>,
  ): Promise<boolean> => {
    try {
      const result = (await (
        await client.apiCall(
          `api/recipes/v60`,
          'POST',
          JSON.stringify({ ...recipe, userId: userId() }),
        )
      ).json()) as V60Recipe;
      v60Actions.mutate((r) => r?.concat(result));
      return true;
    } catch {
      return false;
    }
  };
  return {
    espresso: () =>
      espressoResource.latest
        ?.filter((r) => r.userId === userId())
        .map((r) => ({ recipe: r, delete: () => removeEspresso(r.id) })) ?? [],
    v60: () =>
      v60Resource.latest
        ?.filter((r) => r.userId === userId())
        .map((r) => ({ recipe: r, delete: () => removeV60(r.id) })) ?? [],
    addEspresso: (recipe: Omit<EspressoRecipe, 'id' | 'userId'>) =>
      addEspresso(recipe),
    addV60: (recipe: Omit<V60Recipe, 'id' | 'userId'>) => addV60(recipe),
    loading: () =>
      espressoResource.state === 'pending' || v60Resource.state === 'pending',
  };
};
