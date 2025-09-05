import { Component, Show } from 'solid-js';
import { useScaleSelectorContext } from './ScaleSelectorContextProvider';
import { Button, Icon, Select } from '@micra-pro/shared/ui';
import { createScalesAccessor } from '@micra-pro/scale-management/data-access';
import { twMerge } from 'tailwind-merge';
import { useNavigate } from '@solidjs/router';
import { T } from '../generated/language-types';

export const ScaleSelector: Component<{ class?: string }> = (props) => {
  var ctx = useScaleSelectorContext();
  var scalesAccessor = createScalesAccessor();
  var navigate = useNavigate();

  const setupScales = () => {
    navigate('/menu/scales', { replace: true });
  };

  return (
    <>
      <Show when={scalesAccessor.scales().length > 0}>
        <Select
          options={scalesAccessor.scales().map((s) => s.id)}
          displayElement={(id) => (
            <div class="flex items-center px-1">
              <Icon iconName="scale" class="mr-2" />
              {scalesAccessor.scales().find((s) => s.id === id)?.name}
            </div>
          )}
          onChange={(val) => ctx.setSelectedScale(val)}
          value={ctx.selectedScale()}
          class={props.class}
        />
      </Show>
      <Show when={scalesAccessor.scales().length === 0}>
        <div
          class={twMerge(props.class, 'flex items-center justify-center p-2')}
        >
          <Button variant="outline" class="w-full" onClick={setupScales}>
            <T key="scales-menu" />
          </Button>
        </div>
      </Show>
    </>
  );
};
