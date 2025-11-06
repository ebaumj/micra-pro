import { ParentComponent, JSX } from 'solid-js';

export const KeyboardContainer: ParentComponent<
  JSX.HTMLAttributes<HTMLDivElement>
> = (props) => {
  return (
    <div
      {...props}
      classList={{
        ...props.classList,
        'pointer-events-auto flex flex-col gap-1 bg-background p-3 z-60 border-t shadow-xl': true,
      }}
      onMouseDown={(e) => e.preventDefault()}
    >
      {props.children}
    </div>
  );
};
