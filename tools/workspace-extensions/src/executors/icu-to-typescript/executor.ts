import { IcuToTypescriptExecutorSchema } from './schema';
import {
  MessageFormatElement,
  parse,
  TYPE,
} from '@formatjs/icu-messageformat-parser';
import * as fs from 'fs';
import * as path from 'path';
import * as lodash from 'lodash';

export async function runExecutor(options: IcuToTypescriptExecutorSchema) {
  const en = JSON.parse(fs.readFileSync(options.inputFile, 'utf-8'));

  let outputFile = renderImports(options.localesFolderRelativeToOutputFile);

  const keysWithProperties: { key: string; propertiesInterface: string }[] = [];
  const keysWithoutProperties: string[] = [];

  Object.keys(en).forEach((keyWithNamespace) => {
    if (!keyWithNamespace.startsWith(options.namespace + '.')) {
      throw new Error(
        `Key "${keyWithNamespace}" does not start with the namespace "${options.namespace}"`,
      );
    }

    const key = keyWithNamespace.replace(options.namespace + '.', '');
    const element = en[keyWithNamespace];

    const value =
      typeof element === 'string' ? element : element.defaultMessage;

    if (typeof value !== 'string') {
      console.error(`Value for key "${key}" is not a string: ${value}`);
      return;
    }

    let ast: MessageFormatElement[];

    try {
      ast = parse(value);
    } catch (e) {
      throw new Error(
        `Error parsing the value ${value} for key ${key}. Inner Error: ${e.message}`,
      );
    }

    const properties = ast
      .map((node) => {
        switch (node.type) {
          case TYPE.literal:
            return null;
          case TYPE.argument:
            return { name: node.value, type: 'string' };
          case TYPE.number:
            return { name: node.value, type: 'number' };
          case TYPE.date:
          case TYPE.time:
            return { name: node.value, type: 'Date' };
          case TYPE.select:
            return {
              name: node.value,
              type: Object.keys(node.options)
                .map((option) => `'${option}'`)
                .join('|'),
            };
          case TYPE.plural:
            return { name: node.value, type: 'number' };
          default:
            console.error(`Unhandled node type: ${node.type}`);
            return null;
        }
      })
      .filter((prop) => prop !== null)
      .reduce(
        (acc, property) => {
          const indexOfOtherPropertyWithSameName = acc.findIndex(
            (otherProperty) => otherProperty.name === property.name,
          );

          // When there's no other property with the same name
          // -> add the property to the list
          if (indexOfOtherPropertyWithSameName < 0) {
            return [...acc, property];
          }

          // When there's a property with the same name and same type
          // it should not be added again, so return the same list
          if (acc[indexOfOtherPropertyWithSameName].type === property.type) {
            return acc;
          }

          // Multiple properties with the same name but different types
          throw new Error(
            `Found multiple properties with name ${property.name} for key ${key}.`,
          );
        },
        [] as { name: string; type: string }[],
      );

    if (properties.length > 0) {
      keysWithProperties.push({
        key,
        propertiesInterface: 'PropertiesType' + toPascalCase(key),
      });
      outputFile += renderInterface(
        'PropertiesType' + toPascalCase(key),
        properties,
      );
    } else {
      keysWithoutProperties.push(key);
    }
  });

  outputFile += `
export type KeysWithoutProperties = ${
    keysWithoutProperties.map((k) => `'${k}'`).join(' | ') || 'never'
  };\n`;

  outputFile += `
// eslint-disable-next-line @typescript-eslint/no-empty-interface, @typescript-eslint/no-empty-object-type
${renderInterface(
  'KeysWithProperties',
  keysWithProperties.map((k) => ({
    name: k.key,
    type: k.propertiesInterface,
  })),
)}`;

  outputFile += `
${renderTFunction}

${renderUseTranslationContext(options.namespace)}

${renderGetTranslationConfig(options.namespace)}

${renderTComponent}
`;

  // Ensure the directory exists
  const dirPath = path.dirname(options.outputFile);
  if (!fs.existsSync(dirPath)) {
    fs.mkdirSync(dirPath, { recursive: true });
  }

  fs.writeFileSync(options.outputFile, outputFile);

  return {
    success: true,
  };
}

const renderImports = (
  localesImportPath: string,
) => `import { useNamespaceTranslationContext, parseImport } from '@micra-pro/shared/utils-ts';
import { Component } from 'solid-js'
const locales = import.meta.glob('${localesImportPath}*.json', { eager: true });
`;

const renderInterface = (
  name: string,
  properties: { name: string; type: string }[],
) => {
  return `export interface ${name} {
${properties.map((p) => `    "${p.name}": ${p.type};`).join('\n')}
}
`;
};

const renderTFunction = `// Converts a union of object types into an intersection of object types
type UnionToIntersection<T> = (T extends any ? (x: T) => void : never) extends (
    x: infer R,
) => void
    ? R
    : never;

type TFunction = {
    <TKey extends keyof KeysWithProperties>(
        key: TKey,
        values: UnionToIntersection<KeysWithProperties[TKey]>
    ): string;
    <TKey extends KeysWithoutProperties>(key: TKey): string;
};`;

const renderUseTranslationContext = (
  namespace: string,
) => `export function useTranslationContext() {
    return useNamespaceTranslationContext<TFunction>('${namespace}');
}`;

const renderGetTranslationConfig = (
  namespace: string,
) => `export function Get${toPascalCase(namespace)}TranslationConfig() {
    return parseImport('${namespace}', locales);
}`;

const renderTComponent = `type PropsWithProperties<TKey extends keyof KeysWithProperties> = {
    key: TKey;
    values: KeysWithProperties[TKey];
};

type PropsWithoutProperties = { key: KeysWithoutProperties };

export type TranslationComponentProps = PropsWithProperties<keyof KeysWithProperties> | PropsWithoutProperties;

export const T: Component<TranslationComponentProps> = (
    props
) => {
    const { t } = useTranslationContext();
    // eslint-disable-next-line @typescript-eslint/ban-ts-comment
    // @ts-ignore
    return <>{t(props.key, props.values)}</>;
};`;

const toPascalCase = (str: string) => {
  const camelCaseStr = lodash.camelCase(str);
  return camelCaseStr.charAt(0).toUpperCase() + camelCaseStr.slice(1);
};

export default runExecutor;
