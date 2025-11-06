import { handleError, Icon, SpinnerButton } from '@micra-pro/shared/ui';
import { Component, createSignal } from 'solid-js';
import { useMachineContextInternal } from './MachineContextProvider';
import { useTranslationContext } from '../generated/language-types';

export const StandbyButton: Component<{ class?: string }> = (props) => {
  const { t } = useTranslationContext();
  var ctx = useMachineContextInternal();
  const [loading, setLoading] = createSignal(false);
  const setStandby = () => {
    ctx
      .setStandby(true)
      .catch((_) => {
        handleError({
          title: t('error'),
          message: t('standby-failed'),
        });
        setLoading(false);
      })
      .then(() => setTimeout(() => setLoading(false), 1000));
  };
  return (
    <SpinnerButton
      variant="outline"
      class={props.class}
      onClick={setStandby}
      loading={loading()}
    >
      <Icon iconName="power_settings_new" />
    </SpinnerButton>
  );
};
