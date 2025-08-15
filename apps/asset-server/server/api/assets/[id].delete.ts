import { throwInternalServerError } from '../../../utils/errors';
import authorize from '../../../utils/authorize';
import * as blobStorage from '@vercel/blob';
import * as fs from 'fs';
import { promises as FileStream } from 'fs';

export default defineEventHandler(async (event) => {
  const runtimeConfig = useRuntimeConfig();
  const id = getRouterParam(event, 'id');
  if (!id) return throwInternalServerError();
  authorize(event.headers.get('authorization'), id);
  if (runtimeConfig.blobStorage.host === 'vercelBlob') {
    const allBlobs = await blobStorage.list();
    const blob = allBlobs.blobs.find((b) => b.pathname.includes(id!));
    if (!blob) throwInternalServerError();
    blobStorage.del(blob!.pathname);
  } else if (runtimeConfig.blobStorage.host === 'localFile') {
    if (!fs.existsSync(runtimeConfig.blobStorage.folder))
      throwInternalServerError();
    const allFiles = (
      await FileStream.readdir(runtimeConfig.blobStorage.folder)
    ).filter(
      (c) =>
        !fs.lstatSync(`${runtimeConfig.blobStorage.folder}/${c}`).isDirectory(),
    );
    const file = allFiles.find((f) => f.includes(id!));
    if (!file) throwInternalServerError();
    await FileStream.unlink(`${runtimeConfig.blobStorage.folder}/${file}`);
  } else return throwInternalServerError();
});
