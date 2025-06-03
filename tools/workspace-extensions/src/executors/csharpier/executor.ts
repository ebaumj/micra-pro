import { CsharpierExecutorSchema } from './schema';
import type { ExecutorContext } from '@nx/devkit';
import { exec } from 'child_process';
import { promisify } from 'util';
import path = require('path');

export default async function runExecutor(
  options: CsharpierExecutorSchema,
  context: ExecutorContext,
) {
  console.log('Executor ran for Csharpier', options);

  const projectRoot = path.join(
    context.root,
    context.projectsConfigurations.projects[context.projectName].root,
  );

  await promisify(exec)('dotnet tool restore');

  await promisify(exec)(
    `dotnet csharpier ${options.write ? 'format' : 'check'} ${projectRoot}`,
  );

  return {
    success: true,
  };
}
