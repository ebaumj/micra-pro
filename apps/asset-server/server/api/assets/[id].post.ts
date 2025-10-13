import { throwInternalServerError } from '../../utils/errors';
import { authorizeId } from '../../utils/authorize';
import * as fs from 'fs';
import { promises as FileStream } from 'fs';

export default defineEventHandler(async (event) => {
  const runtimeConfig = useRuntimeConfig();
  authorizeId(event.headers.get('authorization'), getRouterParam(event, 'id'));
  const formData = await readFormData(event);
  const fileName = `${runtimeConfig.blobStorage.folder}/${formData.get('fname')}`;
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
});
