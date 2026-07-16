import { defineConfig } from 'vite';
import solidPlugin from 'vite-plugin-solid';
import tailwindcss from '@tailwindcss/vite';
import { appconfigPlugin } from './plugins/appconfig.check';

export default defineConfig((conf) => ({
  root: import.meta.dirname,
  plugins: [solidPlugin(), tailwindcss(), appconfigPlugin()],
  resolve: {
    tsconfigPaths: true,
  },
  server: {
    host: true,
    fs: {
      allow: ['../../'],
    },
  },
  build: {
    outDir: `../../dist/apps/frontend${conf.mode === 'dummy' ? '-dummy' : ''}`,
    emptyOutDir: true,
    reportCompressedSize: true,
    commonjsOptions: { transformMixedEsModules: true },
    target: 'esnext',
    chunkSizeWarningLimit: 1500,
  },
  define: {
    __USE_DUMMIES__: conf.mode === 'dummy',
  },
}));
