import { JSX, ParentComponent, splitProps } from 'solid-js';
import { cn } from '../../../utils/cn';

export const BaseKey: ParentComponent<
  JSX.ButtonHTMLAttributes<HTMLButtonElement>
> = (props) => {
  const [local, rest] = splitProps(props, ['class']);

  return (
    <button
      class={cn(
        'grow basis-4 rounded-md border border-input active:bg-gray-200',
        local.class,
      )}
      {...rest}
    />
  );
};

export default BaseKey;
