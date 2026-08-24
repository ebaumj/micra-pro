import jwt from 'jsonwebtoken';
import useAppconfig from './appconfig';

const RefreshTokenSubject = '__refresh_token__';
const TokenLifetime = 60 * 60;

const fail = () => {
  throw createError({
    statusCode: 401,
    statusMessage: 'Unauthorized',
  });
};

const authorize = (header: string | null, assetId?: string) => {
  if (!header) return fail();
  const [prefix, token] = header.split(' ');
  if (prefix !== 'Bearer') return fail();
  if (!token) return fail();
  const runtimeConfig = useAppconfig();
  try {
    return jwt.verify(
      token,
      process.env.REMOTE_ASSET_SERVER_PRIVATE_KEY ?? '',
      {
        audience: runtimeConfig.authorization.jwtAudience,
        issuer: [
          runtimeConfig.authorization.jwtValidIssuers[0]!,
          ...runtimeConfig.authorization.jwtValidIssuers,
        ],
        subject: assetId,
      },
    ) as jwt.JwtPayload;
  } catch {
    return fail();
  }
};

export const refresh = (
  accessToken: jwt.JwtPayload,
  refreshToken: string,
): { token: string; refreshToken: string } => {
  const subject = accessToken.sub ?? fail();
  const runtimeConfig = useAppconfig();
  try {
    jwt.verify(
      refreshToken,
      process.env.REMOTE_ASSET_SERVER_PRIVATE_KEY ?? '',
      {
        audience: runtimeConfig.authorization.jwtAudience,
        issuer: [
          runtimeConfig.authorization.jwtValidIssuers[0]!,
          ...runtimeConfig.authorization.jwtValidIssuers,
        ],
        subject: RefreshTokenSubject,
      },
    ) as jwt.JwtPayload;
  } catch {
    return fail();
  }
  return {
    token: createToken(subject),
    refreshToken: createToken(RefreshTokenSubject),
  };
};

const createToken = (subject: string): string => {
  const runtimeConfig = useAppconfig();
  return jwt.sign({}, process.env.REMOTE_ASSET_SERVER_PRIVATE_KEY ?? '', {
    expiresIn: TokenLifetime,
    issuer: runtimeConfig.authorization.jwtValidIssuers[0],
    audience: runtimeConfig.authorization.jwtAudience,
    subject: subject,
  });
};

export default authorize;
