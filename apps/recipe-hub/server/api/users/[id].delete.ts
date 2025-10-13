import { throwInternalServerError } from '../../utils/errors';
import authorize from '../../utils/authorize';
import { getUserRepository } from '../../utils/database/database_access';

export default defineEventHandler(async (event) => {
  const userId = getRouterParam(event, 'id');
  if (!userId) return throwInternalServerError();
  const runtimeConfig = useRuntimeConfig();
  authorize((getQuery(event)['token'] as string | undefined) ?? null, userId);
  const repository = getUserRepository(
    runtimeConfig.secrets.databaseConnectionString,
  );
  try {
    await repository.remove(userId);
  } catch {
    throwInternalServerError();
  }
  return { success: true };
});
