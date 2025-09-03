import merge from 'lodash/merge';

const config = {
  graphql: {
    wsUri: 'ws://localhost:5232/graphql',
    httpUri: 'http://localhost:5232/graphql',
    foreverAdminToken: import.meta.env.VITE_FOREVER_ADMIN_TOKEN,
  },
  recipeHub: {
    uri: 'http://micra-pro.vercel.app',
  },
  useLostBackendConnectionModal: true,
  display: {
    resolution: {
      width: 800,
      height: 480,
    },
  },
};

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

const merged = merge(config, getLocalDevConfig());

export default merged;
