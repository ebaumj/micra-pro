import merge from 'lodash/merge';
import { type AppConfig } from './config.type';

declare global {
  interface Window {
    __APPCONFIG__: AppConfig;
  }
}

const getLocalDevConfig = () => {
  if (import.meta.env.MODE !== 'development') return undefined;

  const localConfigImport = import.meta.glob('./config.local.ts', {
    eager: true,
  });
  const imported = localConfigImport['./config.local.ts'];
  if (
    imported !== undefined &&
    typeof imported === 'object' &&
    imported !== null &&
    'default' in imported &&
    typeof imported.default === 'object' &&
    imported.default !== null
  ) {
    return imported.default;
  }
  return undefined;
};

const merged = () => {
  const baseConfig: AppConfig = window.__APPCONFIG__ || {};
  return merge(baseConfig, getLocalDevConfig());
};

export const fetchConfig = async (): Promise<void> => {
  const cfg = await (await fetch('/appconfig.json')).json();
  window.__APPCONFIG__ = merge(cfg as AppConfig, getLocalDevConfig());
};

export default merged;
