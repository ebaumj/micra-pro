import { Component, Show } from 'solid-js';
import { Select } from './Select';
import { useTranslationContext } from '@micra-pro/shared/utils-ts';
import * as flags from 'country-flag-icons/string/3x2';
import { twMerge } from 'tailwind-merge';
import { CountryFlag } from './CountryFlag';

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
  const context = useTranslationContext();
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
