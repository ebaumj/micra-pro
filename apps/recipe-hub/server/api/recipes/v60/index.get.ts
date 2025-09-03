import { throwInternalServerError } from '../../../../utils/errors';
import authorize from '../../../../utils/authorize';
import {
  getUserRepository,
  getV60RecipeRepository,
} from '../../../../utils/database/database_access';

export default defineEventHandler(async (event) => {
  authorize((getQuery(event)['token'] as string | undefined) ?? null);
  const runtimeConfig = useRuntimeConfig();
  const repository = getV60RecipeRepository(
    runtimeConfig.secrets.databaseConnectionString,
  );
  const users = await getUserRepository(
    runtimeConfig.secrets.databaseConnectionString,
  ).getAll();
  try {
    return (await repository.getAll()).map((r) => ({
      ...r,
      username: users.find((u) => u.id === r.userId)?.username ?? '',
    }));
  } catch {
    throwInternalServerError();
  }
});
