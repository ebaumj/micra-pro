import { getUpdates } from '@micra-pro/recipe-hub/database';

export default defineEventHandler(async () => {
  const runtimeConfig = useRuntimeConfig();
  const updates = await getUpdates(
    runtimeConfig.secrets.databaseConnectionString,
  );
  return updates;
});
