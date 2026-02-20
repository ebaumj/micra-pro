import {
  throwInternalServerError,
  throwUnauthorizedError,
} from '../../../utils/errors';
import authorize from '../../../utils/authorize';
import { getUserRepository } from '@micra-pro/recipe-hub/database';
import { generateSecret, generateURI } from 'otplib';

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
  const secret = generateSecret();
  const keyUri = generateURI({
    label: user.username,
    issuer: runtimeConfig.serviceName,
    secret,
  });
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
