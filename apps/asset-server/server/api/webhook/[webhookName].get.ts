import authorize from '../../utils/authorize';
import * as webhooks from '../../utils/webhooks';
import { throwInternalServerError } from '../../utils/errors';

export default defineEventHandler(async (event) => {
  const webhookName = getRouterParam(event, 'webhookName');
  authorize(event.headers.get('authorization'), webhookName);
  try {
    const schema = await webhooks.readSchemaAsync(webhookName);
    return {
      content: await webhooks.readWebhookAsync(webhookName),
      eventFormat: `const machineEvent = ${schema.defaultPayloadValue};const timestamp = new Date();`,
    };
  } catch {
    return throwInternalServerError();
  }
});
