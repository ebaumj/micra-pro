import { twMerge } from 'tailwind-merge';
import { Component, JSX } from 'solid-js';

// eslint-disable-next-line no-undef
export const MicraProLogo: Component<JSX.HTMLAttributes<HTMLOrSVGElement>> = (
  props,
) => {
  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      viewBox="0 0 207.9 133.4"
      fill="none"
      class={twMerge('text-primary-on-container size-12', props.class)}
    >
      <path
        fill="currentColor"
        d="M153 47v-2.1c0-3.8-3.1-6.9-6.8-6.9H95.9c-5.6 0-8.4 2.7-8.4 8.2V54H146c3.7.2 6.8-2.7 7-6.4V47z"
      />
      <path
        fill="currentColor"
        d="M204.8 8.2C201.4 3.1 193.4 0 187.3 0H22.5C16.3 0 11 .2 5.9 4.8S.1 14.6.1 21v87.2c0 5.9-.7 11.5 3 17 3.4 5.1 9.8 8.2 15.9 8.2h166.4c6.2 0 11.5-.1 16.5-4.8s5.9-9.8 5.9-16.2V25.2c0-5.9.7-11.4-3-17zM50.4 41c0-20.9 14-31.3 42.1-31.3h68.8c19.3 0 28.9 8.8 28.9 26.4v16.2c0 8.5-2.4 14.7-7.1 18.7-4.8 4-12 6-21.7 6H87.6v7c0 4.8 2.8 8.3 8.5 8.3h93v29.8H92.8c-28.3 0-42.5-11.4-42.5-34.1l.1-47z"
      />
    </svg>
  );
};

export default MicraProLogo;
