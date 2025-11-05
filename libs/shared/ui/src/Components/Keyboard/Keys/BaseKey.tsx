import { JSX, ParentComponent, splitProps } from 'solid-js';
import { cn } from '../../../utils/cn';

export const BaseKey: ParentComponent<
  JSX.ButtonHTMLAttributes<HTMLButtonElement>
> = (props) => {
  const [local, rest] = splitProps(props, ['class']);

  return (
    <button
      class={cn(
        'border-border active:bg-secondary grow basis-4 rounded-md border',
        local.class,
      )}
      {...rest}
    />
  );
};

export default BaseKey;
