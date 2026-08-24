/// <reference types="vitest" />
import { defineConfig } from 'vite';
import solidPlugin from 'vite-plugin-solid';
import dts from 'vite-plugin-dts';
import path from 'path';

export default defineConfig({
  root: import.meta.dirname,
  cacheDir: '../../../node_modules/.vite/bean-management-data-access',
  plugins: [
    dts({
      entryRoot: 'src',
      tsconfigPath: path.posix.join(import.meta.dirname, 'tsconfig.lib.json'),
    }),
    solidPlugin(),
  ],
  resolve: {
    tsconfigPaths: true,
  },
  build: {
    outDir: '../../../dist/libs/bean-management/data-access',
    emptyOutDir: true,
    reportCompressedSize: true,
    commonjsOptions: { transformMixedEsModules: true },
    lib: {
      entry: 'src/index.ts',
      name: 'bean-management-data-access',
      fileName: 'index',
      formats: ['es'],
    },
    rollupOptions: {
      external: [],
    },
    target: 'esnext',
  },
});
