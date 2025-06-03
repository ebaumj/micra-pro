import { Icon } from '@micra-pro/shared/ui';
import { Component, JSX } from 'solid-js';
import { KeyboardContainer } from './KeyboardContainer';
import { KeyboardRow } from './KeyboardRow';
import { useKeyboardInternal } from '../KeyboardContext';
import {
  Key,
  BackspaceKey,
  CapsLockKey,
  EnterKey,
  ShiftKey,
  LayoutSelectionKey,
} from '../Keys';

export const English: Component<JSX.HTMLAttributes<HTMLDivElement>> = (
  props,
) => {
  const keyboardContext = useKeyboardInternal();
  const shift = () => keyboardContext.activeKeys().shift;

  return (
    <KeyboardContainer {...props}>
      <KeyboardRow>
        <Key value={shift() ? `~` : '`'} />
        <Key value={shift() ? `!` : '1'} />
        <Key value={shift() ? `@` : '2'} />
        <Key value={shift() ? `#` : '3'} />
        <Key value={shift() ? `$` : '4'} />
        <Key value={shift() ? `%` : '5'} />
        <Key value={shift() ? `^` : '6'} />
        <Key value={shift() ? `&` : '7'} />
        <Key value={shift() ? `*` : '8'} />
        <Key value={shift() ? `(` : '9'} />
        <Key value={shift() ? `)` : '0'} />
        <Key value={shift() ? `_` : '-'} />
        <Key value={shift() ? `+` : '='} />
        <BackspaceKey class="grow-[2.2]">
          <Icon iconName="backspace" />
        </BackspaceKey>
      </KeyboardRow>
      <KeyboardRow>
        <div class="grow-[1.8]" />
        <Key value="q" />
        <Key value="w" />
        <Key value="e" />
        <Key value="r" />
        <Key value="t" />
        <Key value="y" />
        <Key value="u" />
        <Key value="i" />
        <Key value="o" />
        <Key value="p" />
        <Key value={shift() ? `{` : '['} />
        <Key value={shift() ? `}` : ']'} />
        <Key class="grow-[1.5]" value={shift() ? `|` : '\\'} />
      </KeyboardRow>
      <KeyboardRow>
        <CapsLockKey class="grow-[2.2]">Caps</CapsLockKey>
        <Key value="a" />
        <Key value="s" />
        <Key value="d" />
        <Key value="f" />
        <Key value="g" />
        <Key value="h" />
        <Key value="j" />
        <Key value="k" />
        <Key value="l" />
        <Key value={shift() ? `:` : ';'} />
        <Key value={shift() ? `"` : "'"} />
        <EnterKey class="grow-[2.5]">
          <Icon iconName="keyboard_return" />
        </EnterKey>
      </KeyboardRow>
      <KeyboardRow>
        <ShiftKey class="grow-[2.8]">Shift</ShiftKey>
        <Key value="z" />
        <Key value="x" />
        <Key value="c" />
        <Key value="v" />
        <Key value="b" />
        <Key value="n" />
        <Key value="m" />
        <Key value={shift() ? `<` : ','} />
        <Key value={shift() ? `>` : '.'} />
        <Key value={shift() ? `?` : '/'} />
        <ShiftKey class="grow-[2.2]">Shift</ShiftKey>
        <div class="grow" />
      </KeyboardRow>
      <KeyboardRow>
        <div class="grow-[1]" />
        <LayoutSelectionKey>
          <Icon class="font-extralight" iconName="language" />
        </LayoutSelectionKey>
        <Key class="grow-[8]" value=" " />
        <div class="grow-[2]" />
      </KeyboardRow>
    </KeyboardContainer>
  );
};

export default English;
