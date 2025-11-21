import { Icon } from '@micra-pro/shared/ui';
import { twMerge } from 'tailwind-merge';
import { usePannelStyle } from './PannelStyleProvider';

export const BrewByWeightPannelStyleSelector = () => {
  const ctx = usePannelStyle();
  return (
    <div class="flex w-full justify-center">
      <div class="flex h-14 w-56">
        <div
          class="z-10 flex w-1/2 items-center justify-center"
          onClick={() => ctx.setPannelStyle('Graph')}
        >
          <div class="bg-primary text-primary-foreground shadow-primary-shadow flex h-10 w-10 items-center justify-center rounded-full shadow-sm">
            <Icon iconName="ssid_chart" />
          </div>
        </div>
        <div
          class="z-10 flex w-1/2 items-center justify-center"
          onClick={() => ctx.setPannelStyle('Gauge')}
        >
          <div class="bg-primary text-primary-foreground shadow-primary-shadow flex h-10 w-10 items-center justify-center rounded-full shadow-sm">
            <Icon iconName="speed" />
          </div>
        </div>
      </div>
      <div class="fixed flex h-14 w-56 rounded-md border inset-shadow-sm">
        <div
          class={twMerge(
            'bg-secondary w-1/2 rounded inset-shadow-sm transition-transform duration-300',
            ctx.pannelStyle() !== 'Graph'
              ? 'translate-x-full'
              : 'translate-x-0',
          )}
        />
      </div>
    </div>
  );
};
