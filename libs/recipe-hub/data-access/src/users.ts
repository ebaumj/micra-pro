import { useRecipeHubClient, getClientId } from '@micra-pro/recipe-hub/client';
import { createResource } from 'solid-js';
import { RecipeHubUser } from '@micra-pro/recipe-hub/data-definition';

export const createUserAccessor = () => {
  const client = useRecipeHubClient();
  const [resource, actions] = createResource(
    async () =>
      (await (await client.apiCall('api/users')).json()) as RecipeHubUser,
  );
  return {
    user: () => resource.latest,
    loading: () =>
      resource.state === 'pending' || resource.state === 'refreshing',
    delete: async () => {
      const id = resource.latest?.id;
      if (!id) return false;
      try {
        const result = await (
          await client.apiCall(`api/users/${id}`, 'DELETE')
        ).json();
        if (result.success === true) {
          client.logout();
          return true;
        }
        return false;
      } catch {
        return false;
      }
    },
    changePassword: async (oldPw: string, newPw: string) => {
      const id = resource.latest?.id;
      if (!id) return false;
      try {
        const result = await (
          await client.apiCall(
            `api/users/changepassword/${id}`,
            'PATCH',
            JSON.stringify({ newPassword: newPw, oldPassword: oldPw }),
          )
        ).json();
        if (result.success === true) {
          actions.refetch();
          return true;
        }
        return false;
      } catch {
        return false;
      }
    },
    setMfa: async (mfaCode: string, enabled: boolean) => {
      const id = resource.latest?.id;
      if (!id) return false;
      try {
        const result = await (
          await client.apiCall(
            `api/users/changemfa/${id}`,
            'PATCH',
            JSON.stringify({ code: mfaCode, enable: enabled }),
          )
        ).json();
        if (result.success === true) {
          actions.refetch();
          return true;
        }
        return false;
      } catch {
        return false;
      }
    },
    createMfaSecret: async () => {
      const id = resource.latest?.id;
      if (!id) return null;
      try {
        const result = await (
          await client.apiCall(`api/users/createmfasecret/${id}`, 'PATCH')
        ).json();
        return result.otpauth ?? null;
      } catch {
        return null;
      }
    },
  };
};

export const createUserFactory = () => {
  const client = useRecipeHubClient();
  const clientId = getClientId();
  return {
    createUser: async (username: string, password: string, email: string) =>
      (
        await (
          await client.apiCall(
            'api/users',
            'POST',
            JSON.stringify({ username, password, email, clientId }),
          )
        ).json()
      ).success
        ? true
        : false,
  };
};
