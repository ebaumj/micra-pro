import { Component, createEffect, createSignal, Show } from 'solid-js';
import {
  Button,
  Dialog,
  DialogContent,
  Icon,
  LongPressDiv,
  Spinner,
  handleError,
} from '@micra-pro/shared/ui';
import {
  createScalesAccessor,
  createScanAccessor,
} from '@micra-pro/scale-management/data-access';
import { twMerge } from 'tailwind-merge';
import { ScanScalesDialog } from './ScanScalesDialog';
import { useTranslationContext } from '../generated/language-types';

export const ScaleSelector: Component<{ class?: string }> = (props) => {
  const { t } = useTranslationContext();
  const scalesAccessor = createScalesAccessor();
  const scanAccessor = createScanAccessor();
  const [scanDialog, setScanDialog] = createSignal(false);
  const [scaleDeleting, setScaleDeleting] = createSignal(false);
  createEffect(
    () =>
      !scanDialog() && scanAccessor.isScanning() && scanAccessor.stopScanning(),
  );
  const addDevice = (identifier: string) =>
    scalesAccessor.add(
      identifier,
      () => setScanDialog(false),
      () => handleError({ title: t('error'), message: t('unknown-error') }),
    );

  return (
    <>
      <Dialog open={scanDialog()} onOpenChange={(o) => setScanDialog(o)}>
        <DialogContent
          onOpenAutoFocus={(e) => e.preventDefault()}
          onInteractOutside={(e) => e.preventDefault()}
        >
          <ScanScalesDialog
            isOpen={scanDialog()}
            close={() => setScanDialog(false)}
            addDevice={addDevice}
            isScanning={scanAccessor.isScanning()}
          />
        </DialogContent>
      </Dialog>
      <Show when={scalesAccessor.scale()}>
        {(scale) => (
          <LongPressDiv
            class={twMerge(props.class, 'flex items-center justify-center')}
            onLongPress={() => {
              scale().remove();
              setScaleDeleting(false);
            }}
            onPressStart={() => setScaleDeleting(true)}
            onPressEnd={() => setScaleDeleting(false)}
            delayTimeMs={1000}
            maxShortPressTimeMs={300}
          >
            <Button
              variant="default"
              class="h-full w-full"
              disabled={scale().isDeleting()}
            >
              <Show when={!scale().isDeleting() && !scaleDeleting()}>
                <Icon iconName="scale" class="mx-2" />
              </Show>
              <Show when={scale().isDeleting()}>
                <Spinner class="h-full" />
              </Show>
              <Show when={!scale().isDeleting() && scaleDeleting()}>
                <div class="flex items-center gap-1">
                  <Icon iconName="delete" class="text-destructive" />
                  <div class="animate-spin-loader-1s bg-destructive h-6 w-6 rotate-45 rounded-full" />
                </div>
              </Show>
            </Button>
          </LongPressDiv>
        )}
      </Show>
      <Show when={!scalesAccessor.scale()}>
        <div class={twMerge(props.class, 'flex items-center justify-center')}>
          <Button
            variant="outline"
            class="h-full w-full"
            onClick={() => setScanDialog(true)}
          >
            <Icon iconName="bluetooth" class="mx-2" />
          </Button>
        </div>
      </Show>
    </>
  );
};
