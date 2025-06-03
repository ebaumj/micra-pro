import { Config } from 'tailwindcss';

import kobalte from '@kobalte/tailwindcss';

export default {
  content: ['./apps/**/*.{js,jsx,ts,tsx,html}', './libs/**/*.{js,jsx,ts,tsx}'],
  darkMode: ['class', '[data-kb-theme="dark"]'],
  theme: {
    extend: {
      fontFamily: {
        brown: [
          'Brown',
          '-apple-system',
          'system-ui',
          'Segoe UI',
          'Helvetica',
          'Arial',
          'sans-serif',
        ],
        'material-outlined': ['Material Symbols Outlined'],
      },
      opacity: {
        '8': '.08',
        '12': '.12',
        '38': '.38',
      },
      colors: {
        border: 'hsl(0 0% 89.8%)',
        input: 'hsl(0 0% 89.8%)',
        ring: 'hsl(0 0% 3.9%)',
        background: 'hsl(0 0% 100%)',
        foreground: 'hsl(0 0% 3.9%)',
        primary: {
          DEFAULT: 'hsl(0 0% 9%)',
          foreground: 'hsl(0 0% 98%)',
        },
        secondary: {
          DEFAULT: 'hsl(0 0% 96.1%)',
          foreground: 'hsl(0 0% 9%)',
        },
        destructive: {
          DEFAULT: 'hsl(0 61% 45%)',
          foreground: 'hsl(0 0% 98%)',
        },
        muted: {
          DEFAULT: 'hsl(0 0% 96.1%)',
          foreground: 'hsl(0 0% 45.1%)',
        },
        accent: {
          DEFAULT: 'hsl(0 0% 96.1%)',
          foreground: 'hsl(0 0% 9%)',
        },
        popover: {
          DEFAULT: 'hsl(0 0% 100%)',
          foreground: 'hsl(0 0% 3.9%)',
        },
        card: {
          DEFAULT: 'hsl(0 0% 100%)',
          foreground: 'hsl(0 0% 3.9%)',
        },
      },
    },
  },
  plugins: [require('tailwindcss-animate'), kobalte],
} satisfies Config;
