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

type AssetSelectorProps = Omit<JSX.HTMLAttributes<HTMLImageElement>, 'src'> & {
  assetId?: string;
  onIdChange: (assetId: string) => void;
};

export const AssetSelector: Component<AssetSelectorProps> = (props) => {
  const accessor = useAssetAccessor();
  const rest = splitProps(props, ['assetId', 'onIdChange'])[1];
  const [createOpen, setCreateOpen] = createSignal(false);
  const [unfinished, setUnfinished] = createSignal<string | undefined>();
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
  return (
    <>
      <AssetCreator
        isOpen={createOpen()}
        onClose={() => setCreateOpen(false)}
        onCreate={props.onIdChange}
        uploadPath={unfinished()}
      />
      <Show when={props.assetId}>
        <Asset assetId={props.assetId!} {...rest} onClick={changeAsset} />
      </Show>
      <Show when={!props.assetId}>
        <img
          src={picturesImport['image-add']}
          {...rest}
          onClick={() => setCreateOpen(true)}
        />
      </Show>
    </>
  );
};
