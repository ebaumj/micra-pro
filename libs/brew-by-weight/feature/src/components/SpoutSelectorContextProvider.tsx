import { Spout } from '@micra-pro/brew-by-weight/data-access';
import { createConfigAccessor } from '@micra-pro/shared/utils-ts';
import { Accessor, createContext, ParentComponent, useContext } from 'solid-js';

type SpoutSelectorContextType = {
  selectedSpout: Accessor<Spout>;
  setSelectedSpout: (value: Spout) => void;
};

const SpoutSelectorContext = createContext<SpoutSelectorContextType>();

export const useSpoutSelectorContext = () => {
  const context = useContext(SpoutSelectorContext);
  if (!context)
    throw new Error(
      'SpoutSelectorContext not found. Please wrap your component in a SpoutSelectorContextProvider',
    );
  return context;
};

export const useSelectedSpoutContext = () => ({
  selectedSpout: useSpoutSelectorContext().selectedSpout,
});

export const SpoutSelectorContextProvider: ParentComponent = (props) => {
  const spoutConfig = createConfigAccessor<{ spout: Spout }>('SelectedSpout');
  return (
    <SpoutSelectorContext.Provider
      value={{
        selectedSpout: () => spoutConfig.config()?.spout ?? Spout.Single,
        setSelectedSpout: (value) => spoutConfig.writeConfig({ spout: value }),
      }}
      children={props.children}
    />
  );
};
