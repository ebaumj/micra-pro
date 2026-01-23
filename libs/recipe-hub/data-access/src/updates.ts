import { useRecipeHubClient } from '@micra-pro/recipe-hub/client';
import { Update } from '@micra-pro/recipe-hub/data-definition';

export const fetchUpdates = async () => {
  const updates = (await (
    await useRecipeHubClient().apiCall('api/update')
  ).json()) as Update[];
  return updates.map((u) => ({
    ...u,
    created_at: new Date(u.created_at),
  }));
};
