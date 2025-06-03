import { Component, Show } from 'solid-js';
import { Select } from './Select';
import { useTranslationContext } from '@micra-pro/shared/utils-ts';
import * as flags from 'country-flag-icons/string/3x2';
import { twMerge } from 'tailwind-merge';

const LanguageFlag: Component<{ language?: string; class?: string }> = (
  props,
) => {
  const image = (language?: string) => {
    var flag = Object.entries(flags).find(
      (f) => f[0] === language?.toUpperCase(),
    );
    if (flag) return flag[1];
    // Language Code is not Country code
    switch (language) {
      case 'en':
        return flags.GB;
    }
    return undefined;
  };
  const svg = () => image(props.language);
  const imageUrl = (svg: string) =>
    URL.createObjectURL(new Blob([svg], { type: 'image/svg+xml' }));
  return (
    <>
      <Show when={svg()}>
        <img src={imageUrl(svg()!)} class={props.class} />
      </Show>
      <Show when={!svg()}>
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
