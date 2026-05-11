import { throwInternalServerError } from '../../utils/errors';
import authorize from '../../utils/authorize';

type RestorePayload = {
  server: string;
  directory: string;
  username: string;
  password: string;
};

const checkPayload = (payload: RestorePayload) => {
  if (!payload.server || typeof payload.server !== 'string') return false;
  if (!payload.directory || typeof payload.directory !== 'string') return false;
  if (!payload.username || typeof payload.username !== 'string') return false;
  if (!payload.password || typeof payload.password !== 'string') return false;
  return true;
};

export default defineEventHandler(async (event) => {
  authorize(event.headers.get('authorization'));
  const payload = await readBody<RestorePayload>(event);
  if (!checkPayload(payload)) return throwInternalServerError();
  try {
    await restoreAssets(
      payload.server,
      payload.directory,
      payload.username,
      payload.password,
    );
  } catch {
    return throwInternalServerError();
  }
});
