import { ExecutorContext } from '@nx/devkit';
import { RecursiveImportExecutorSchema } from './schema';
import * as fs from 'fs';
import * as path from 'path';

export default async function runExecutor(
  options: RecursiveImportExecutorSchema,
  context: ExecutorContext,
) {
  const outputFile = path.posix.join(context.root, options.outputFile);
  const startDir = path.posix.join(context.root, options.importDir);
  const relativePath = path.posix.relative(
    path.posix.dirname(outputFile),
    startDir,
  );

  const { imports, map } = buildImportMap(startDir, relativePath);

  const file = `${imports.join('\n')}


export default ${generateCode(map)}

`;

  // Ensure the directory exists
  const dirPath = path.posix.dirname(outputFile);
  if (!fs.existsSync(dirPath)) {
    fs.mkdirSync(dirPath, { recursive: true });
  }

  fs.writeFileSync(outputFile, file);

  return {
    success: true,
  };
}

function generateCode(map: ImportMap, indent = '    '): string {
  const lines: string[] = [];

  function buildObject(obj: ImportMap, level: number) {
    lines.push(`${' '.repeat((level - 1) * indent.length)}{`);
    for (const key in obj) {
      const value = obj[key];
      if (typeof value === 'string') {
        lines.push(`${' '.repeat(level * indent.length)}"${key}": ${value},`);
      } else {
        lines.push(`${' '.repeat(level * indent.length)}"${key}": `);
        buildObject(value as ImportMap, level + 1);
      }
    }
    lines.push(
      `${' '.repeat((level - 1) * indent.length)}}${level === 1 ? ';' : ','}`,
    );
  }

  buildObject(map, 1);
  return lines.join('\n');
}

interface ImportMap {
  [key: string]: string | ImportMap;
}

function buildImportMap(
  dirPath: string,
  relativeImport: string,
  basePath = '',
): { imports: string[]; map: ImportMap } {
  const importMap: ImportMap = {};
  const imports: string[] = [];

  const items = fs.readdirSync(dirPath).filter((file) => !file.startsWith('.'));

  items.forEach((item) => {
    const fullPath = path.posix.join(dirPath, item);
    const relativePath = path.posix.join(basePath, item);

    if (fs.statSync(fullPath).isDirectory()) {
      const subMap = buildImportMap(fullPath, relativeImport, relativePath);
      importMap[item] = subMap.map;
      imports.push(...subMap.imports);
    } else {
      const importName = toValidIdentifier(relativePath);
      const importPath = path.posix.join(relativeImport, relativePath);
      imports.push(`import ${importName} from "${importPath}";`);
      importMap[path.posix.parse(item).name] = importName;
    }
  });

  return { imports, map: importMap };
}

function toValidIdentifier(filePath: string): string {
  return filePath
    .replace(/[^a-zA-Z0-9_]/g, '_') // Replace non-alphanumeric characters with '_'
    .replace(/^(\d)/, '_$1'); // Prefix with '_' if it starts with a number
}
