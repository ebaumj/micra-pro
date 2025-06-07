import useAuthorize from '../../../composables/useAuthorize';
import useInternalServerError from '../../../composables/useInternalServerError';
import * as blob from '@vercel/blob';
import { fromBlob } from 'image-resize-compress';

const maxFileSizeInBytes = 100000;

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id');
  if (!id) return useInternalServerError();
  useAuthorize(event.headers.get('authorization'), id);
  const formData = await readFormData(event);
  const fileName = `images/${formData.get('fname')}`;
  const file = formData.get('data') as Blob;
  let compressedFile =
    file.size > maxFileSizeInBytes
      ? await fromBlob(
          file,
          (maxFileSizeInBytes * 100) / file.size,
          'auto',
          'auto',
          'jpeg',
        )
      : file;
  try {
    blob.put(fileName, file, {
      access: 'public',
      allowOverwrite: true,
    });
    return true;
  } catch {
    return useInternalServerError();
  }
});
