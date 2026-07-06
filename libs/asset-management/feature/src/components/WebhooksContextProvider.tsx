import {
  createContext,
  createEffect,
  ParentComponent,
  useContext,
} from 'solid-js';
import { webhooksAvailable } from '@micra-pro/asset-management/data-access';

const WebhooksContext = createContext<{ isAvailable: boolean }>();

export const useWebhooksContext = () => {
  const ctx = useContext(WebhooksContext);
  if (!ctx) throw new Error("Can't find Webhooks Context!");
  return ctx;
};

export const WebhooksContextProvider: ParentComponent<{}> = (props) => {
  const value = {
    isAvailable: false,
  };
  const isAvailable = webhooksAvailable();
  createEffect(() => (value.isAvailable = isAvailable()));
  return (
    <WebhooksContext.Provider value={value}>
      {props.children}
    </WebhooksContext.Provider>
  );
};
