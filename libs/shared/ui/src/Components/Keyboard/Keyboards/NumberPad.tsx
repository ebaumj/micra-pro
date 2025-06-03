import { Component, JSX } from 'solid-js';
import { KeyboardContainer } from './KeyboardContainer';
import { KeyboardRow } from './KeyboardRow';
import { Key, BackspaceKey, EnterKey } from '../Keys';
import { Icon } from '@micra-pro/shared/ui';

export const NumberPad: Component<JSX.HTMLAttributes<HTMLDivElement>> = (
  props,
) => {
  return (
    <KeyboardContainer {...props}>
      <div class="flex w-full justify-center">
        <div class="w-1/2">
          <KeyboardRow>
            <Key value="1" />
            <Key value="2" />
            <Key value="3" />
            <Key value="-" />
          </KeyboardRow>
          <KeyboardRow>
            <Key value="4" />
            <Key value="5" />
            <Key value="6" />
            <Key value=" ">
              <Icon iconName="space_bar" />
            </Key>
          </KeyboardRow>
          <KeyboardRow>
            <Key value="7" />
            <Key value="8" />
            <Key value="9" />
            <BackspaceKey>
              <Icon iconName="backspace" />
            </BackspaceKey>
          </KeyboardRow>
          <KeyboardRow>
            <Key value="." />
            <Key value="0" />
            <Key value="," />
            <EnterKey>
              <Icon iconName="keyboard_return" />
            </EnterKey>
          </KeyboardRow>
        </div>
      </div>
    </KeyboardContainer>
  );
};

export default NumberPad;
