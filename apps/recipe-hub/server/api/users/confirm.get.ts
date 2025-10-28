import { throwInternalServerError } from '../../utils/errors';
import { getUserRepository } from '@micra-pro/recipe-hub/database';
import { confirmIssuer } from './index.post';
import jwt from 'jsonwebtoken';
import { authenticator } from 'otplib';

export default defineEventHandler(async (event) => {
  const runtimeConfig = useRuntimeConfig();
  const token = getQuery(event)['token'] as string;
  if (!token) throwInternalServerError();
  const repository = getUserRepository(
    runtimeConfig.secrets.databaseConnectionString,
  );
  try {
    const tokenDecoded = jwt.verify(token, runtimeConfig.secrets.privateKey, {
      issuer: confirmIssuer,
      audience: runtimeConfig.authorization.jwtAudience,
    }) as jwt.JwtPayload;
    if (!tokenDecoded.username) throw new Error();
    if (!tokenDecoded.email) throw new Error();
    if (!tokenDecoded.password) throw new Error();
    if (!tokenDecoded.clientId) throw new Error();
    const newUser = {
      username: tokenDecoded.username as string,
      email: tokenDecoded.email as string,
      password: tokenDecoded.password as string,
      clientId: tokenDecoded.clientId as string,
    };
    await repository.add({
      clientId: newUser.clientId,
      email: newUser.email,
      username: newUser.username,
      password: newUser.password,
      enabled2fa: false,
      secret2fa: authenticator.generateSecret(),
    });
  } catch {
    throwInternalServerError();
  }
  return { success: true };
});
