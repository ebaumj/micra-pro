import { Accessor } from 'solid-js';

export const testConnection = (): {
  connectionState: Accessor<'loading' | 'connected' | 'error'>;
  refetch: () => void;
} => {
  return {
    connectionState: () => 'connected',
    refetch: () => undefined,
  };
};
