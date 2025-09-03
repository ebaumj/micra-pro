import {
  throwInternalServerError,
  throwUnauthorizedError,
} from '../../../../utils/errors';
import authorize from '../../../../utils/authorize';
import { getUserRepository } from '../../../../utils/database/database_access';
import bcrypt from 'bcrypt';

type ChangePasswordRequestBodyType = {
  newPassword: string;
  oldPassword: string;
};

export default defineEventHandler(async (event) => {
  const userId = getRouterParam(event, 'id');
  if (!userId) return throwInternalServerError();
  const runtimeConfig = useRuntimeConfig();
  const body = JSON.parse(
    await readBody(event),
  ) as ChangePasswordRequestBodyType;
  authorize((getQuery(event)['token'] as string | undefined) ?? null, userId);
  const repository = getUserRepository(
    runtimeConfig.secrets.databaseConnectionString,
  );
  const user = await repository.getById(userId);
  if (!(await bcrypt.compare(body.oldPassword, user.password)))
    throwUnauthorizedError();
  try {
    await repository.update({
      ...user,
      password: await bcrypt.hash(body.newPassword, await bcrypt.genSalt()),
    });
  } catch {
    throwInternalServerError();
  }
  return { success: true };
});
