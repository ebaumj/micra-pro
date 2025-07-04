import { Component, createEffect, Show } from 'solid-js';
import { Select } from './Select';
import {
  createConfigAccessor,
  useTranslationContext,
} from '@micra-pro/shared/utils-ts';
import * as flags from 'country-flag-icons/string/3x2';
import { twMerge } from 'tailwind-merge';
import { CountryFlag } from './CountryFlag';
import { useKeyboardInternal } from './Keyboard/KeyboardContext';
import { LayoutOptions, Layouts } from './Keyboard/Keyboards';

const LanguageFlag: Component<{ language?: string; class?: string }> = (
  props,
) => {
  const countryCode = (language?: string) => {
    const flageName = Object.keys(flags).find(
      (f) => f === language?.toUpperCase(),
    );
    if (flageName) return flageName;
    // Language Code is not Country code
    switch (language) {
      case 'en':
        return 'GB';
      case 'de_CH':
        return 'CH';
    }
    return undefined;
  };
  return (
    <>
      <Show when={countryCode(props.language)}>
        <CountryFlag
          countryCode={countryCode(props.language)!}
          class={props.class}
        />
      </Show>
      <Show when={!countryCode(props.language)}>
        <span class={twMerge(props.class, 'flex items-center justify-center')}>
          {props.language?.toUpperCase()}
        </span>
      </Show>
    </>
  );
};

export const LanguageSelector: Component<{ class?: string }> = (props) => {
  const context = useTranslationContext();
  const keyboard = useKeyboardInternal();
  const configAccessor = createConfigAccessor<{ language: string }>('Language');
  createEffect(() => {
    const lan = configAccessor.config()?.language;
    if (lan) context.changeLanguage(lan);
  });
  createEffect(() => {
    var lan = context.language();
    if (LayoutOptions.includes(lan)) keyboard.setLayout(lan as Layouts);
  });
  return (
    <Select
      options={context.languages().map((l) => l)}
      displayElement={(lan) => (
        <div class="w-10 px-1">
          <div class="flex items-center justify-center">
            <LanguageFlag language={lan} class="h-6 w-8" />
          </div>
        </div>
      )}
      onChange={(val) => configAccessor.writeConfig({ language: val })}
      value={context.language()}
      class={props.class}
    />
  );
};
