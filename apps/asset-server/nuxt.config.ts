// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-05-15',
  devtools: { enabled: true },
  css: ['./assets/css/main.css'],
  postcss: {
    plugins: {
      tailwindcss: {},
      autoprefixer: {},
    },
  },
  runtimeConfig: {
    secrets: {
      privateKey: process.env.REMOTE_ASSET_SERVER_PRIVATE_KEY,
    },
    authorization: {
      jwtAudience: 'MicraProAssetServer',
      jwtValidIssuers: ['MicraPro'],
    },
  },
  $production: {
    runtimeConfig: {
      blobStorage: {
        folder: '/mnt/localdata/micra-pro/blob-storage',
      },
    },
  },
  $development: {
    runtimeConfig: {
      blobStorage: {
        folder: '../../tmp/blob-storage',
      },
    },
  },
});
