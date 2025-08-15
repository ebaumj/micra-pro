import authorize from '../../../utils/authorize';
import * as blobStorage from '@vercel/blob';
import * as fs from 'fs';
import { promises as FileStream } from 'fs';

export default defineEventHandler(async (event) => {
  const runtimeConfig = useRuntimeConfig();
  authorize(event.headers.get('authorization'));
  if (runtimeConfig.blobStorage.host === 'vercelBlob') {
    const allBlobs = await blobStorage.list();
    return {
      Assets: allBlobs.blobs.map(
        (b) =>
          b.pathname
            .replace(`${runtimeConfig.blobStorage.folder}/`, '')
            .split('.')[0],
      ),
    };
  } else if (runtimeConfig.blobStorage.host === 'localFile') {
    if (!fs.existsSync(runtimeConfig.blobStorage.folder)) return { Assets: [] };
    const allFiles = (
      await FileStream.readdir(runtimeConfig.blobStorage.folder)
    ).filter(
      (c) =>
        !fs.lstatSync(`${runtimeConfig.blobStorage.folder}/${c}`).isDirectory(),
    );
    return {
      Assets: allFiles.map((f) => f.split('.')[0]),
    };
  } else return throwInternalServerError();
});
