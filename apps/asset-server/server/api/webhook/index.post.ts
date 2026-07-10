import authorize from '../../utils/authorize';
import * as webhooks from '../../utils/webhooks';
import vm from 'node:vm';

const tokeSubject = 'WebhookInvoke';

const createScriptCode = (
  payload: string,
) => `function execute(machineEvent, timestamp) {
    ${payload}
  }
  execute(payloadData, timestampData);
`;

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
  const backgroundTasks = events.map((ev) => {
    return new Promise<void>((resolve) =>
      setImmediate(() => {
        invokeWebhook(ev.webhookName, ev.payload, ev.timestamp, ev.content);
        resolve();
      }),
    );
  });
  event.waitUntil(Promise.all(backgroundTasks));
});

const invokeWebhook = (
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
    };
    vm.createContext(sandbox);
    vm.runInContext(createScriptCode(content), sandbox, { timeout: 10000 });
  } catch (error) {
    console.error(`Error executing webhook '${name}':`, error);
  }
};
