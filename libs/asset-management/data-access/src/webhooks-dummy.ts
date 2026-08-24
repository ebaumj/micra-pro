import { Accessor } from 'solid-js';
import { Webhook } from './generated/graphql';

export const getWebhooksAsync = async (): Promise<Webhook[]> => {
  return new Promise((resolve) => resolve([]));
};

export const webhooksAvailable = (): Accessor<boolean> => {
  return () => false;
};
