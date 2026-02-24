import type { Plugin } from 'vite';
import fs from 'node:fs';
import path from 'node:path';
import { verifyConfig } from '../src/config.type';

export const appconfigPlugin = (): Plugin => {
  return {
    name: 'Appconfig Check',
    buildStart: () => {
      const source = path.resolve(__dirname, '../public/appconfig.json');
      if (!fs.existsSync(source)) throw new Error('Appconfig not found');
      try {
        verifyConfig(JSON.parse(fs.readFileSync(source, 'utf-8')));
      } catch {
        throw new Error('Appconfig format is wrong');
      }
    },
  };
};
