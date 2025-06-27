import {
  RebootDocument,
  RebootMutation,
  RebootMutationVariables,
  ShutdownDocument,
  ShutdownMutation,
  ShutdownMutationVariables,
} from '../generated/graphql';
import { createMutation } from '../graphql-client/createMutation';

export const createSystemAccessor = (): {
  reboot: () => Promise<void>;
  shutdown: () => Promise<void>;
} => {
  const rebootMutation = createMutation<
    RebootMutation,
    RebootMutationVariables
  >(RebootDocument);
  const shutdownMutation = createMutation<
    ShutdownMutation,
    ShutdownMutationVariables
  >(ShutdownDocument);
  async function systemReboot(): Promise<void> {
    const result = await rebootMutation({});
    if (result.reboot.boolean === true) return Promise.resolve();
    return Promise.reject();
  }
  async function systemShutdown(): Promise<void> {
    const result = await shutdownMutation({});
    if (result.shutdown.boolean === true) return Promise.resolve();
    return Promise.reject();
  }
  return {
    reboot: () => systemReboot(),
    shutdown: () => systemShutdown(),
  };
};
