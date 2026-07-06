import authorize from '../../utils/authorize';
import * as webhooks from '../../utils/webhooks';

const tokeSubject = 'WebhookSchema';

export default defineEventHandler(async (event) => {
  authorize(event.headers.get('authorization'), tokeSubject);
  const body = (await readBody(event)) as { events: webhooks.WebhookSchema[] };
  await webhooks.writeSchemaAsync(body.events);
});
