import { Component, createEffect, createSignal, Show } from 'solid-js';
import { Dialog, DialogContent, Spinner } from '@micra-pro/shared/ui';
import QRCode from 'qrcode';
import { useAssetAccessor } from './AssetContextProvider';

export const AssetCreator: Component<{
  onClose: () => void;
  onCreate: (assetId: string) => void;
  isOpen: boolean;
  uploadPath?: string;
}> = (props) => {
  const accessor = useAssetAccessor();
  const [qr, setQr] = createSignal<string | undefined>();
  createEffect(() => {
    if (props.isOpen) {
      if (props.uploadPath) QRCode.toDataURL(props.uploadPath).then(setQr);
      else
        accessor
          .create()
          .then((a) => {
            QRCode.toDataURL(a.uploadPath).then(setQr);
            onCreate(a.id);
          })
          .catch(() => {
            close();
          });
    } else {
      setQr(undefined);
    }
  });
  const onCreate = (id: string) => {
    accessor.pollAsset(id);
    props.onCreate(id);
  };
  createEffect(() => {
    const path = props.uploadPath;
    const id = accessor.unfinished().find((a) => a.uploadPath === path)?.id;
    if (path && id) accessor.pollAsset(id);
  });
  const close = () => {
    setQr(undefined);
    props.onClose();
  };
  return (
    <Dialog
      open={props.isOpen}
      onOpenChange={(o) => (o ? undefined : props.onClose())}
    >
      <DialogContent
        onOpenAutoFocus={(e) => e.preventDefault()}
        onInteractOutside={(e) => e.preventDefault()}
        class="h-80 pt-0"
      >
        <Show when={!qr()}>
          <div class="flex h-full w-full items-center justify-center">
            <Spinner class="h-20 w-20" />
          </div>
        </Show>
        <Show when={qr()}>
          <div class="flex h-full w-full flex-col items-center justify-center">
            <img src={qr()} class="h-64 w-64" />
          </div>
        </Show>
      </DialogContent>
    </Dialog>
  );
};
