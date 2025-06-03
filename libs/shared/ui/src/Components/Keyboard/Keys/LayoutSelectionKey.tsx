import { JSX, ParentComponent, createSignal, Show, For } from 'solid-js';
import { useKeyboardInternal } from '../KeyboardContext';
import { BaseKey } from './BaseKey';
import { Portal } from 'solid-js/web';
import { Icon } from '@micra-pro/shared/ui';

export const LayoutSelectionKey: ParentComponent<
  JSX.HTMLAttributes<HTMLButtonElement>
> = (props) => {
  const keyboardContext = useKeyboardInternal();
  const [position, setPosition] = createSignal<{
    left: number;
    top: number;
  } | null>(null);
  let button!: HTMLButtonElement;

  const clicked = () => {
    setPosition((oldPosition) => {
      if (oldPosition !== null) {
        return null;
      }
      const pos = button.getBoundingClientRect();
      return { left: pos.x, top: pos.y - 10 };
    });
  };

  return (
    <>
      <BaseKey {...props} ref={button} onClick={clicked}>
        {props.children}
      </BaseKey>
      <Show when={position() !== null}>
        <Portal>
          <div
            class="fixed left-0 top-0 z-40 h-screen w-screen"
            onClick={() => setPosition(null)}
          />
          <div
            class="'min-w-8rem fixed z-50 flex min-w-24 -translate-y-full flex-col gap-1 rounded-md border bg-popover p-1 text-popover-foreground"
            style={{
              top: position()?.top + 'px',
              left: position()?.left + 'px',
            }}
          >
            <For each={keyboardContext.availableLayouts()}>
              {(layout) => {
                return (
                  <button
                    class="w-full p-2 text-left hover:bg-accent hover:text-accent-foreground"
                    onClick={() => {
                      keyboardContext.setLayout(layout);
                      setPosition(null);
                    }}
                  >
                    <Icon
                      iconName="checkbox"
                      class="mr-2"
                      classList={{
                        'opacity-0': layout !== keyboardContext.currentLayout(),
                      }}
                    />
                    {layout}
                  </button>
                );
              }}
            </For>
          </div>
        </Portal>
      </Show>
    </>
  );
};

export default LayoutSelectionKey;
