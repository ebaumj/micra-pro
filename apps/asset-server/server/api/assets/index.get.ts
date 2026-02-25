import authorize from '../../utils/authorize';
import * as fs from 'fs';
import { promises as FileStream } from 'fs';
import useAppconfig from '../../utils/appconfig';

export default defineEventHandler(async (event) => {
  const runtimeConfig = useAppconfig();
  authorize(event.headers.get('authorization'));
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
});
