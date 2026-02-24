import tailwindcss from '@tailwindcss/vite';
import fs from 'node:fs';
import path from 'node:path';
import { verifyAppConfigType } from './server/utils/appconfig.type';

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-10-28',
  vite: {
    plugins: [tailwindcss() as any],
  },
  css: ['assets/css/main.css'],
  runtimeConfig: {
    appconfigMigration: 'appconfig.Migration.json',
  },
  hooks: {
    close: () => {
      const source = path.resolve(__dirname, 'app-config');
      const dest = path.resolve(__dirname, '.output');
      if (fs.existsSync(source) && fs.existsSync(dest))
        fs.cpSync(source, dest, { recursive: true });
    },
    'build:before': () => {
      const source = path.resolve(__dirname, 'app-config/appconfig.json');
      if (!fs.existsSync(source)) throw new Error('Appconfig not found');
      try {
        verifyAppConfigType(JSON.parse(fs.readFileSync(source, 'utf-8')));
      } catch {
        throw new Error('Appconfig format is wrong');
      }
    },
  },
});
