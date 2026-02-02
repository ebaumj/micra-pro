import tailwindcss from '@tailwindcss/vite';
import { fileURLToPath } from 'node:url';

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-10-28',
  vite: {
    plugins: [tailwindcss()],
  },
  css: ['./app/assets/css/main.css'],
  alias: {
    '@micra-pro/recipe-hub/data-definition': fileURLToPath(
      new URL(
        '../../libs/recipe-hub/data-definition/src/index.ts',
        import.meta.url,
      ),
    ),
    '@micra-pro/recipe-hub/database': fileURLToPath(
      new URL('server/utils/database/index.ts', import.meta.url),
    ),
  },
  routeRules: {
    '/api/**': {
      cors: true,
    },
    '/token': {
      cors: true,
    },
    '/token/mfa': {
      cors: true,
    },
  },
  runtimeConfig: {
    secrets: {
      privateKey: process.env.RECIPE_HUB_PRIVATE_KEY,
      databaseConnectionString: process.env.DATABASE_URL,
      emailServerToken: process.env.EMAIL_SERVER_TOKEN,
      emailServerAddress: process.env.EMAIL_SERVER_ADDRESS,
    },
    authorization: {
      jwtAudience: 'MicraProRecipeHub',
      jwtIssuer: 'MicraProRecipeHub',
      jwtValidIssuers: ['MicraProRecipeHub'],
    },
    serviceName: 'Micra Pro Recipe Hub',
    dummyFrontendPage: 'https://mp-dummy.edgeone.app/',
  },
  $production: {
    runtimeConfig: {
      userConfirmApi: 'https://micra-pro.vercel.app/new',
    },
  },
  $development: {
    runtimeConfig: {
      userConfirmApi: 'http://localhost:3002/new',
    },
  },
});
