import { useAuthentication } from '@micra-pro/recipe-hub/data-access';
import { Component, createEffect, createSignal, Match, Switch } from 'solid-js';
import {
  Button,
  handleError,
  OTPField,
  OTPFieldGroup,
  OTPFieldInput,
  OTPFieldSeparator,
  OTPFieldSlot,
  SpinnerButton,
  TextField,
  TextFieldRoot,
} from '@micra-pro/shared/ui';
import { T, useTranslationContext } from '../generated/language-types';
import { LoggedInProfilePage } from './LoggedInProfilePage';
import { NewUserPage } from './NewUserPage';

export const RecipeHubProfilePage: Component = () => {
  const { t } = useTranslationContext();
  const authenctication = useAuthentication();
  const [state, setState] = createSignal<
    | { type: 'username' }
    | { type: 'mfa'; mfa: (code: string) => void }
    | { type: 'loggedIn' }
    | { type: 'createAccount' }
  >({ type: 'username' });
  const [loading, setLoading] = createSignal(false);
  const [password, setPassword] = createSignal('');
  const [user, setUser] = createSignal('');
  const [mfaCode, setMfaCode] = createSignal('');
  const [mfaTimeout, setMfaTimeout] = createSignal(0);
  const decrementMfaTimeout = () => {
    setMfaTimeout((to) => (to - 1 >= 0 ? to - 1 : 0));
    setTimeout(decrementMfaTimeout, 1000);
  };
  setTimeout(decrementMfaTimeout, 1000);
  createEffect(() => {
    if (!authenctication.currentUser()) setState({ type: 'username' });
    else setState({ type: 'loggedIn' });
  });
  createEffect(() => {
    const code = mfaCode();
    const s = state();
    if (code.length === 6 && s.type === 'mfa') {
      s.mfa(code);
      setMfaCode('');
    }
  });
  createEffect(() => {
    if (mfaTimeout() === 0 && state().type === 'mfa')
      setState({ type: 'username' });
  });
  createEffect(() => {
    if (state().type === 'username') {
      setUser('');
      setPassword('');
    }
  });
  const login = async () => {
    setLoading(true);
    const result = await authenctication.login(user(), password());
    setLoading(false);
    switch (result.result) {
      case 'success':
        break;
      case 'unauthorized':
        setState({ type: 'username' });
        handleError({ title: t('failed'), message: t('password-incorrect') });
        break;
      case 'failed':
        setState({ type: 'username' });
        handleError({ title: t('failed'), message: t('user-does-not-extist') });
        break;
      case 'mfa':
        setMfaCode('');
        setMfaTimeout(result.due);
        setState({
          type: 'mfa',
          mfa: (code: string) => {
            setLoading(true);
            result
              .mfa(code)
              .then((result) => {
                setLoading(false);
                switch (result.result) {
                  case 'success':
                    break;
                  case 'unauthorized':
                    setState({ type: 'username' });
                    handleError({ title: t('failed'), message: t('mfa-fail') });
                    break;
                  case 'failed':
                    setState({ type: 'username' });
                    handleError({ title: t('failed'), message: t('mfa-fail') });
                    break;
                }
              })
              .catch((_) => {
                setLoading(false);
                setState({ type: 'username' });
                handleError({ title: t('failed'), message: t('mfa-fail') });
              });
          },
        });
        break;
    }
  };
  return (
    <Switch>
      <Match when={state().type === 'username'}>
        <div class="flex h-full w-full flex-col items-center justify-center gap-1">
          <TextFieldRoot onChange={setUser} value={user()}>
            <TextField placeholder={t('username')} />
          </TextFieldRoot>
          <TextFieldRoot onChange={setPassword} value={password()}>
            <TextField placeholder={t('password')} type="password" />
          </TextFieldRoot>
          <SpinnerButton class="mt-3 w-44" loading={loading()} onClick={login}>
            <T key="login" />
          </SpinnerButton>
          <Button
            class="w-44"
            variant="outline"
            onClick={() => setState({ type: 'createAccount' })}
          >
            <T key="create-account" />
          </Button>
        </div>
      </Match>
      <Match when={state().type === 'mfa'}>
        <div class="flex h-full w-full flex-col items-center justify-center gap-1">
          <div class="mb-4 text-lg">
            <T key="enter-mfa-code" />
          </div>
          <OTPField maxLength={6} onValueChange={setMfaCode} value={mfaCode()}>
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
          <div class="mt-4 text-xl font-bold">{mfaTimeout()}</div>
        </div>
      </Match>
      <Match when={state().type === 'loggedIn'}>
        <LoggedInProfilePage
          logout={authenctication.logout}
          username={authenctication.currentUser() ?? ''}
        />
      </Match>
      <Match when={state().type === 'createAccount'}>
        <NewUserPage close={() => setState({ type: 'username' })} />
      </Match>
    </Switch>
  );
};
