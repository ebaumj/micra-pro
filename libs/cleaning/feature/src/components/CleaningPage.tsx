import { Component, Match, Switch } from 'solid-js';
import { useCleaningContext } from './CleaningContextProvider';
import { cleaningParametersAccessor } from '@micra-pro/cleaning/data-access';
import {
  Button,
  NumberField,
  NumberFieldDecrementTrigger,
  NumberFieldGroup,
  NumberFieldIncrementTrigger,
  NumberFieldInput,
  Spinner,
  SwitchControl,
  SwitchThumb,
  Switch as SwitchUi,
} from '@micra-pro/shared/ui';
import { dateString } from '@micra-pro/shared/utils-ts';
import moment from 'moment';
import { CleaningDialog } from './CleaningDialog';
import { T } from '../generated/language-types';

export const CleaningPage: Component = () => {
  const ctx = useCleaningContext();
  const params = cleaningParametersAccessor();
  return (
    <Switch>
      <Match when={params.loading()}>
        <div class="flex h-full w-full items-center justify-center">
          <Spinner class="h-16 w-16" />
        </div>
      </Match>
      <Match when={!params.loading()}>
        <div class="flex h-full w-full items-center justify-center">
          <CleaningDialog cleaningParams={params.sequence()} />
          <div class="flex h-full w-1/2 flex-col items-center justify-center gap-4 border-r pr-4">
            <div class="flex flex-col items-center rounded-sm border px-3 py-1 text-sm inset-shadow-sm">
              <div>
                <T key="last-cleaning-date" />
              </div>
              <div class="font-bold">{dateString(ctx.lastDate())}</div>
            </div>
            <Button onClick={() => ctx.start()}>
              <T key="start-cleaning" />
            </Button>
          </div>
          <div class="flex h-full w-1/2 flex-col justify-center gap-4 pl-4">
            <div class="flex items-center justify-center gap-2">
              <div>
                <T key="reminder" />
              </div>
              <SwitchUi checked={ctx.reminder()} onChange={ctx.setReminder}>
                <SwitchControl>
                  <SwitchThumb />
                </SwitchControl>
              </SwitchUi>
            </div>
            <div class="flex w-full flex-col items-center gap-1 px-4">
              <div>
                <T key="interval" />
              </div>
              <NumberField
                onFocusIn={(e) => e.preventDefault()}
                onRawValueChange={(v) =>
                  v > 0 && ctx.setInterval(moment.duration(v, 'days'))
                }
                rawValue={ctx.interval().asDays()}
                formatOptions={{ style: 'unit', unit: 'day' }}
                minValue={1}
                maxValue={999}
                step={1}
              >
                <NumberFieldGroup class="flex items-center justify-center">
                  <NumberFieldDecrementTrigger aria-label="Decrement" />
                  <NumberFieldInput />
                  <NumberFieldIncrementTrigger aria-label="Increment" />
                </NumberFieldGroup>
              </NumberField>
            </div>
          </div>
        </div>
      </Match>
    </Switch>
  );
};
