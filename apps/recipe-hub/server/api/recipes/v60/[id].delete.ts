import { throwInternalServerError } from '../../../../utils/errors';
import authorize from '../../../../utils/authorize';
import { getV60RecipeRepository } from '../../../../utils/database/database_access';

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id');
  if (!id) return throwInternalServerError();
  const runtimeConfig = useRuntimeConfig();
  const repository = getV60RecipeRepository(
    runtimeConfig.secrets.databaseConnectionString,
  );
  try {
    const item = await repository.getById(id);
    authorize(
      (getQuery(event)['token'] as string | undefined) ?? null,
      item.userId,
    );
  } catch {
    throwInternalServerError();
  }
  try {
    await repository.remove(id);
  } catch {
    throwInternalServerError();
  }
  return { success: true };
});
