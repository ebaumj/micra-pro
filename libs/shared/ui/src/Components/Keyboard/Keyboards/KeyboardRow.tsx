import { ParentComponent } from 'solid-js';

export const KeyboardRow: ParentComponent = (props) => {
  return <div class="flex h-12 gap-1">{props.children}</div>;
};

export default KeyboardRow;
