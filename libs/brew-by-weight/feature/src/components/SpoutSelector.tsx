import { Component } from 'solid-js';
import { useSpoutSelectorContext } from './SpoutSelectorContextProvider';
import { Select, selectPicturesForMode } from '@micra-pro/shared/ui';
import { Spout } from '@micra-pro/brew-by-weight/data-access';
import picturesImport from '../generated/pictures-import';

export const SpoutSelector: Component<{ class?: string }> = (props) => {
  var ctx = useSpoutSelectorContext();
  const pictures = selectPicturesForMode(picturesImport);

  const spoutToImage = (spout: Spout) => {
    switch (spout) {
      case Spout.Double:
        return pictures().spout.double;
      case Spout.Naked:
        return pictures().spout.naked;
      case Spout.Single:
        return pictures().spout.single;
    }
  };

  return (
    <Select
      options={Object.values(Spout)}
      displayElement={(val) => (
        <img class="max-h-7 object-contain" src={spoutToImage(val as Spout)} />
      )}
      onChange={(val) => ctx.setSelectedSpout(val as Spout)}
      value={ctx.selectedSpout()}
      class={props.class}
    />
  );
};
