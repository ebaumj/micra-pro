import { createContext, ParentComponent, useContext } from 'solid-js';

type ExtractionTimeThresholdContextType = {
  good: number;
  bad: number;
};

const ExtractionTimeThresholdContext =
  createContext<ExtractionTimeThresholdContextType>();

export const useExtractionTimeThreshold = () => {
  const context = useContext(ExtractionTimeThresholdContext);
  if (!context)
    throw new Error(
      'ExtractionTimeThresholdContext not found. Please wrap your component in a ExtractionTimeThresholdContextProvider',
    );
  return context;
};

export const ExtractionTimeThresholdContextProvider: ParentComponent<{
  threshold: ExtractionTimeThresholdContextType;
}> = (props) => {
  return (
    <ExtractionTimeThresholdContext.Provider
      // eslint-disable-next-line solid/reactivity
      value={props.threshold}
      children={props.children}
    />
  );
};
