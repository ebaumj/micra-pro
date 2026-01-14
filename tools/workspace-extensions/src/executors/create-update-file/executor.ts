import { CreateUpdateFileExecutorSchema } from './schema';
import type { ExecutorContext } from '@nx/devkit';
import * as path from 'path';
import * as fs from 'fs';
import * as crypto from 'crypto';
const AdmZip = require('adm-zip');

const AddFiles = (
  source: string,
  root: string,
  folder: string,
  zip: typeof AdmZip,
) => {
  fs.readdirSync(source).forEach((e) => {
    const p = path.join(source, e);
    if (fs.statSync(p).isFile())
      zip.addFile(path.join(folder, e), fs.readFileSync(p));
    if (fs.statSync(p).isDirectory())
      AddFiles(p, root, path.join(folder, e), zip);
  });
};

export default async function runExecutor(
  options: CreateUpdateFileExecutorSchema,
  context: ExecutorContext,
) {
  const zip = new AdmZip();
  const backend = path.join(context.root, options.applicationPaths.backend);
  const frontend = path.join(context.root, options.applicationPaths.frontend);
  const asset_server = path.join(
    context.root,
    options.applicationPaths.asset_server,
  );
  const output = path.join(context.root, options.outputPath);
  AddFiles(backend, backend, 'backend', zip);
  AddFiles(frontend, frontend, 'frontend', zip);
  AddFiles(asset_server, asset_server, 'asset-server', zip);
  if (!fs.existsSync(output)) fs.mkdirSync(output, { recursive: true });
  const file = path.join(output, 'mp-apps.zip');
  zip.writeZip(file);
  const signature = crypto
    .createSign('SHA256')
    .update(fs.readFileSync(file))
    .sign(
      fs.readFileSync(path.join(context.root, options.signingKey)),
      'base64',
    );
  fs.writeFileSync(path.join(output, 'signature.txt'), signature, {
    encoding: 'utf-8',
  });
  return {
    success: true,
  };
}
