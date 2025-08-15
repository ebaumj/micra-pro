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
    const response = await fetch(blob!.downloadUrl);
    const blobData = await response.blob();
    if (!blobData) throwInternalServerError();
    const buffer = await blobData.arrayBuffer();
    return {
      DataBase64: Buffer.from(buffer).toString('base64'),
      FileExtension: blob!.pathname.split('.')[1],
    };
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
    const content = await FileStream.readFile(
      `${runtimeConfig.blobStorage.folder}/${file}`,
    );
    return {
      DataBase64: content.toString('base64'),
      FileExtension: file.split('.')[1],
    };
  } else return throwInternalServerError();
});
