import tailwindcss from '@tailwindcss/vite';

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-10-28',
  vite: {
    plugins: [tailwindcss() as any],
  },
  css: ['assets/css/main.css'],
  runtimeConfig: {
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
