import {
  Component,
  createEffect,
  createSignal,
  For,
  Show,
  splitProps,
} from 'solid-js';
import { twMerge } from 'tailwind-merge';
import { Button } from '../Button';

const SingleNumber: Component<{
  value: number;
  onChanged: (value: number) => void;
  options: number[];
  lookup?: string[];
  class?: string;
}> = (props) => {
  const [pos, setPos] = createSignal(0);
  const [isDragging, setIsDragging] = createSignal(false);
  let startingPosition = 0;
  let startingValue = 0;
  let dragTarget!: HTMLDivElement;
  createEffect(() => {
    const value = props.value;
    const options = props.options;
    const closest = options.reduce((prev, curr) =>
      Math.abs(curr - value) < Math.abs(prev - value) ? curr : prev,
    );
    setPos(options.indexOf(closest));
  });
  const value = () => Math.round(pos());
  const onDragStart = (e: PointerEvent) => {
    startingPosition = e.clientY;
    startingValue = pos();
    dragTarget.setPointerCapture(e.pointerId);
    setIsDragging(true);
  };
  const drag = (e: PointerEvent) => {
    if (!isDragging()) return;
    const position =
      (2 * (startingPosition - e.clientY) * (props.options.length + 2)) /
        dragTarget.clientHeight +
      startingValue;
    if (position >= 0 && position <= props.options.length - 1) setPos(position);
  };
  const onDragEnd = (e: PointerEvent) => {
    setIsDragging(false);
    dragTarget.releasePointerCapture(e.pointerId);
    const newPos = value();
    setPos(newPos);
    props.onChanged(props.options[newPos]);
  };
  return (
    <div class={twMerge('', props.class)}>
      <div
        class="h-full w-full overflow-hidden"
        style={{
          'mask-image': `linear-gradient(to bottom, hsl(var(--background) / 0), hsl(var(--background) / 1) 10%, hsl(var(--background) / 1) 90%, hsl(var(--background) / 0) 100%)`,
        }}
      >
        <div class="relative z-0 flex h-full w-full items-center">
          <div class="bg-secondary h-1/2 w-full rounded-md border inset-shadow-sm" />
        </div>
        <div
          style={{
            transform: `translateY(${-(pos() * 50 + 125) / (props.options.length + 2)}%)`,
            height: `${(props.options.length + 2) * 100}%`,
          }}
          class={twMerge(
            'z-10 select-none',
            isDragging() ? '' : 'transition-transform duration-300 ease-in-out',
          )}
          ref={dragTarget}
          onPointerDown={(e) => onDragStart(e)}
          onPointerUp={onDragEnd}
          onPointerCancel={onDragEnd}
          onPointerMove={drag}
        >
          <div
            class="w-full"
            style={{ height: `${50 / (props.options.length + 2)}%` }}
          />
          <For each={props.options}>
            {(n, i) => (
              <div
                class={twMerge(
                  'flex w-full items-center justify-center transition-opacity duration-300',
                  i() === value() ? '' : 'opacity-25',
                )}
                style={{ height: `${50 / (props.options.length + 2)}%` }}
              >
                {props.lookup ? props.lookup[i()] : n}
              </div>
            )}
          </For>
          <div
            class="w-full"
            style={{ height: `${50 / (props.options.length + 2)}%` }}
          />
        </div>
      </div>
    </div>
  );
};

const NumberWheelComplete: Component<{
  value: number;
  onChanged: (value: number) => void;
  min?: number;
  max?: number;
  step?: number;
  class?: string;
}> = (props) => {
  const numbers = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
  const config = () => {
    const places = Math.max(props.max ?? 0, Math.abs(props.min ?? 0)).toFixed(
      0,
    ).length;
    const decimals = `${props.step ?? 0}`.split('.')[1]?.length ?? 0;
    const preDecimal = Array.from({ length: decimals }, (_, i) => ({
      valueSelector: (v: number) =>
        Number(v.toFixed(decimals).split('.')[1][i]),
      valuevalueModifier: (v: number, curr: number) => {
        const valueParts = curr.toFixed(decimals).split('.');
        return Number(
          `${valueParts[0]}.${valueParts[1]
            .split('')
            .map((c, idx) => (idx === i ? v : c))
            .join('')}`,
        );
      },
    }));
    const fixedLength = (v: number) => {
      const value = `${Math.floor(Math.abs(v))}`;
      return `${'0'.repeat(places - value.length)}${value}`;
    };
    const postDecimal = Array.from({ length: places }, (_, i) => ({
      valueSelector: (v: number) => Number(fixedLength(v)[i]),
      valuevalueModifier: (v: number, curr: number) => {
        const valueParts = `${curr}`.split('.');
        return Number(
          `${Math.sign(curr) < 0 ? '-' : ''}${fixedLength(Number(valueParts[0]))
            .split('')
            .map((c, idx) => (idx === i ? v : c))
            .join('')}.${valueParts[1] ?? '0'}`,
        );
      },
    }));
    return {
      preDecimal,
      postDecimal,
      total: preDecimal.length,
    };
  };
  const sign = (s: number, curr: number): number => {
    const str = `${curr}`;
    return Number(
      s >= 0 ? str.replaceAll('-', '') : str.includes('-') ? str : `-${str}`,
    );
  };
  return (
    <div class={props.class}>
      <div class="flex h-full w-fit gap-2 rounded-md border p-2 shadow-sm">
        <Show when={(props.min ?? 1) < 0}>
          <SingleNumber
            onChanged={(v) => props.onChanged(sign(v, props.value))}
            options={[-1, 1]}
            value={props.value < 0 ? -1 : 1}
            lookup={['-', '']}
            class="h-full w-12"
          />
        </Show>
        <For each={config().postDecimal}>
          {(n) => (
            <SingleNumber
              onChanged={(v) =>
                props.onChanged(n.valuevalueModifier(v, props.value))
              }
              options={numbers}
              value={n.valueSelector(props.value)}
              class="h-full w-12"
            />
          )}
        </For>
        <Show when={config().preDecimal.length > 0}>
          <div class="flex h-full items-center justify-center">.</div>
        </Show>
        <For each={config().preDecimal}>
          {(n) => (
            <SingleNumber
              onChanged={(v) =>
                props.onChanged(n.valuevalueModifier(v, props.value))
              }
              options={numbers}
              value={n.valueSelector(props.value)}
              class="h-full w-12"
            />
          )}
        </For>
      </div>
    </div>
  );
};

export const NumberWheel: Component<{
  value?: number;
  onSetValue: (value: number) => void;
  min?: number;
  max?: number;
  step?: number;
  close: () => void;
}> = (props) => {
  const [local, rest] = splitProps(props, ['value', 'onSetValue', 'close']);
  const [value, setValue] = createSignal(0);
  createEffect(() => local.value && setValue(local.value));
  return (
    <div class="flex h-fit flex-col items-center px-6 pt-8">
      <NumberWheelComplete
        {...rest}
        value={value()}
        onChanged={setValue}
        class="h-32"
      />
      <Button class="mt-8 h-10 w-1/2" onClick={() => local.onSetValue(value())}>
        OK
      </Button>
    </div>
  );
};
