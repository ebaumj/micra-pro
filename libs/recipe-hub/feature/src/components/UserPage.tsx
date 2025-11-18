import { RecipeHubUser } from '@micra-pro/recipe-hub/data-definition';
import {
  Button,
  handleError,
  Icon,
  makeToast,
  OTPField,
  OTPFieldGroup,
  OTPFieldInput,
  OTPFieldSeparator,
  OTPFieldSlot,
  Spinner,
  SpinnerButton,
  TextField,
  TextFieldRoot,
} from '@micra-pro/shared/ui';
import {
  Component,
  createEffect,
  createSignal,
  Match,
  Show,
  Switch,
} from 'solid-js';
import { createStore } from 'solid-js/store';
import { T, useTranslationContext } from '../generated/language-types';
import { twMerge } from 'tailwind-merge';
import qrcode from 'qrcode';

type MfaState =
  | {
      state: 'idle';
    }
  | {
      state: 'enable-qr';
      qr: string;
    }
  | {
      state: 'code';
      enable: boolean;
    };

const MfaHandler: Component<{
  isEnabled: boolean;
  createSecret: () => Promise<string | null>;
  setMfa: (code: string, enabled: boolean) => Promise<boolean>;
}> = (props) => {
  const { t } = useTranslationContext();
  const [state, setState] = createSignal<MfaState>({ state: 'idle' });
  const [loading, setLoading] = createSignal(false);
  const [qrCodeUrl, setQrCodeUrl] = createSignal('');
  const [code, setCode] = createSignal('');
  const changeMfa = () => {
    if (props.isEnabled) setState({ state: 'code', enable: false });
    else {
      setLoading(true);
      props
        .createSecret()
        .then((secret) => {
          if (secret) setState({ state: 'enable-qr', qr: secret });
          else {
            handleError({ title: t('failed') });
            cancel();
          }
        })
        .finally(() => setLoading(false));
    }
  };
  createEffect(() => {
    const mfaCode = code();
    const s = state();
    if (mfaCode.length === 6 && s.state === 'code') {
      setLoading(true);
      setCode('');
      props
        .setMfa(mfaCode, s.enable)
        .then((s) => {
          if (s) cancel();
          else handleError({ title: t('mfa-fail') });
        })
        .finally(() => {
          setLoading(false);
        });
    }
  });
  const cancel = () => setState({ state: 'idle' });
  createEffect(() => {
    const stateObject = state();
    if (stateObject.state === 'enable-qr')
      qrcode.toDataURL(stateObject.qr).then(setQrCodeUrl);
  });
  createEffect(() => {
    if (state().state !== 'code') setCode('');
  });
  return (
    <div class="h-full w-full">
      <Switch>
        <Match when={state().state === 'idle'}>
          <>
            <div class="flex w-full items-center justify-center gap-2 text-base font-bold">
              <T key="mfa" />
              <Icon
                class={twMerge(
                  'flex items-center justify-center text-2xl font-normal',
                  props.isEnabled ? 'text-positive' : 'text-destructive',
                )}
                iconName={props.isEnabled ? 'check_box' : 'disabled_by_default'}
              />
            </div>
            <div class="flex h-full w-full items-center justify-center">
              <SpinnerButton
                class="w-1/2"
                variant="outline"
                loading={loading()}
                onClick={changeMfa}
              >
                <T key={props.isEnabled ? 'disable' : 'enable'} />
              </SpinnerButton>
            </div>
          </>
        </Match>
        <Match when={state().state === 'enable-qr'}>
          <>
            <div class="flex h-32 w-full items-center justify-center gap-2 text-base font-bold">
              <img src={qrCodeUrl()} class="h-full w-full object-contain" />
            </div>
            <div class="flex justify-center overflow-hidden py-1 text-xs whitespace-nowrap">
              <T key="scan-mfa" />
            </div>
            <div class="flex w-full items-center justify-center gap-2 pt-3">
              <Button class="w-1/2" variant="outline" onClick={cancel}>
                <T key="cancel" />
              </Button>
              <Button
                class="w-1/2"
                variant="default"
                onClick={() => setState({ state: 'code', enable: true })}
              >
                <T key="ok" />
              </Button>
            </div>
          </>
        </Match>
        <Match when={state().state === 'code'}>
          <div class="flex h-full flex-col items-center justify-center">
            <div class="flex h-full w-full flex-col items-center justify-center gap-2 text-base font-bold">
              <Show when={!loading()}>
                <>
                  <OTPField
                    maxLength={6}
                    onValueChange={setCode}
                    value={code()}
                  >
                    <OTPFieldInput />
                    <OTPFieldGroup>
                      <OTPFieldSlot index={0} />
                      <OTPFieldSlot index={1} />
                      <OTPFieldSlot index={2} />
                    </OTPFieldGroup>
                    <OTPFieldSeparator />
                    <OTPFieldGroup>
                      <OTPFieldSlot index={3} />
                      <OTPFieldSlot index={4} />
                      <OTPFieldSlot index={5} />
                    </OTPFieldGroup>
                  </OTPField>
                  <div class="flex justify-center overflow-hidden py-1 text-xs font-normal whitespace-nowrap">
                    <T key="enter-mfa-code" />
                  </div>
                </>
              </Show>
              <Show when={loading()}>
                <Spinner class="h-20 w-20" />
              </Show>
            </div>
            <div class="flex w-full items-center justify-center gap-2 pt-3">
              <Button class="w-1/2" variant="outline" onClick={cancel}>
                <T key="cancel" />
              </Button>
            </div>
          </div>
        </Match>
      </Switch>
    </div>
  );
};

export const UserPage: Component<{
  user: RecipeHubUser;
  delete: () => Promise<boolean>;
  changePassword: (oldPw: string, newPw: string) => Promise<boolean>;
  createMfaSecret: () => Promise<string | null>;
  setMfa: (code: string, enabled: boolean) => Promise<boolean>;
}> = (props) => {
  const { t } = useTranslationContext();
  const [pwChange, setPwChange] = createSignal(false);
  const [password, setPassword] = createStore({ old: '', new: '', repeat: '' });
  const passwordValid = () =>
    password.old !== '' &&
    password.new !== '' &&
    password.repeat !== '' &&
    password.new === password.repeat;
  const changePassword = () => {
    setPwChange(true);
    props
      .changePassword(password.old, password.new)
      .then((s) => {
        if (s) makeToast({ title: t('change-password-success') });
        else handleError({ title: t('failed') });
      })
      .finally(() => {
        setPwChange(false);
        setPassword('old', '');
        setPassword('new', '');
        setPassword('repeat', '');
      });
  };
  const [delAccount, setDelAccount] = createSignal(false);
  const deleteAccount = () => {
    setDelAccount(true);
    props
      .delete()
      .then((s) => {
        if (!s) handleError({ title: t('failed') });
      })
      .finally(() => setDelAccount(false));
  };
  return (
    <div class="flex h-full w-full flex-col overflow-hidden">
      <div class="bg-secondary my-2 flex w-full items-center justify-center rounded-lg border text-lg font-bold inset-shadow-sm">
        <Icon
          class="mr-3 flex h-full items-center text-3xl font-normal"
          iconName="person"
        />
        {props.user.username}
      </div>
      <div class="flex w-full gap-2 border-b py-2">
        <div class="flex w-1/2 flex-col gap-2 border-r pr-2 pl-1">
          <div class="flex w-full justify-center text-base font-bold">
            <T key="change-password" />
          </div>
          <TextFieldRoot
            value={password.old}
            onChange={(v) => setPassword('old', v.trim().replace(' ', ''))}
          >
            <TextField
              class="w-full"
              placeholder={t('old-password')}
              type="password"
            />
          </TextFieldRoot>
          <TextFieldRoot
            value={password.new}
            onChange={(v) => setPassword('new', v.trim().replace(' ', ''))}
          >
            <TextField
              class="w-full"
              placeholder={t('new-password')}
              type="password"
            />
          </TextFieldRoot>
          <TextFieldRoot
            value={password.repeat}
            onChange={(v) => setPassword('repeat', v.trim().replace(' ', ''))}
          >
            <TextField
              class="w-full"
              placeholder={t('repeat-new-password')}
              type="password"
            />
          </TextFieldRoot>
          <div class="flex w-full justify-center">
            <SpinnerButton
              onClick={changePassword}
              loading={pwChange()}
              class="w-1/2"
              disabled={!passwordValid()}
            >
              <T key="save" />
            </SpinnerButton>
          </div>
        </div>
        <div class="h-full w-1/2">
          <MfaHandler
            isEnabled={props.user.mfaEnabled}
            createSecret={props.createMfaSecret}
            setMfa={props.setMfa}
          />
        </div>
      </div>
      <div class="flex h-full w-full items-center justify-center">
        <SpinnerButton
          onClick={deleteAccount}
          loading={delAccount()}
          variant="destructive"
          class="w-1/3"
        >
          <T key="delete-account" />
        </SpinnerButton>
      </div>
    </div>
  );
};
