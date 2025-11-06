import {
  Collapsible,
  CollapsibleContent,
  handleError,
  NumberField,
  NumberFieldDecrementTrigger,
  NumberFieldGroup,
  NumberFieldIncrementTrigger,
  NumberFieldInput,
  selectPicturesForMode,
  SpinnerButton,
  Switch,
  SwitchControl,
  SwitchThumb,
} from '@micra-pro/shared/ui';
import { Component, createSignal, Show } from 'solid-js';
import { T, useTranslationContext } from '../generated/language-types';
import { useMachineContextInternal } from './MachineContextProvider';
import { twMerge } from 'tailwind-merge';
import picturesImport from '../generated/pictures-import';
import moment from 'moment';
import { SmartStandbyMode } from '@micra-pro/machine/data-access';

const fromMinutes = (value: number) =>
  moment.duration(value, 'minutes').toISOString();
const toMinutes = (duration: any) => moment.duration(duration).asMinutes();

const EnableSmartStandby = (current: SmartStandbyMode) => {
  switch (current) {
    case SmartStandbyMode.LastBrew:
    case SmartStandbyMode.Off:
      return SmartStandbyMode.LastBrew;
    case SmartStandbyMode.PowerOn:
      return SmartStandbyMode.PowerOn;
  }
};

export const MachineAccessPage: Component = () => {
  const machineAccessor = useMachineContextInternal();
  const pictures = selectPicturesForMode(picturesImport);
  const { t } = useTranslationContext();
  const [disconnecting, setDisconnecting] = createSignal(false);
  const disconnect = () => {
    setDisconnecting(true);
    machineAccessor
      .disconnect()
      .catch(() =>
        handleError({
          title: t('error'),
          message: t('disconnect-failed'),
        }),
      )
      .finally(() => setDisconnecting(false));
  };
  const setCoffeeTemperature = (value: number) => {
    const current = machineAccessor.boilers()?.coffeeBoiler.targetTemperature;
    if (value >= 60 && value <= 99 && current && current !== value)
      machineAccessor.setCoffeeTemperature(value);
  };
  const setSmartStandbyEnabled = (enabled: boolean) => {
    const current = machineAccessor.smartStandby();
    if (current)
      machineAccessor.setSmartStandby({
        ...current,
        mode: enabled ? EnableSmartStandby(current.mode) : SmartStandbyMode.Off,
      });
  };
  const setSmartStandbyMode = (mode: SmartStandbyMode) => {
    const current = machineAccessor.smartStandby();
    if (current) machineAccessor.setSmartStandby({ ...current, mode });
  };
  const setSmartStandbyTime = (minutes: number) => {
    const current = machineAccessor.smartStandby();
    if (current)
      machineAccessor.setSmartStandby({
        ...current,
        time: fromMinutes(minutes),
      });
  };
  return (
    <div class="flex h-full w-full flex-col">
      <div class="flex w-full items-center justify-center border-b pb-2 shadow-md">
        <SpinnerButton
          onClick={disconnect}
          variant="outline"
          class="flex w-36 items-center justify-center p-0"
          loading={disconnecting()}
          disabled={disconnecting()}
        >
          <T key="disconnect" />
        </SpinnerButton>
      </div>
      <div class="flex h-full w-full pt-6 text-base">
        <div class="h-full w-1/2 pr-2">
          <div
            class={twMerge(
              'w-full rounded-lg',
              machineAccessor.smartStandby()
                ? 'border-2 shadow-md'
                : 'border opacity-50',
            )}
          >
            <div
              class={twMerge(
                'flex items-center transition-all duration-200 ease-in-out',
                machineAccessor.smartStandby() &&
                  machineAccessor.smartStandby()?.mode !== SmartStandbyMode.Off
                  ? 'border-b'
                  : '',
              )}
            >
              <div class="w-full px-3 py-2">
                <T key="smart-standby" />
              </div>
              <div class="flex h-full w-24 items-center justify-center border-l">
                <Switch
                  checked={
                    machineAccessor.smartStandby()?.mode !==
                    SmartStandbyMode.Off
                  }
                  onChange={setSmartStandbyEnabled}
                  disabled={!machineAccessor.smartStandby()}
                >
                  <SwitchControl>
                    <SwitchThumb />
                  </SwitchControl>
                </Switch>
              </div>
            </div>
            <Collapsible
              open={
                machineAccessor.smartStandby() &&
                machineAccessor.smartStandby()?.mode !== SmartStandbyMode.Off
              }
            >
              <CollapsibleContent>
                <div class="flex w-full flex-col gap-2 px-3 pt-2 pb-3">
                  <div class="w-full">
                    <T key="smart-standby-mode" />
                  </div>
                  <div class="flex rounded-md border">
                    <div
                      class={twMerge(
                        'active:bg-secondary flex w-1/2 items-center justify-center rounded-l-md border-r py-1',
                        machineAccessor.smartStandby()?.mode ===
                          SmartStandbyMode.LastBrew
                          ? 'bg-secondary inset-shadow-sm'
                          : '',
                      )}
                      onClick={() =>
                        setSmartStandbyMode(SmartStandbyMode.LastBrew)
                      }
                    >
                      <T key="last-brew" />
                    </div>
                    <div
                      class={twMerge(
                        'active:bg-secondary flex w-1/2 items-center justify-center rounded-r-md py-1',
                        machineAccessor.smartStandby()?.mode ===
                          SmartStandbyMode.PowerOn
                          ? 'bg-secondary inset-shadow-sm'
                          : '',
                      )}
                      onClick={() =>
                        setSmartStandbyMode(SmartStandbyMode.PowerOn)
                      }
                    >
                      <T key="power-up" />
                    </div>
                  </div>
                  <div class="w-full pt-2">
                    <T key="smart-standby-time" />
                  </div>
                  <NumberField
                    onFocusIn={(e) => e.preventDefault()}
                    onRawValueChange={setSmartStandbyTime}
                    rawValue={
                      machineAccessor.smartStandby()?.time
                        ? toMinutes(machineAccessor.smartStandby()?.time)
                        : undefined
                    }
                    formatOptions={{ style: 'unit', unit: 'minute' }}
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
              </CollapsibleContent>
            </Collapsible>
          </div>
        </div>
        <div class="flex h-full w-1/2 flex-col gap-4 pl-2">
          <div
            class={twMerge(
              'w-full rounded-lg',
              machineAccessor.boilers()
                ? 'border-2 shadow-md'
                : 'border opacity-50',
            )}
          >
            <div class="flex items-center border-b">
              <div class="w-full px-3 py-2">
                <T key="coffee-boiler" />
              </div>
              <div class="flex h-full w-24 items-center justify-center border-l">
                <Show
                  when={machineAccessor.boilers()?.coffeeBoiler.temperature}
                >
                  {(temp) => <>{temp()} °C</>}
                </Show>
              </div>
            </div>
            <div class="flex w-full flex-col gap-2 px-3 pt-2 pb-3">
              <div class="w-full">
                <T key="target-temperature" />
              </div>
              <NumberField
                onFocusIn={(e) => e.preventDefault()}
                onRawValueChange={setCoffeeTemperature}
                rawValue={
                  machineAccessor.boilers()?.coffeeBoiler.targetTemperature
                }
                formatOptions={{ style: 'unit', unit: 'celsius' }}
                minValue={60}
                maxValue={99}
                step={1}
                disabled={!machineAccessor.boilers()}
              >
                <NumberFieldGroup class="flex items-center justify-center">
                  <NumberFieldDecrementTrigger aria-label="Decrement" />
                  <NumberFieldInput />
                  <NumberFieldIncrementTrigger aria-label="Increment" />
                </NumberFieldGroup>
              </NumberField>
            </div>
          </div>
          <div
            class={twMerge(
              'w-full rounded-lg',
              machineAccessor.boilers()
                ? 'border-2 shadow-md'
                : 'border opacity-50',
            )}
          >
            <div
              class={twMerge(
                'flex items-center transition-all duration-200 ease-in-out',
                machineAccessor.boilers()?.steamBoiler.isEnabled
                  ? 'border-b'
                  : '',
              )}
            >
              <div class="w-full px-3 py-2">
                <T key="steam-boiler" />
              </div>
              <div class="flex h-full w-24 items-center justify-center border-l">
                <Switch
                  checked={machineAccessor.boilers()?.steamBoiler.isEnabled}
                  onChange={(v) => machineAccessor.setSteamEnabled(v)}
                  disabled={!machineAccessor.boilers()}
                >
                  <SwitchControl>
                    <SwitchThumb />
                  </SwitchControl>
                </Switch>
              </div>
            </div>
            <Collapsible
              open={machineAccessor.boilers()?.steamBoiler.isEnabled}
            >
              <CollapsibleContent>
                <div class="flex w-full flex-col gap-2 px-3 pt-2 pb-3">
                  <div class="w-full">
                    <T key="target-power" />
                  </div>
                  <div class="mt-1 flex h-10 w-full items-center">
                    <div class="flex w-1/3 justify-center">
                      <div
                        class={twMerge(
                          'flex h-10 w-10 items-center justify-center rounded-full border',
                          machineAccessor.boilers()?.steamBoiler.targetLevel ===
                            1
                            ? 'bg-secondary inset-shadow-sm'
                            : '',
                          machineAccessor.boilers()
                            ? 'active:bg-secondary'
                            : '',
                        )}
                        onClick={() =>
                          machineAccessor.boilers() &&
                          machineAccessor.setSteamLevel(1)
                        }
                      >
                        <img
                          src={pictures().steam}
                          class="h-5 w-5 object-contain"
                        />
                      </div>
                    </div>
                    <div class="flex w-1/3 justify-center">
                      <div
                        class={twMerge(
                          'flex h-10 w-10 items-center justify-center rounded-full border',
                          machineAccessor.boilers()?.steamBoiler.targetLevel ===
                            2
                            ? 'bg-secondary inset-shadow-sm'
                            : '',
                          machineAccessor.boilers()
                            ? 'active:bg-secondary'
                            : '',
                        )}
                        onClick={() =>
                          machineAccessor.boilers() &&
                          machineAccessor.setSteamLevel(2)
                        }
                      >
                        <img
                          src={pictures().steam}
                          class="h-6 w-6 object-contain"
                        />
                      </div>
                    </div>
                    <div class="flex w-1/3 justify-center">
                      <div
                        class={twMerge(
                          'flex h-10 w-10 items-center justify-center rounded-full border',
                          machineAccessor.boilers()?.steamBoiler.targetLevel ===
                            3
                            ? 'bg-secondary inset-shadow-sm'
                            : '',
                          machineAccessor.boilers()
                            ? 'active:bg-secondary'
                            : '',
                        )}
                        onClick={() =>
                          machineAccessor.boilers() &&
                          machineAccessor.setSteamLevel(3)
                        }
                      >
                        <img
                          src={pictures().steam}
                          class="h-7 w-7 object-contain"
                        />
                      </div>
                    </div>
                  </div>
                </div>
              </CollapsibleContent>
            </Collapsible>
          </div>
        </div>
      </div>
    </div>
  );
};
