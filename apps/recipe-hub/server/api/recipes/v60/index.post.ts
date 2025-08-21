import { throwInternalServerError } from '../../../../utils/errors';
import authorize from '../../../../utils/authorize';
import { getV60RecipeRepository } from '../../../../utils/database/database_access';

export default defineEventHandler(async (event) => {
  const body = JSON.parse(await readBody(event));
  authorize(
    (getQuery(event)['token'] as string | undefined) ?? null,
    body.userId,
  );
  const runtimeConfig = useRuntimeConfig();
  const repository = getV60RecipeRepository(
    runtimeConfig.secrets.databaseConnectionString,
  );
  try {
    return await repository.add(body);
  } catch {
    throwInternalServerError();
  }
});
