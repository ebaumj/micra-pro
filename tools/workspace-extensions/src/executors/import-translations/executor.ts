import { ImportTranslationsExecutorSchema } from './schema';
import * as fs from 'fs';
import * as path from 'path';
import { promises as FileStream } from 'fs';
import { ExecutorContext } from 'nx/src/devkit-exports';

export async function runExecutor(
  options: ImportTranslationsExecutorSchema,
  context: ExecutorContext,
) {
  const inputPath = path.join(context.root, 'locale');
  const outputPath = path.join(context.root, options.outputFolder);
  const locales = await Promise.all(
    (await FileStream.readdir(inputPath)).map(async (f) => ({
      content: Object.entries(
        JSON.parse(
          (await FileStream.readFile(path.join(inputPath, f))).toString(),
        )[options.namespace],
      ).map((e) => ({
        key: `${options.namespace}.${e[0]}`,
        value: e[1] as string,
      })),
      fileName: f,
    })),
  );
  console.log(locales[0].content);
  await Promise.all(
    locales.map(async (locale) => {
      const outputFile = path.join(outputPath, locale.fileName);
      let current = {};
      if (fs.existsSync(outputFile)) {
        current = JSON.parse(
          (await FileStream.readFile(outputFile)).toString(),
        );
        await FileStream.unlink(outputFile);
      }
      await FileStream.writeFile(
        outputFile,
        JSON.stringify(
          locale.content.reduce((a, v) => {
            a[v.key] = v.value;
            return a;
          }, current),
          null,
          2,
        ),
      );
    }),
  );
  return {
    success: true,
  };
}

export default runExecutor;
