import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogTitle,
  Button,
  SpinnerButton,
} from '@micra-pro/shared/ui';
import { Component, createEffect, createSignal, Match, Switch } from 'solid-js';
import { useCleaningContext } from './CleaningContextProvider';
import { CleaningCycle } from '@micra-pro/cleaning/data-access';
import moment from 'moment';
import { T } from '../generated/language-types';

export const CleaningDialog: Component<{ cleaningParams: CleaningCycle[] }> = (
  props,
) => {
  const ctx = useCleaningContext();
  const [dialogOpen, setDialogOpen] = createSignal(false);
  const [cancelling, setCacncelling] = createSignal(false);
  createEffect(() => ctx.isRunning() && setDialogOpen(true));
  createEffect(
    () =>
      ctx.state().__typename === 'CleaningStateCancelled' &&
      setCacncelling(false),
  );
  const totalTime = () =>
    props.cleaningParams.reduce(
      (prev, curr) =>
        prev +
        moment.duration(curr.paddleOnTime).asSeconds() +
        moment.duration(curr.paddleOffTime).asSeconds(),
      0,
    );
  const currentTime = () => {
    const state = ctx.state();
    switch (state.__typename) {
      case 'CleaningStateCancelled':
      case 'CleaningStateFailed':
      case 'CleaningStateFinished':
        return moment.duration(state.totalTime).asSeconds();
      case 'CleaningStateRunning':
        return moment.duration(state.totalTime).asSeconds();
      case 'CleaningStateStarted':
        return 0;
    }
  };
  const running = () => {
    const state = ctx.state();
    switch (state.__typename) {
      case 'CleaningStateCancelled':
      case 'CleaningStateFailed':
        return 'stopped';
      case 'CleaningStateFinished':
        return 'finished';
      case 'CleaningStateRunning':
      case 'CleaningStateStarted':
        return 'running';
    }
  };
  return (
    <AlertDialog open={dialogOpen()}>
      <AlertDialogContent>
        <AlertDialogTitle autofocus>
          <div>
            <T key="cleaning" />
          </div>
        </AlertDialogTitle>
        <div class="flex w-full flex-col gap-2">
          <div class="bg-secondary flex h-2 w-full rounded-full border">
            <div
              class="bg-primary h-full rounded-full"
              style={{ 'min-width': `${(100 * currentTime()) / totalTime()}%` }}
            />
          </div>
        </div>
        <div class="flex w-full justify-center">
          <Switch>
            <Match when={running() === 'running'}>
              <SpinnerButton
                variant="outline"
                class="w-44"
                loading={cancelling()}
                disabled={cancelling()}
                onClick={() => setCacncelling(true) && ctx.stop()}
              >
                <T key="cancel" />
              </SpinnerButton>
            </Match>
            <Match when={running() === 'finished'}>
              <Button
                class="shadow-positive w-44 shadow-lg"
                onClick={() => setDialogOpen(false)}
              >
                <T key="close" />
              </Button>
            </Match>
            <Match when={running() === 'stopped'}>
              <Button
                class="shadow-destructive w-44 shadow-lg"
                onClick={() => setDialogOpen(false)}
              >
                <T key="close" />
              </Button>
            </Match>
          </Switch>
        </div>
      </AlertDialogContent>
    </AlertDialog>
  );
};
