import {
  throwInternalServerError,
  throwUnauthorizedError,
} from '../../../utils/errors';
import authorize from '../../../utils/authorize';
import { getUserRepository } from '../../../utils/database/database_access';
import { authenticator } from 'otplib';

type ChangeMfaRequestBodyType = {
  code: string;
  enable: boolean;
};

export default defineEventHandler(async (event) => {
  const userId = getRouterParam(event, 'id');
  if (!userId) return throwInternalServerError();
  const runtimeConfig = useRuntimeConfig();
  const body = JSON.parse(await readBody(event)) as ChangeMfaRequestBodyType;
  authorize((getQuery(event)['token'] as string | undefined) ?? null, userId);
  const repository = getUserRepository(
    runtimeConfig.secrets.databaseConnectionString,
  );
  const user = await repository.getById(userId);
  if (!authenticator.check(body.code, user.secret2fa)) throwUnauthorizedError();
  try {
    await repository.update({
      ...user,
      enabled2fa: body.enable,
    });
  } catch {
    throwInternalServerError();
  }
  return { success: true };
});
