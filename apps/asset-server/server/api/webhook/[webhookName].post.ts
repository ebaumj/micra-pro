import authorize from '../../utils/authorize';
import * as webhooks from '../../utils/webhooks';
import { throwInternalServerError } from '../../utils/errors';

export default defineEventHandler(async (event) => {
  const webhookName = getRouterParam(event, 'webhookName');
  authorize(event.headers.get('authorization'), webhookName);
  try {
    const body = await readBody<{ content: string }>(event);
    await webhooks.writeWebhookAsync(webhookName, body.content);
    return { content: body.content };
  } catch {
    return throwInternalServerError();
  }
});
