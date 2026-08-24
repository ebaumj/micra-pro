/// <reference types="vitest" />
import { defineConfig } from 'vite';
import dts from 'vite-plugin-dts';
import path from 'path';

export default defineConfig({
  root: import.meta.dirname,
  cacheDir: '../../../node_modules/.vite/recipe-hub-data-definition',
  plugins: [
    dts({
      entryRoot: 'src',
      tsconfigPath: path.posix.join(import.meta.dirname, 'tsconfig.lib.json'),
    }),
  ],
  resolve: {
    tsconfigPaths: true,
  },
  build: {
    outDir: '../../../dist/libs/recipe-hub/data-access',
    emptyOutDir: true,
    reportCompressedSize: true,
    commonjsOptions: { transformMixedEsModules: true },
    lib: {
      entry: 'src/index.ts',
      name: 'recipe-hub-data-definition',
      fileName: 'index',
      formats: ['es'],
    },
    rollupOptions: {
      external: [],
    },
    target: 'esnext',
  },
});
