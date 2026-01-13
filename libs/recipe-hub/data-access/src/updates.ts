import { useRecipeHubClient } from '@micra-pro/recipe-hub/client';
import { Update } from '@micra-pro/recipe-hub/data-definition';
import { createResource } from 'solid-js';

export const fetchUpdates = () => {
  const client = useRecipeHubClient();
  const [updatesResource] = createResource<Update[]>(
    async () => (await (await client.apiCall('api/update')).json()) as Update[],
  );
  return {
    updates: () =>
      updatesResource.latest?.map((u) => ({
        ...u,
        created_at: new Date(u.created_at),
      })) ?? [],
    loading: () => updatesResource.state === 'pending',
  };
};
