/// <reference types="vitest" />
import { defineConfig } from 'vite';

import { nxViteTsPaths } from '@nx/vite/plugins/nx-tsconfig-paths.plugin';
import dts from 'vite-plugin-dts';
import path from 'path';

export default defineConfig({
  root: __dirname,
  cacheDir: '../../../node_modules/.vite/scale-management-data-access',
  plugins: [
    dts({
      entryRoot: 'src',
      tsconfigPath: path.posix.join(__dirname, 'tsconfig.lib.json'),
    }),

    nxViteTsPaths(),
  ],
  build: {
    outDir: '../../../dist/libs/scale-management/data-access',
    emptyOutDir: true,
    reportCompressedSize: true,
    commonjsOptions: { transformMixedEsModules: true },
    lib: {
      entry: 'src/index.ts',
      name: 'scale-management-data-access',
      fileName: 'index',
      formats: ['es'],
    },
    rollupOptions: {
      external: [],
    },
    target: 'esnext',
  },
});
