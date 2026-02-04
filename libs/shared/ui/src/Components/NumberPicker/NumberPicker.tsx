import {
  Accessor,
  Component,
  createContext,
  createSignal,
  Match,
  ParentComponent,
  splitProps,
  Switch,
  useContext,
} from 'solid-js';
import { Dialog, DialogContent } from '../Dialog';
import { NumberPad } from './NumberPad';
import { NumberWheel } from './NumberWheel';
import { useNumberPickerStyle } from './NumberPickerStyleProvider';

const NumberPickerContext = createContext<{
  setOpen: (ipen: boolean) => void;
  open: Accessor<boolean>;
}>();

export const NumberPickerContextProvider: ParentComponent = (props) => {
  const [open, setOpen] = createSignal(false);
  return (
    <NumberPickerContext.Provider value={{ setOpen, open }}>
      {props.children}
    </NumberPickerContext.Provider>
  );
};

export const useNumberPickerContext = () => {
  const ctx = useContext(NumberPickerContext);
  if (!ctx) throw new Error("Can't find Number Picker Context!");
  return { setOpen: ctx.setOpen };
};

export const NumberPicker: Component<{
  value?: number;
  onSetValue: (value: number) => void;
  min?: number;
  max?: number;
  step?: number;
}> = (props) => {
  const ctx = useContext(NumberPickerContext);
  if (!ctx) throw new Error("Can't find Number Picker Context!");
  const style = useNumberPickerStyle().style;
  const [local, rest] = splitProps(props, ['value', 'onSetValue']);
  const setValue = (value: number) => {
    local.onSetValue(value);
    ctx.setOpen(false);
  };
  return (
    <Dialog open={ctx.open()} onOpenChange={ctx.setOpen}>
      <DialogContent
        class="bg-background max-w-88"
        onOpenAutoFocus={(e) => e.preventDefault()}
      >
        <Switch>
          <Match when={style() === 'NumberPad'}>
            <NumberPad
              onSetValue={setValue}
              value={local.value}
              close={() => ctx.setOpen(false)}
            />
          </Match>
          <Match when={style() === 'NumberWheel'}>
            <NumberWheel
              onSetValue={setValue}
              value={local.value}
              close={() => ctx.setOpen(false)}
              {...rest}
            />
          </Match>
        </Switch>
      </DialogContent>
    </Dialog>
  );
};
