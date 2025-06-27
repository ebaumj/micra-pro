import { Component, createEffect, createSignal, Show } from 'solid-js';
import { Select } from './Select';
import {
  readConfig,
  useTranslationContext,
  writeConfig,
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
  let storageValue: string | undefined;
  const context = useTranslationContext();
  const [initialRead, setInitialRead] = createSignal(false);
  const keyboard = useKeyboardInternal();
  readConfig<{ language: string }>('Language')
    .then((l) => {
      context.changeLanguage(l.language);
      storageValue = l.language;
    })
    .finally(() => setInitialRead(true));
  createEffect(() => {
    var lan = context.language();
    if (lan !== storageValue && initialRead())
      writeConfig('Language', { language: lan }).then(
        (l) => (storageValue = l.language),
      );
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
      onChange={(val) => context.changeLanguage(val)}
      value={context.language()}
      class={props.class}
    />
  );
};
