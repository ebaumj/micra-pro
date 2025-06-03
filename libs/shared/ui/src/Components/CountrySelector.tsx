import { Component } from 'solid-js';
import {
  Combobox,
  ComboboxContent,
  ComboboxInput,
  ComboboxItem,
  ComboboxTrigger,
} from './Combobox';
import * as countryCodes from 'country-codes-list';
import * as flags from 'country-flag-icons/string/3x2';
import { CountryFlag } from './CountryFlag';

export const CountrySelector: Component<{
  country?: string;
  onCountryChange?: (country: string) => void;
  class?: string;
}> = (props) => {
  const countryName = (code?: string) =>
    code ? countryCodes.findOne('countryCode', code)?.countryNameEn : undefined;
  const countryCode = (name: string) =>
    countryCodes.findOne('countryNameEn', name)?.countryCode;
  const options = countryCodes
    .all()
    .filter((c) => Object.keys(flags).includes(c.countryCode))
    .map((c) => c.countryNameEn);
  const onChange = (name: string | null) => {
    const code = countryCode(name ?? '');
    if (code) {
      props.onCountryChange?.(code);
    }
  };
  return (
    <Combobox
      options={options}
      onChange={onChange}
      defaultValue={countryName(props.country)}
      itemComponent={(props) => (
        <ComboboxItem item={props.item}>
          <div class="flex">
            <CountryFlag
              class="mr-2 h-6 w-8"
              countryCode={countryCode(props.item.rawValue) ?? ''}
            />
            {props.item.rawValue}
          </div>
        </ComboboxItem>
      )}
      class={props.class}
    >
      <ComboboxContent
        class="no-scrollbar bottom-40 h-28 overflow-y-scroll"
        onCloseAutoFocus={(e) => e.preventDefault()}
        onFocusOutside={(e) => e.preventDefault()}
        onInteractOutside={(e) => e.preventDefault()}
      />
      <ComboboxTrigger>
        <ComboboxInput />
      </ComboboxTrigger>
    </Combobox>
  );
};
