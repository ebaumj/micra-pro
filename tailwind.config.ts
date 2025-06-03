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
      keyframes: {
        'spin-loader': {
          '0%': { 'clip-path': 'polygon(50% 50%,0 0,0 0,0 0,0 0,0 0)' },
          '25%': {
            'clip-path': 'polygon(50% 50%,0 0,100% 0,100% 0,100% 0,100% 0)',
          },
          '50%': {
            'clip-path':
              'polygon(50% 50%,0 0,100% 0,100% 100%,100% 100%,100% 100%)',
          },
          '75%': {
            'clip-path': 'polygon(50% 50%,0 0,100% 0,100% 100%,0 100%,0 100%)',
          },
          '100%': {
            'clip-path': 'polygon(50% 50%,0 0,100% 0,100% 100%,0 100%,0 0)',
          },
        },
      },
      animation: {
        'spin-loader-0.5s': 'spin-loader 0.5s linear infinite',
        'spin-loader-1s': 'spin-loader 1s linear infinite',
        'spin-loader-1.5s': 'spin-loader 1.5s linear infinite',
        'spin-loader-2s': 'spin-loader 2s linear infinite',
      },
    },
  },
  plugins: [require('tailwindcss-animate'), kobalte],
} satisfies Config;
