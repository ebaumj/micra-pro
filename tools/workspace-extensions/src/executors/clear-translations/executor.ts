import { ClearTranslationsExecutorSchema } from './schema';
import type { ExecutorContext } from '@nx/devkit';
import * as path from 'path';
import * as fs from 'fs';
import { promises as FileStream } from 'fs';

export default async function runExecutor(
  options: ClearTranslationsExecutorSchema,
  context: ExecutorContext,
) {
  const localesDir = path.join(context.root, 'locale');

  if (fs.existsSync(localesDir)) {
    const allFiles = await FileStream.readdir(localesDir);
    if (options.write)
      await Promise.all(
        allFiles.map(async (f) => FileStream.unlink(path.join(localesDir, f))),
      );
    else if (allFiles.length > 0)
      throw new Error('Run command "npm run import-translations" before merge');
  }

  return {
    success: true,
  };
}
