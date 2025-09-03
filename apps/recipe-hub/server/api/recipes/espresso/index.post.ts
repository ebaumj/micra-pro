import { throwInternalServerError } from '../../../../utils/errors';
import authorize from '../../../../utils/authorize';
import { getEspressoRecipeRepository } from '../../../../utils/database/database_access';

export default defineEventHandler(async (event) => {
  const body = JSON.parse(await readBody(event));
  authorize(
    (getQuery(event)['token'] as string | undefined) ?? null,
    body.userId,
  );
  const runtimeConfig = useRuntimeConfig();
  const repository = getEspressoRecipeRepository(
    runtimeConfig.secrets.databaseConnectionString,
  );
  try {
    return await repository.add(body);
  } catch {
    throwInternalServerError();
  }
});
