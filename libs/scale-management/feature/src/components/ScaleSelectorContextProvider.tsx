import { createConfigAccessor } from '@micra-pro/shared/utils-ts';
import { Accessor, createContext, ParentComponent, useContext } from 'solid-js';

type ScaleSelectorContextType = {
  selectedScale: Accessor<string | undefined>;
  setSelectedScale: (value: string) => void;
};

const ScaleSelectorContext = createContext<ScaleSelectorContextType>();

export const useScaleSelectorContext = () => {
  const context = useContext(ScaleSelectorContext);
  if (!context)
    throw new Error(
      'ScaleSelectorContext not found. Please wrap your component in a ScaleSelectorContextProvider',
    );
  return context;
};

export const useSelectedScaleContext = () => ({
  selectedScale: useScaleSelectorContext().selectedScale,
});

export const ScaleSelectorContextProvider: ParentComponent = (props) => {
  const scaleConfig = createConfigAccessor<{ id: string }>('SelectedScale');
  return (
    <ScaleSelectorContext.Provider
      value={{
        selectedScale: () => scaleConfig.config()?.id,
        setSelectedScale: (value) => scaleConfig.writeConfig({ id: value }),
      }}
      children={props.children}
    />
  );
};
