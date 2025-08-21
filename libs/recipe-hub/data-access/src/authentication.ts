import { useRecipeHubClient } from '@micra-pro/recipe-hub/client';

export const useAuthentication = () => {
  const client = useRecipeHubClient();
  return {
    currentUser: client.user,
    login: client.login,
    logout: client.logout,
  };
};
