import { DeployUpdateExecutorSchema } from './schema';
import type { ExecutorContext } from '@nx/devkit';
import * as path from 'path';
import * as fs from 'fs';
import { Pool } from 'pg';
import { put } from '@vercel/blob';

const uploadToDatabase = async (
  version: string,
  link: string,
  signature: string,
) => {
  const pool = new Pool({
    connectionString: process.env.DATABASE_URL,
    ssl: {
      rejectUnauthorized: false,
    },
  });
  const client = await pool.connect();
  await client.query(
    `INSERT INTO update (version, link, signature) VALUES ('${version}', '${link}', '${signature}')`,
  );
};

const uploadFile = async (version: string, file: string) => {
  const { url } = await put(
    `release/micra-pro-${version}.zip`,
    fs.readFileSync(file),
    { access: 'public' },
  );
  return url;
};

type AppsettingsType = {
  MicraPro: {
    Shared: {
      Infrastructure: {
        SystemVersion: string;
      };
    };
  };
};

export default async function runExecutor(
  options: DeployUpdateExecutorSchema,
  context: ExecutorContext,
) {
  const appSettings = JSON.parse(
    fs.readFileSync(path.join(context.root, options.appSettingsFile), {
      encoding: 'utf-8',
    }),
  ) as AppsettingsType;
  const version = appSettings.MicraPro.Shared.Infrastructure.SystemVersion;
  const signature = fs.readFileSync(
    path.join(context.root, options.signatureFile),
    { encoding: 'utf-8' },
  );
  const link = await uploadFile(
    version,
    path.join(context.root, options.updateFile),
  );
  await uploadToDatabase(version, link, signature);
  return {
    success: true,
  };
}
