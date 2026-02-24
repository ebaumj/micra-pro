import { defu } from 'defu';
import fs from 'node:fs';
import path from 'node:path';
import { type AppConfigType } from './appconfig.type';

const configFileName = 'appconfig.json';
const configDevName = 'appconfig.Dev.json';

const getConfigDir = () =>
  process.env.NODE_ENV === 'development'
    ? path.resolve(process.cwd(), 'app-config')
    : path.resolve(path.dirname(process.argv[1] ?? ''), '../');

export const migrateAppConfig = () => {
  const configPath = path.resolve(getConfigDir(), configFileName);
  const configMigrationPath = path.resolve(
    getConfigDir(),
    useRuntimeConfig().appconfigMigration,
  );
  if (!fs.existsSync(configMigrationPath)) return;
  const migration = JSON.parse(fs.readFileSync(configMigrationPath, 'utf-8'));
  const config: AppConfigType = fs.existsSync(configPath)
    ? defu(JSON.parse(fs.readFileSync(configPath, 'utf-8')), migration)
    : migration;
  config.authorization.jwtValidIssuers =
    config.authorization.jwtValidIssuers.filter(
      (v, i, a) => a.indexOf(v) === i,
    );
  fs.writeFileSync(configPath, JSON.stringify(config));
  fs.unlinkSync(configMigrationPath);
};

const useAppconfig = (): AppConfigType => {
  const config = JSON.parse(
    fs.readFileSync(path.resolve(getConfigDir(), configFileName), 'utf-8'),
  );
  if (process.env.NODE_ENV === 'development') {
    const configDevPath = path.resolve(getConfigDir(), configDevName);
    const dev = fs.existsSync(configDevPath)
      ? JSON.parse(fs.readFileSync(configDevPath, 'utf-8'))
      : null;
    return defu(dev, config);
  }
  return config;
};

export default useAppconfig;
