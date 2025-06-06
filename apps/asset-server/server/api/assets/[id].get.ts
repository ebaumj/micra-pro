import useInternalServerError from '../../../composables/useInternalServerError';
import useAuthorize from '../../../composables/useAuthorize';
import * as blobStorage from '@vercel/blob';

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id');
  if (!id) return useInternalServerError();
  useAuthorize(event.headers.get('authorization'), id);
  const allBlobs = await blobStorage.list();
  const blob = allBlobs.blobs.find((b) => b.pathname.includes(id!));
  if (!blob) useInternalServerError();
  const response = await fetch(blob!.downloadUrl);
  const blobData = await response.blob();
  if (!blobData) useInternalServerError();
  const buffer = await blobData.arrayBuffer();
  console.log(buffer.byteLength);
  return {
    DataBase64: Buffer.from(buffer).toString('base64'),
    FileExtension: blob!.pathname.split('.')[1],
  };
});
