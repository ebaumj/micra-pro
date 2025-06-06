import useAuthorize from '../../../composables/useAuthorize';
import useInternalServerError from '../../../composables/useInternalServerError';
import * as blob from '@vercel/blob';

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id');
  if (!id) return useInternalServerError();
  useAuthorize(event.headers.get('authorization'), id);
  const formData = await readFormData(event);
  const fname = formData.get('fname') as string;
  const fileName = `images/${id}.${fname.split('.').pop()}`;
  const file = formData.get('data') as Blob;
  try {
    blob.put(fileName, file, { access: 'public', allowOverwrite: true });
    return true;
  } catch {
    return useInternalServerError();
  }
});
