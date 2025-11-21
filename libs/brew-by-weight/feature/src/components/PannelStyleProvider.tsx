import { createConfigAccessor } from '@micra-pro/shared/utils-ts';
import { Accessor, createContext, ParentComponent, useContext } from 'solid-js';

type PannelStyle = 'Graph' | 'Gauge';

const Context = createContext<{
  pannelStyle: Accessor<PannelStyle>;
  setPannelStyle: (value: PannelStyle) => void;
}>();

export const usePannelStyle = () => {
  const ctx = useContext(Context);
  if (!ctx)
    throw new Error('Could not find Brew By Weight Pannel Style Context!');
  return ctx;
};

export const BrewByWeightPannelStyleProvider: ParentComponent = (props) => {
  const config = createConfigAccessor<{ style: PannelStyle }>(
    'BrewByWeightPannel',
  );
  const pannelStyle = (): PannelStyle => config.config()?.style ?? 'Graph';
  const setPannelStyle = (value: PannelStyle) =>
    config.writeConfig({ style: value });
  return (
    <Context.Provider value={{ pannelStyle, setPannelStyle }}>
      {props.children}
    </Context.Provider>
  );
};
