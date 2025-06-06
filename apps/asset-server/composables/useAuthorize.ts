import jwt from 'jsonwebtoken';

const fail = () => {
  throw createError({
    statusCode: 401,
    statusMessage: 'Unauthorized',
  });
};

const useAuthorize = (header: string | null, assetId?: string) => {
  if (!header) return fail();
  const [prefix, token] = header.split(' ');
  if (prefix !== 'Bearer') return fail();
  if (!token) return fail();
  const runtimeConfig = useRuntimeConfig();
  try {
    jwt.verify(token, runtimeConfig.secrets.privateKey, {
      audience: runtimeConfig.authorization.jwtAudience,
      issuer: runtimeConfig.authorization.jwtValidIssuers,
      subject: assetId,
    });
  } catch {
    return fail();
  }
};

export default useAuthorize;
