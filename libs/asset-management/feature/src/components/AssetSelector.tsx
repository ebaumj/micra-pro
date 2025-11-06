import {
  Component,
  createEffect,
  createSignal,
  JSX,
  Show,
  splitProps,
} from 'solid-js';
import { Asset } from './Asset';
import picturesImport from '../generated/pictures-import';
import { AssetCreator } from './AssetCreator';
import { useAssetAccessor } from './AssetContextProvider';
import { twMerge } from 'tailwind-merge';
import {
  LongPressDiv,
  selectPicturesForMode,
  Spinner,
} from '@micra-pro/shared/ui';

type AssetSelectorProps = Omit<JSX.HTMLAttributes<HTMLImageElement>, 'src'> & {
  assetId?: string;
  onIdChange: (assetId: string) => void;
  onRemove?: () => void;
};

export const AssetSelector: Component<AssetSelectorProps> = (props) => {
  const accessor = useAssetAccessor();
  const pictures = selectPicturesForMode(picturesImport);
  const [local, rest] = splitProps(props, ['assetId', 'onIdChange', 'class']);
  const [createOpen, setCreateOpen] = createSignal(false);
  const [unfinished, setUnfinished] = createSignal<string | undefined>();
  const [isPolling, setIsPolling] = createSignal(false);
  createEffect(() => {
    const path = accessor
      .unfinished()
      .find((a) => a.id === props.assetId)?.uploadPath;
    if (path) setUnfinished(path);
  });
  const changeAsset = () => {
    setUnfinished(undefined);
    setCreateOpen(true);
  };
  createEffect(() => {
    if (props.assetId) {
      const isPollingAccessor = accessor.isPolling(props.assetId);
      createEffect(() => setIsPolling(isPollingAccessor()));
    } else setIsPolling(false);
  });
  return (
    <>
      <AssetCreator
        isOpen={createOpen()}
        onClose={() => setCreateOpen(false)}
        onCreate={props.onIdChange}
        uploadPath={unfinished()}
      />
      <Show when={props.assetId}>
        <LongPressDiv
          onClick={changeAsset}
          onLongPress={() => props.onRemove?.()}
        >
          <Show when={!isPolling()}>
            <Asset assetId={props.assetId!} {...rest} class={local.class} />
          </Show>
          <Show when={isPolling()}>
            <div class={local.class}>
              <Spinner class="h-full w-full p-4" />
            </div>
          </Show>
        </LongPressDiv>
      </Show>
      <Show when={!props.assetId}>
        <img
          src={pictures()['image-add']}
          {...rest}
          class={twMerge('opacity-50', local.class)}
          onClick={() => setCreateOpen(true)}
        />
      </Show>
    </>
  );
};
