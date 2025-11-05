import { Component, splitProps } from 'solid-js';
import { JSX } from 'solid-js/jsx-runtime';
import { useAssetAccessor } from './AssetContextProvider';
import picturesImport from '../generated/pictures-import';
import { selectPicturesForMode } from '@micra-pro/shared/ui';

type AssetProps = Omit<JSX.HTMLAttributes<HTMLImageElement>, 'src'> & {
  assetId?: string;
};

export const Asset: Component<AssetProps> = (props) => {
  const asseccor = useAssetAccessor();
  const rest = splitProps(props, ['assetId'])[1];
  const pictures = selectPicturesForMode(picturesImport);
  return (
    <img
      src={
        asseccor.assets().find((a) => a.id === props.assetId)?.path ??
        pictures()['image-load-failed']
      }
      {...rest}
    />
  );
};
