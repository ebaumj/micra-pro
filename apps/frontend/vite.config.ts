import { defineConfig } from 'vite';
import solidPlugin from 'vite-plugin-solid';
import { nxViteTsPaths } from '@nx/vite/plugins/nx-tsconfig-paths.plugin';
import tailwindcss from '@tailwindcss/vite';

export default defineConfig((conf) => ({
  root: __dirname,
  plugins: [solidPlugin(), tailwindcss(), nxViteTsPaths()],
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
