import { throwInternalServerError } from '../../../utils/errors';
import authorize from '../../../utils/authorize';
import { getEspressoRecipeRepository } from '@micra-pro/recipe-hub/database';
import { type EspressoRecipe } from '@micra-pro/recipe-hub/data-definition';

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id');
  if (!id) return throwInternalServerError();
  const body = JSON.parse(await readBody(event)) as EspressoRecipe;
  if (body.id !== id) throwInternalServerError();
  authorize(
    (getQuery(event)['token'] as string | undefined) ?? null,
    body.userId,
  );
  const runtimeConfig = useRuntimeConfig();
  const repository = getEspressoRecipeRepository(
    runtimeConfig.secrets.databaseConnectionString,
  );
  try {
    return await repository.update(body);
  } catch {
    throwInternalServerError();
  }
});
