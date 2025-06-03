import { createSignal, JSX } from 'solid-js';
import {
  SelectPrimitive,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from './SelectPrimitive';

export type SelectProps<T> = {
  options: T[];
  value: T | undefined;
  displayElement: (value: T | undefined) => JSX.Element;
  class?: string;
  onChange: (value: T) => Promise<void> | void;
};

export const Select = <T,>(props: SelectProps<T>) => {
  const [next, setNext] = createSignal<T | undefined>();

  const onChange = (value: T | null) => {
    if (!value || next()) return;

    const returnValue = props.onChange(value);
    if (!returnValue) return;
    setNext(() => value);
    returnValue.finally(() => setNext(undefined));
  };

  return (
    <SelectPrimitive
      options={props.options}
      value={props.value}
      onChange={onChange}
      itemComponent={(itemProps) => (
        <SelectItem
          item={itemProps.item}
          loading={next() !== undefined}
          isSelected={props.value === itemProps.item.rawValue}
        >
          {props.displayElement(itemProps.item.rawValue)}
        </SelectItem>
      )}
      class={props.class}
    >
      <SelectTrigger class="">
        <SelectValue<string>>{props.displayElement(props.value)}</SelectValue>
      </SelectTrigger>
      <SelectContent />
    </SelectPrimitive>
  );
};

export default Select;
