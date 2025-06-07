import { Component, JSX, splitProps } from 'solid-js';
import * as flags from 'country-flag-icons/string/3x2';

type CountryFlagProps = Omit<JSX.HTMLAttributes<HTMLImageElement>, 'src'> & {
  countryCode: string;
};

const emptyFlag =
  '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 513 342"><path fill="#F0F0F0" d="M0 0h513v342H0z"/></svg>';

export const CountryFlag: Component<CountryFlagProps> = (props) => {
  const image = (code: string) => {
    var flag = Object.entries(flags).find((f) => f[0] === code.toUpperCase());
    if (flag) return flag[1];
    return emptyFlag;
  };
  const imageUrl = (svg: string) =>
    URL.createObjectURL(new Blob([svg], { type: 'image/svg+xml' }));
  const rest = splitProps(props, ['countryCode'])[1];
  return <img src={imageUrl(image(props.countryCode))} {...rest} />;
};
