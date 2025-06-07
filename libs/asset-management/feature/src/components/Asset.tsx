import { Component, splitProps } from 'solid-js';
import { JSX } from 'solid-js/jsx-runtime';
import { useAssetAccessor } from './AssetContextProvider';
import picturesImport from '../generated/pictures-import';

type AssetProps = Omit<JSX.HTMLAttributes<HTMLImageElement>, 'src'> & {
  assetId?: string;
};

export const Asset: Component<AssetProps> = (props) => {
  const asseccor = useAssetAccessor();
  const rest = splitProps(props, ['assetId'])[1];
  return (
    <img
      src={
        asseccor.assets().find((a) => a.id === props.assetId)?.path ??
        picturesImport['image-load-failed']
      }
      {...rest}
    />
  );
};
