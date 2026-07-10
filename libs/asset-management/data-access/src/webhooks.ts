import { createQuery } from '@micra-pro/shared/utils-ts';
import { Accessor, createEffect } from 'solid-js';
import {
  AvailableWebhooksDocument,
  AvailableWebhooksQuery,
  AvailableWebhooksQueryVariables,
  Webhook,
  WebhooksAvailableDocument,
  WebhooksAvailableQuery,
  WebhooksAvailableQueryVariables,
} from './generated/graphql';

export const getWebhooksAsync = async (): Promise<Webhook[]> => {
  const query = createQuery<
    AvailableWebhooksQuery,
    AvailableWebhooksQueryVariables
  >(AvailableWebhooksDocument, () => ({}));
  return new Promise((resolve, reject) => {
    createEffect(() => {
      switch (query.resource.state) {
        case 'unresolved':
        case 'errored':
          reject();
          break;
        case 'pending':
        case 'refreshing':
          break;
        case 'ready':
          resolve(query.resource.latest?.availableWebhooks);
      }
    });
  });
};

export const webhooksAvailable = (): Accessor<boolean> => {
  const query = createQuery<
    WebhooksAvailableQuery,
    WebhooksAvailableQueryVariables
  >(WebhooksAvailableDocument, () => ({}));
  return () => query.resource.latest?.webhooksAvailable ?? false;
};
