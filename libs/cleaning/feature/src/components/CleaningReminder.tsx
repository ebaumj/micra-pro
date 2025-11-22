import {
  Component,
  createEffect,
  createSignal,
  onCleanup,
  onMount,
  Show,
} from 'solid-js';
import { useCleaningContext } from './CleaningContextProvider';
import { twMerge } from 'tailwind-merge';
import {
  Button,
  Dialog,
  DialogContent,
  DialogTitle,
  Icon,
} from '@micra-pro/shared/ui';
import moment from 'moment';
import { useNavigate } from '@solidjs/router';
import { T } from '../generated/language-types';

const cleaningCheckIntervalMs = 1000;

export const CleaningRemnder: Component<{ class?: string }> = (props) => {
  const navigate = useNavigate();
  let timer: NodeJS.Timeout | null;
  const ctx = useCleaningContext();
  const [isCleaningDue, setIsCleaningDue] = createSignal(false);
  const [ignore, setIgnore] = createSignal(false);
  const [dialog, setDialog] = createSignal(false);
  const checkCleaning = () => {
    if (ctx.reminder()) {
      if (
        ctx.interval().add(ctx.lastDate().valueOf(), 'milliseconds') <
        moment.duration(new Date().valueOf(), 'milliseconds')
      )
        setIsCleaningDue(true);
    }
    timer = setTimeout(checkCleaning, cleaningCheckIntervalMs);
  };
  createEffect(() => {
    if (!ctx.reminder()) setIsCleaningDue(false);
  });
  onMount(checkCleaning);
  onCleanup(() => timer && clearTimeout(timer));
  return (
    <>
      <Dialog open={dialog()} onOpenChange={setDialog}>
        <DialogContent class="max-w-80">
          <DialogTitle>
            <T key="cleaning-due" />
          </DialogTitle>
          <Button onClick={() => navigate('/menu/cleaning')}>
            <T key="cleaning-page" />
          </Button>
          <Button
            variant="outline"
            onClick={() => setIgnore(true) && setDialog(false)}
          >
            <T key="ignore" />
          </Button>
        </DialogContent>
      </Dialog>
      <Show when={isCleaningDue() && !ignore()}>
        <Button
          class={twMerge('text-lg', props.class)}
          variant="destructive"
          onClick={() => setDialog(true)}
        >
          <Icon class="animate-pulse" iconName="warning" />
        </Button>
      </Show>
    </>
  );
};
