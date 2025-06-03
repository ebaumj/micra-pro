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
import { AltGrKey } from '../Keys/ActiveKeys';

export const German: Component<JSX.HTMLAttributes<HTMLDivElement>> = (
  props,
) => {
  const keyboardContext = useKeyboardInternal();
  const shift = () => keyboardContext.activeKeys().shift;
  const altGr = () => keyboardContext.activeKeys()['alt-gr'];

  const createKey = (key: {
    default: string;
    shift?: string;
    altGr?: string;
  }) => {
    if (shift() && key.shift) {
      return key.shift;
    }
    if (altGr() && key.altGr) {
      return key.altGr;
    }
    return key.default;
  };

  return (
    <KeyboardContainer {...props}>
      <KeyboardRow>
        <Key value={createKey({ default: '^', shift: '°' })} />
        <Key value={createKey({ default: '1', shift: '!' })} />
        <Key value={createKey({ default: '2', shift: '"', altGr: '²' })} />
        <Key value={createKey({ default: '3', shift: '§', altGr: '³' })} />
        <Key value={createKey({ default: '4', shift: '$', altGr: '' })} />
        <Key value={createKey({ default: '5', shift: '%', altGr: '' })} />
        <Key value={createKey({ default: '6', shift: '&', altGr: '' })} />
        <Key value={createKey({ default: '7', shift: '/', altGr: '{' })} />
        <Key value={createKey({ default: '8', shift: '(', altGr: '[' })} />
        <Key value={createKey({ default: '9', shift: ')', altGr: ']' })} />
        <Key value={createKey({ default: '0', shift: '=', altGr: '}' })} />
        <Key
          autoCapitalize={false}
          value={createKey({ default: 'ß', shift: '?', altGr: '\\' })}
        />
        <Key value={createKey({ default: '´', shift: '`' })} />
        <BackspaceKey class="grow-[3.2]">
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
        <Key value="z" />
        <Key value="u" />
        <Key value="i" />
        <Key value="o" />
        <Key value="p" />
        <Key value="ü" />
        <Key value={createKey({ default: '+', shift: '*', altGr: '~' })} />
        <div class="grow-[2.8]" />
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
        <Key value="ö" />
        <Key value="ä" />
        <Key value={createKey({ default: '#', shift: "'" })} />
        <EnterKey class="grow-[2.5]">
          <Icon iconName="keyboard_return" />
        </EnterKey>
      </KeyboardRow>
      <KeyboardRow>
        <ShiftKey class="grow-[1.5]">Shift</ShiftKey>
        <Key value={createKey({ default: '<', shift: '>', altGr: '|' })} />
        <Key value="y" />
        <Key value="x" />
        <Key value="c" />
        <Key value="v" />
        <Key value="b" />
        <Key value="n" />
        <Key value="m" />
        <Key value={shift() ? `;` : ','} />
        <Key value={shift() ? `:` : '.'} />
        <Key value={shift() ? `_` : '-'} />
        <ShiftKey class="grow-[4]">Shift</ShiftKey>
      </KeyboardRow>
      <KeyboardRow>
        <div class="grow-[1]" />
        <LayoutSelectionKey>
          <Icon class="font-extralight" iconName="language" />
        </LayoutSelectionKey>
        <Key class="grow-[8]" value=" " />
        <AltGrKey>AltGr</AltGrKey>
        <div class="grow-[2]" />
      </KeyboardRow>
    </KeyboardContainer>
  );
};

export default German;
