import { handleError, Icon, SpinnerButton } from '@micra-pro/shared/ui';
import { Component, createSignal } from 'solid-js';
import { twMerge } from 'tailwind-merge';
import { useBackupContext } from './BackupContextProvider';
import { useTranslationContext } from '../generated/language-types';

export const BackupButton: Component<{
  class?: string;
}> = (props) => {
  const { t } = useTranslationContext();
  const context = useBackupContext();
  const [loading, setLoading] = createSignal(false);
  const startBackup = () => {
    setLoading(true);
    context
      .backup()
      .then(() => setLoading(false))
      .catch(() => {
        setLoading(false);
        handleError({ title: t('backup-failed') });
      });
  };
  return (
    <>
      <SpinnerButton
        class={twMerge('h-full w-full', props.class)}
        variant="outline"
        loading={loading()}
        disabled={!context.enabled()}
        onClick={startBackup}
      >
        <Icon
          iconName="cloud_upload"
          class={twMerge('flex items-center text-3xl')}
        />
      </SpinnerButton>
    </>
  );
};
