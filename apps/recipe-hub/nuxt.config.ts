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
  },
  $production: {
    runtimeConfig: {
      userConfirmApi: 'http://localhost:3002/new',
    },
  },
  $development: {
    runtimeConfig: {
      userConfirmApi: 'http://localhost:3002/new',
    },
  },
});
