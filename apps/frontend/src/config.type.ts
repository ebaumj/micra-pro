export type AppConfig = {
  graphql: {
    wsUri: string;
    httpUri: string;
  };
  recipeHub: {
    uri: string;
  };
  useLostBackendConnectionModal: boolean;
  display: {
    resolution: {
      width: number;
      height: number;
    };
  };
};

export const verifyConfig = (config: unknown): AppConfig => {
  if (!IsObject(config)) throw new Error();
  if (!IsObject(config.graphql)) throw new Error();
  if (!IsString(config.graphql.wsUri)) throw new Error();
  if (!IsString(config.graphql.httpUri)) throw new Error();
  if (!IsObject(config.recipeHub)) throw new Error();
  if (!IsString(config.recipeHub.uri)) throw new Error();
  if (!IsBoolean(config.useLostBackendConnectionModal)) throw new Error();
  if (!IsObject(config.display)) throw new Error();
  if (!IsObject(config.display.resolution)) throw new Error();
  if (!IsNumber(config.display.resolution.width)) throw new Error();
  if (!IsNumber(config.display.resolution.height)) throw new Error();
  return config as AppConfig;
};

const IsObject = (value: unknown): value is Record<string, unknown> =>
  typeof value === 'object' && value !== null;
const IsString = (value: unknown): value is string =>
  typeof value === 'string' && value !== null;
const IsNumber = (value: unknown): value is number =>
  typeof value === 'number' && value !== null;
const IsBoolean = (value: unknown): value is boolean =>
  typeof value === 'boolean' && value !== null;
