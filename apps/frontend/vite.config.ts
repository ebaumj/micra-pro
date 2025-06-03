import { defineConfig } from 'vite';
import solidPlugin from 'vite-plugin-solid';
import { nxViteTsPaths } from '@nx/vite/plugins/nx-tsconfig-paths.plugin';

export default defineConfig({
  root: __dirname,
  plugins: [solidPlugin(), nxViteTsPaths()],
  server: {
    host: true,
    port: 3000,
    fs: {
      allow: ['../../'],
    },
  },
  build: {
    outDir: '../../dist/apps/frontend',
    emptyOutDir: true,
    reportCompressedSize: true,
    commonjsOptions: { transformMixedEsModules: true },
    target: 'esnext',
    chunkSizeWarningLimit: 1500,
  },
});
