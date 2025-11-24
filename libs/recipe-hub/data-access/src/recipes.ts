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
  const espressoAction = async (
    recipe: Omit<EspressoRecipe, 'userId'>,
    action: 'add' | 'update',
  ): Promise<boolean> => {
    try {
      const result = (await (
        await client.apiCall(
          action === 'add'
            ? `api/recipes/espresso`
            : `api/recipes/espresso/${recipe.id}`,
          action === 'add' ? 'POST' : 'PUT',
          JSON.stringify({ ...recipe, userId: userId() }),
        )
      ).json()) as EspressoRecipe;
      espressoActions.mutate((r) => {
        if (!r) return undefined;
        if (action === 'add') return r.concat(result);
        r[r.findIndex((r) => r.id === result.id)] = result;
        return r;
      });
      return true;
    } catch {
      return false;
    }
  };
  const v60Action = async (
    recipe: Omit<V60Recipe, 'userId'>,
    action: 'add' | 'update',
  ): Promise<boolean> => {
    try {
      const result = (await (
        await client.apiCall(
          action === 'add' ? `api/recipes/v60` : `api/recipes/v60/${recipe.id}`,
          action === 'add' ? 'POST' : 'PUT',
          JSON.stringify({ ...recipe, userId: userId() }),
        )
      ).json()) as V60Recipe;
      v60Actions.mutate((r) => {
        if (!r) return undefined;
        if (action === 'add') return r.concat(result);
        r[r.findIndex((r) => r.id === result.id)] = result;
        return r;
      });
      return true;
    } catch {
      return false;
    }
  };
  return {
    espresso: () =>
      espressoResource.latest
        ?.filter((r) => r.userId === userId())
        .map((r) => ({
          recipe: r,
          delete: () => removeEspresso(r.id),
          update: () => espressoAction(r, 'update'),
        })) ?? [],
    v60: () =>
      v60Resource.latest
        ?.filter((r) => r.userId === userId())
        .map((r) => ({
          recipe: r,
          delete: () => removeV60(r.id),
          update: () => v60Action(r, 'update'),
        })) ?? [],
    addEspresso: (recipe: Omit<EspressoRecipe, 'userId'>) =>
      espressoAction(recipe, 'add'),
    addV60: (recipe: Omit<V60Recipe, 'userId'>) => v60Action(recipe, 'add'),
    loading: () =>
      espressoResource.state === 'pending' || v60Resource.state === 'pending',
  };
};
