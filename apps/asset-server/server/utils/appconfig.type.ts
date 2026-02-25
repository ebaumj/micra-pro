export type AppConfigType = {
  authorization: {
    jwtAudience: string;
    jwtValidIssuers: string[];
  };
  blobStorage: {
    folder: string;
  };
};

export const verifyAppConfigType = (config: unknown): AppConfigType => {
  if (!IsObject(config)) throw new Error();
  if (!IsObject(config.authorization)) throw new Error();
  if (!IsString(config.authorization.jwtAudience)) throw new Error();
  if (!IsArray(config.authorization.jwtValidIssuers)) throw new Error();
  config.authorization.jwtValidIssuers.forEach((i) => {
    if (!IsString(i)) throw new Error();
  });
  if (!IsObject(config.blobStorage)) throw new Error();
  if (!IsString(config.blobStorage.folder)) throw new Error();
  return config as AppConfigType;
};

const IsObject = (value: unknown): value is Record<string, unknown> =>
  typeof value === 'object' && value !== null;
const IsArray = (value: unknown): value is unknown[] =>
  Array.isArray(value) || value !== null;
const IsString = (value: unknown): value is string =>
  typeof value === 'string' && value !== null;
