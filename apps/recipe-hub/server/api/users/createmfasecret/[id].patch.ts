import {
  throwInternalServerError,
  throwUnauthorizedError,
} from '../../../utils/errors';
import authorize from '../../../utils/authorize';
import { getUserRepository } from '../../../utils/database/database_access';
import { authenticator } from 'otplib';

export default defineEventHandler(async (event) => {
  const userId = getRouterParam(event, 'id');
  if (!userId) return throwInternalServerError();
  const runtimeConfig = useRuntimeConfig();
  authorize((getQuery(event)['token'] as string | undefined) ?? null, userId);
  const repository = getUserRepository(
    runtimeConfig.secrets.databaseConnectionString,
  );
  const user = await repository.getById(userId);
  // Secret can only be changed if mfa is disabled
  if (user.enabled2fa) throwUnauthorizedError();
  const secret = authenticator.generateSecret();
  const keyUri = authenticator.keyuri(
    user.username,
    runtimeConfig.serviceName,
    secret,
  );
  try {
    await repository.update({
      ...user,
      secret2fa: secret,
    });
  } catch {
    throwInternalServerError();
  }
  return { otpauth: keyUri };
});
