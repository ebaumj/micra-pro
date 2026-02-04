import {
  Accessor,
  createContext,
  createEffect,
  ParentComponent,
  useContext,
} from 'solid-js';
import {
  NumberPickerStyle,
  NumberPickerStyleProvider as UiProvider,
  useNumberPickerStyle as uiUseStyle,
} from '@micra-pro/shared/ui';
import { createConfigAccessor } from '@micra-pro/shared/utils-ts';

const NumberPickerStyleContext = createContext<{
  setStyle: (value: NumberPickerStyle) => Promise<any>;
  style: Accessor<NumberPickerStyle | undefined>;
}>();

export const NumberPickerStyleProviderInternal: ParentComponent = (props) => {
  const uiCtx = uiUseStyle();
  const config = createConfigAccessor<{ style: NumberPickerStyle }>(
    'NumberPickerStyle',
  );
  createEffect(() => {
    const style = config.config()?.style;
    if (style) uiCtx.setStyle(style);
  });
  return (
    <NumberPickerStyleContext.Provider
      value={{
        setStyle: (value: NumberPickerStyle) =>
          config.writeConfig({ style: value }),
        style: () => config.config()?.style,
      }}
    >
      {props.children}
    </NumberPickerStyleContext.Provider>
  );
};

export const NumberPickerStyleProvider: ParentComponent = (props) => {
  return (
    <UiProvider>
      <NumberPickerStyleProviderInternal>
        {props.children}
      </NumberPickerStyleProviderInternal>
    </UiProvider>
  );
};

export const useNumberPickerStyle = () => {
  const ctx = useContext(NumberPickerStyleContext);
  if (!ctx) throw new Error("Can't find Number Picker Style Context!");
  return ctx;
};
