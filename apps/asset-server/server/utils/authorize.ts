import jwt from 'jsonwebtoken';

const fail = () => {
  throw createError({
    statusCode: 401,
    statusMessage: 'Unauthorized',
  });
};

export const authorize = (header: string | null) => {
  if (!header) return fail();
  const [prefix, token] = header.split(' ');
  if (prefix !== 'Bearer') return fail();
  if (!token) return fail();
  const runtimeConfig = useRuntimeConfig();
  try {
    jwt.verify(token, runtimeConfig.secrets.privateKey, {
      audience: runtimeConfig.authorization.jwtAudience,
      issuer: [
        runtimeConfig.authorization.jwtValidIssuers[0]!,
        ...runtimeConfig.authorization.jwtValidIssuers,
      ],
    });
  } catch {
    return fail();
  }
};

export const authorizeId = (
  header: string | null,
  assetId?: string,
): string => {
  if (!header) return fail();
  const [prefix, token] = header.split(' ');
  if (prefix !== 'Bearer') return fail();
  if (!token) return fail();
  const runtimeConfig = useRuntimeConfig();
  try {
    if (!assetId) return fail();
    jwt.verify(token, runtimeConfig.secrets.privateKey, {
      audience: runtimeConfig.authorization.jwtAudience,
      issuer: [
        runtimeConfig.authorization.jwtValidIssuers[0]!,
        ...runtimeConfig.authorization.jwtValidIssuers,
      ],
      subject: assetId,
    });
    return assetId;
  } catch {
    return fail();
  }
};
