import useAuthorize from '../../../composables/useAuthorize';
import * as blobStorage from '@vercel/blob';

export default defineEventHandler(async (event) => {
  useAuthorize(event.headers.get('authorization'));
  const allBlobs = await blobStorage.list();
  return {
    Assets: allBlobs.blobs.map(
      (b) => b.pathname.replace('images/', '').split('.')[0],
    ),
  };
});
