import { throwInternalServerError } from '../../utils/errors';
import authorize from '../../utils/authorize';
import * as fs from 'fs';
import { promises as FileStream } from 'fs';
import useAppconfig from '../../utils/appconfig';

export default defineEventHandler(async (event) => {
  const runtimeConfig = useAppconfig();
  const id = getRouterParam(event, 'id');
  if (!id) return throwInternalServerError();
  authorize(event.headers.get('authorization'), id);
  if (!fs.existsSync(runtimeConfig.blobStorage.folder))
    throwInternalServerError();
  const allFiles = (
    await FileStream.readdir(runtimeConfig.blobStorage.folder)
  ).filter(
    (c) =>
      !fs.lstatSync(`${runtimeConfig.blobStorage.folder}/${c}`).isDirectory(),
  );
  const file = allFiles.find((f) => f.includes(id!));
  if (!file) return throwInternalServerError();
  const content = await FileStream.readFile(
    `${runtimeConfig.blobStorage.folder}/${file}`,
  );
  return {
    DataBase64: content.toString('base64'),
    FileExtension: file.split('.')[1],
  };
});
