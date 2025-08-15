import { throwInternalServerError } from '../../../utils/errors';
import authorize from '../../../utils/authorize';
import * as blob from '@vercel/blob';
import * as fs from 'fs';
import { promises as FileStream } from 'fs';

export default defineEventHandler(async (event) => {
  const runtimeConfig = useRuntimeConfig();
  const id = getRouterParam(event, 'id');
  if (!id) return throwInternalServerError();
  authorize(event.headers.get('authorization'), id);
  const formData = await readFormData(event);
  const fileName = `${runtimeConfig.blobStorage.folder}/${formData.get('fname')}`;
  if (runtimeConfig.blobStorage.host === 'vercelBlob') {
    const file = formData.get('data') as Blob;
    try {
      blob.put(fileName, file, {
        access: 'public',
        allowOverwrite: true,
      });
      return true;
    } catch {
      return throwInternalServerError();
    }
  } else if (runtimeConfig.blobStorage.host === 'localFile') {
    try {
      if (!fs.existsSync(runtimeConfig.blobStorage.folder))
        await FileStream.mkdir(runtimeConfig.blobStorage.folder);
      const file = formData.get('data') as Blob;
      const buffer = await file.arrayBuffer();
      fs.writeFileSync(fileName, Buffer.from(buffer));
      return true;
    } catch {
      return throwInternalServerError();
    }
  } else return throwInternalServerError();
});
