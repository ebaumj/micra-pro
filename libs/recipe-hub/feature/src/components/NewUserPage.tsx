import { Component, createSignal, Show } from 'solid-js';
import { T, useTranslationContext } from '../generated/language-types';
import { createUserFactory } from '@micra-pro/recipe-hub/data-access';
import {
  Button,
  handleError,
  SpinnerButton,
  TextField,
  TextFieldRoot,
} from '@micra-pro/shared/ui';

export const NewUserPage: Component<{ close: () => void }> = (props) => {
  const { t } = useTranslationContext();
  const userFactory = createUserFactory();
  const [state, setState] = createSignal<'create' | 'loading' | 'success'>(
    'create',
  );
  const [user, setUser] = createSignal('');
  const [password, setPassword] = createSignal('');
  const [email, setEmail] = createSignal('');
  const userValid = () => user().length > 0;
  const passwordValid = () => password().length > 0;
  const emailValid = () => validateEmail(email());
  const result = (success: boolean) => {
    setState(success ? 'success' : 'create');
    if (!success)
      handleError({ title: t('failed'), message: t('username-email-taken') });
  };
  const create = () => {
    setState('loading');
    userFactory
      .createUser(user(), password(), email())
      .then(result)
      .catch(() => result(false));
  };
  return (
    <div class="flex h-full w-full flex-col items-center justify-center gap-1">
      <Show when={state() === 'create' || state() === 'loading'}>
        <TextFieldRoot
          onChange={setUser}
          value={user()}
          validationState={userValid() ? 'valid' : 'invalid'}
          disabled={state() === 'loading'}
        >
          <TextField placeholder={t('username')} />
        </TextFieldRoot>
        <TextFieldRoot
          onChange={(pw) => setPassword(pw.trim().replace(' ', ''))}
          value={password()}
          validationState={passwordValid() ? 'valid' : 'invalid'}
          disabled={state() === 'loading'}
        >
          <TextField placeholder={t('password')} type="password" />
        </TextFieldRoot>
        <TextFieldRoot
          onChange={setEmail}
          value={email()}
          validationState={emailValid() ? 'valid' : 'invalid'}
          disabled={state() === 'loading'}
        >
          <TextField placeholder={t('email')} />
        </TextFieldRoot>
        <SpinnerButton
          class="mt-3 w-24"
          loading={state() === 'loading'}
          onClick={create}
          disabled={!emailValid() || !userValid() || !passwordValid()}
        >
          <T key="create" />
        </SpinnerButton>
        <Button variant="outline" class="w-24" onClick={props.close}>
          <T key="cancel" />
        </Button>
      </Show>
      <Show when={state() === 'success'}>
        <div class="mb-4 text-lg">
          <T key="verify-email" />
        </div>
        <Button class="w-24" onClick={props.close}>
          <T key="ok" />
        </Button>
      </Show>
    </div>
  );
};

const validateEmail = (email: string) => {
  const regex = new RegExp(
    /^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+\\/0-9=?A-Z^_a-z`{|}~])*@[a-zA-Z0-9](-*\.?[a-zA-Z0-9])*\.[a-zA-Z](-?[a-zA-Z0-9])+$/,
  );
  if (!email) return false;
  if (email.length > 254) return false;
  var valid = regex.test(email);
  if (!valid) return false;
  var parts = email.split('@');
  if (parts[0].length > 64) return false;
  var domainParts = parts[1].split('.');
  if (
    domainParts.some(function (part) {
      return part.length > 63;
    })
  )
    return false;
  return true;
};
