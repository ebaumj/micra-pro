import authorize from '../../utils/authorize';
import * as webhooks from '../../utils/webhooks';
import { throwInternalServerError } from '../../utils/errors';

export default defineEventHandler(async (event) => {
  const webhookName = getRouterParam(event, 'webhookName');
  authorize(event.headers.get('authorization'), webhookName);
  try {
    await webhooks.deleteWebhookAsync(webhookName);
    return { content: null };
  } catch {
    return throwInternalServerError();
  }
});
