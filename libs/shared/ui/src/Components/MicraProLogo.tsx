import { twMerge } from 'tailwind-merge';
import { Component, JSX } from 'solid-js';
import picturesImport from '../generated/pictures-import';

export const MicraProLogo: Component<JSX.HTMLAttributes<HTMLOrSVGElement>> = (
  props,
) => {
  return (
    <img class={twMerge('size-12', props.class)} src={picturesImport.logo} />
  );
};

export default MicraProLogo;
