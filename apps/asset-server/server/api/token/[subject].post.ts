import authorize, { refresh } from '../../utils/authorize';

export default defineEventHandler(async (event) => {
  const subject = getRouterParam(event, 'subject');
  const validAccessToken = authorize(
    event.headers.get('authorization'),
    subject,
  );
  const body = await readBody<{ refreshToken: string }>(event);
  return refresh(validAccessToken, body.refreshToken);
});
