import { getUserRepository } from '@micra-pro/recipe-hub/database';
import jwt from 'jsonwebtoken';
import {
  createResponse,
  errorEvent,
  invalidRequest,
  mfaIssuer,
} from './index.post';
import { authenticator } from 'otplib';

type RequestBodyType = {
  code: string;
};

export default defineEventHandler(async (event) => {
  const body = (await readBody(event)) as RequestBodyType;
  const runtimeConfig = useRuntimeConfig();
  try {
    const tokenDecoded = validateToken(
      getQuery(event)['token'] as string | undefined,
      runtimeConfig,
    );
    const repository = getUserRepository(
      runtimeConfig.secrets.databaseConnectionString,
    );
    const user = await repository.getById(tokenDecoded.userId);
    if (tokenDecoded.clientId !== user.clientId) throw new Error();
    if (!authenticator.check(body.code, user.secret2fa)) throw new Error();
    return createResponse(
      runtimeConfig,
      tokenDecoded.userId,
      tokenDecoded.clientId,
    );
  } catch {
    return errorEvent(event, invalidRequest);
  }
});

const validateToken = (
  token: string | undefined,
  config: ReturnType<typeof useRuntimeConfig>,
): { userId: string; clientId: string } => {
  if (!token) throw new Error();
  const tokenDecoded = jwt.verify(token, config.secrets.privateKey, {
    audience: config.authorization.jwtAudience,
    issuer: mfaIssuer,
  }) as jwt.JwtPayload;
  if (!tokenDecoded.sub) throw new Error();
  if (!tokenDecoded.clientId) throw new Error();
  return { userId: tokenDecoded.sub, clientId: tokenDecoded.clientId };
};
