import useAppconfig from './appconfig';
import * as fs from 'fs';
import { promises as File } from 'fs';

const schemaFile = '__schema.json';

export type WebhookSchema = {
  name: string;
  defaultPayloadValue: string;
};

const file = (name: string) => {
  const runtimeConfig = useAppconfig();
  if (!fs.existsSync(runtimeConfig.blobStorage.folder)) throw new Error();
  const directory = `${runtimeConfig.blobStorage.folder}/Webhooks`;
  if (!fs.existsSync(directory)) fs.mkdirSync(directory);
  return `${directory}/${name}`;
};

export const writeWebhookAsync = async (
  name: string | undefined,
  content: string,
) => {
  if (!name) throw new Error();
  await File.writeFile(file(`${name}.js`), content);
};

export const readWebhookAsync = async (
  name: string | undefined,
): Promise<string | null> => {
  if (!name) return null;
  try {
    return await File.readFile(file(`${name}.js`), 'utf-8');
  } catch {
    return null;
  }
};

export const writeSchemaAsync = async (content: WebhookSchema[]) => {
  await File.writeFile(file(schemaFile), JSON.stringify(content));
};

export const readSchemaAsync = async (
  name: string | undefined,
): Promise<WebhookSchema> => {
  if (!name) throw new Error();
  const schema = JSON.parse(
    await File.readFile(file(schemaFile), 'utf-8'),
  ) as WebhookSchema[];
  const hookSchema = schema.find((s) => s.name === name);
  if (!hookSchema) throw new Error();
  return hookSchema;
};

export const deleteWebhookAsync = async (
  name: string | undefined,
): Promise<void> => {
  if (!name) throw new Error();
  await File.unlink(file(name));
};
