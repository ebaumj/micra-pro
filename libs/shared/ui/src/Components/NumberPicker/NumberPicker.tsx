import {
  Accessor,
  Component,
  createContext,
  createSignal,
  onMount,
  ParentComponent,
  useContext,
} from 'solid-js';
import { Dialog, DialogContent } from '../Dialog';
import { Button } from '../Button';
import { NumberKey } from './NumberKey';
import { BackspaceKey } from './BackspaceKey';

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

const NumberPickerContent: Component<{
  value?: number;
  onSetValue: (value: number) => void;
  close: () => void;
}> = (props) => {
  let element!: HTMLInputElement;
  onMount(() => (element.value = `${props.value ?? 0}`));
  const getValue = (): number => {
    if (element.value === '') return 0;
    return Number(element.value);
  };
  const confirm = () => {
    const value = getValue();
    if (Number.isNaN(value)) props.close();
    else props.onSetValue(value);
  };
  return (
    <div class="px-6 pt-2">
      <input
        ref={element}
        class="h-12 w-full rounded-lg border bg-white p-2 text-right text-xl"
        disabled
      />
      <div class="flex h-16 w-full gap-2 pt-4">
        <NumberKey value="1" refInput={element} />
        <NumberKey value="2" refInput={element} />
        <NumberKey value="3" refInput={element} />
      </div>
      <div class="flex h-16 w-full gap-2 pt-2">
        <NumberKey value="4" refInput={element} />
        <NumberKey value="5" refInput={element} />
        <NumberKey value="6" refInput={element} />
      </div>
      <div class="flex h-16 w-full gap-2 pt-2">
        <NumberKey value="7" refInput={element} />
        <NumberKey value="8" refInput={element} />
        <NumberKey value="9" refInput={element} />
      </div>
      <div class="flex h-16 w-full gap-2 pt-2">
        <NumberKey value="." refInput={element} />
        <NumberKey value="0" refInput={element} />
        <BackspaceKey refInput={element} />
      </div>
      <div class="flex h-14 w-full justify-center pt-4">
        <Button class="h-full w-1/2" onClick={confirm}>
          OK
        </Button>
      </div>
    </div>
  );
};

export const NumberPicker: Component<{
  value?: number;
  onSetValue: (value: number) => void;
}> = (props) => {
  const ctx = useContext(NumberPickerContext);
  if (!ctx) throw new Error("Can't find Number Picker Context!");
  const setValue = (value: number) => {
    props.onSetValue(value);
    ctx.setOpen(false);
  };
  return (
    <Dialog open={ctx.open()} onOpenChange={ctx.setOpen}>
      <DialogContent
        class="max-w-72 bg-slate-50"
        onOpenAutoFocus={(e) => e.preventDefault()}
      >
        <NumberPickerContent
          onSetValue={setValue}
          value={props.value}
          close={() => ctx.setOpen(false)}
        />
      </DialogContent>
    </Dialog>
  );
};
