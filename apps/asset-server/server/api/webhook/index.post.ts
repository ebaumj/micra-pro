import authorize from '../../utils/authorize';
import * as webhooks from '../../utils/webhooks';
import vm from 'node:vm';

const tokeSubject = 'WebhookInvoke';

const createScriptCode = (payload: string) => `(async () => {
  async function execute(machineEvent, timestamp) {
    ${payload}
  }
  await execute(payloadData, timestampData);
})()`;

export default defineEventHandler(async (event) => {
  authorize(event.headers.get('authorization'), tokeSubject);
  const body = (await readBody(event)) as {
    events: { webhookName: string; payload: any; timestamp: string }[];
  };
  var events = await Promise.all(
    body.events.map(async (ev) => ({
      ...ev,
      content: await webhooks.readWebhookAsync(ev.webhookName),
    })),
  );
  const backgroundTasks = events.map((ev) =>
    invokeWebhook(ev.webhookName, ev.payload, ev.timestamp, ev.content),
  );
  event.waitUntil(Promise.all(backgroundTasks));
});

const invokeWebhook = async (
  name: string,
  payload: string,
  timestamp: string,
  content: string | null,
) => {
  try {
    if (!content) return;
    const sandbox = {
      payloadData: payload,
      timestampData: new Date(timestamp),
      console: console,
      fetch: fetch,
      URL: URL,
    };
    vm.createContext(sandbox);
    const timeoutPromise = new Promise((_, reject) =>
      setTimeout(() => reject(new Error('Async execution timed out')), 10000),
    );
    await Promise.race([
      vm.runInContext(createScriptCode(content), sandbox),
      timeoutPromise,
    ]);
  } catch (error) {
    console.error(`Error executing webhook '${name}':`, error);
  }
};
