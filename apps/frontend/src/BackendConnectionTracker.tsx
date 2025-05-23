import { createEffect, createSignal, onCleanup } from 'solid-js';
import { ErrorDialog } from '@micra-pro/shared/ui';
import { testConnection } from '@micra-pro/shared/utils-ts';
import SplashScreen from './SplashScreen';

const pollIntervalMs = 2020;

export const BackendConnectionTracker = () => {
  const connectionTest = testConnection();
  const [backendWasEverConnected, setBackendWasEverConnected] =
    createSignal(false);

  createEffect(() => {
    if (connectionTest.connectionState() === 'connected')
      setBackendWasEverConnected(true);
  });

  const interval = setInterval(connectionTest.refetch, pollIntervalMs);

  onCleanup(() => clearInterval(interval));

  let errorOccurred = false;
  createEffect(() => {
    if (connectionTest.connectionState() === 'error') {
      errorOccurred = true;
    } else if (errorOccurred) {
      location.reload();
    }
  });

  return (
    <>
      <SplashScreen show={!backendWasEverConnected()} />
      <ErrorDialog
        open={
          backendWasEverConnected() &&
          connectionTest.connectionState() === 'error'
        }
        title="An unexpected error has occurred"
        description="Micra Pro is attempting to recover from an error.
            Please wait up to 1 minute. If the issue persists, power
            cycle the machine by turning it off, waiting for a few
            seconds, and then turning it back on."
      />
    </>
  );
};

export default BackendConnectionTracker;
