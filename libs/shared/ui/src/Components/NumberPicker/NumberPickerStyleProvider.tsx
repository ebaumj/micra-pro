import {
  Accessor,
  createContext,
  createSignal,
  ParentComponent,
  useContext,
} from 'solid-js';

export type NumberPickerStyle = 'NumberPad' | 'NumberWheel';

const NumberPickerStyleContext = createContext<{
  setStyle: (value: NumberPickerStyle) => void;
  style: Accessor<NumberPickerStyle>;
}>();

export const NumberPickerStyleProvider: ParentComponent = (props) => {
  const [style, setStyle] = createSignal<NumberPickerStyle>('NumberPad');
  return (
    <NumberPickerStyleContext.Provider value={{ style, setStyle }}>
      {props.children}
    </NumberPickerStyleContext.Provider>
  );
};

export const useNumberPickerStyle = () => {
  const ctx = useContext(NumberPickerStyleContext);
  if (!ctx) throw new Error("Can't find Number Picker Style Context!");
  return ctx;
};
