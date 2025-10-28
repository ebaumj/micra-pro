import { throwInternalServerError } from '../../utils/errors';
import authorize from '../../utils/authorize';
import { getUserRepository } from '@micra-pro/recipe-hub/database';

export default defineEventHandler(async (event) => {
  const runtimeConfig = useRuntimeConfig();
  const userId = authorize(
    (getQuery(event)['token'] as string | undefined) ?? null,
  );
  const repository = getUserRepository(
    runtimeConfig.secrets.databaseConnectionString,
  );
  try {
    const item = await repository.getById(userId);
    return {
      id: item.id,
      username: item.username,
      mfaEnabled: item.enabled2fa,
    };
  } catch {
    throwInternalServerError();
  }
});
