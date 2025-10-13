import jwt, { type JwtPayload } from 'jsonwebtoken';
import { throwUnauthorizedError } from './errors';

const authorize = (token: string | null, userId?: string) => {
  if (!token) return throwUnauthorizedError();
  const runtimeConfig = useRuntimeConfig();
  try {
    const jwtPayload = jwt.verify(token, runtimeConfig.secrets.privateKey, {
      audience: runtimeConfig.authorization.jwtAudience,
      issuer: [
        runtimeConfig.authorization.jwtValidIssuers[0]!,
        ...runtimeConfig.authorization.jwtValidIssuers,
      ],
      subject: userId ?? undefined,
    }) as JwtPayload;
    if (!jwtPayload.sub) throw new Error();
    return jwtPayload.sub;
  } catch {
    return throwUnauthorizedError();
  }
};

export default authorize;
