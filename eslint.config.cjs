const solidTsConfig = require('eslint-plugin-solid/configs/typescript');
const nxEslintPlugin = require('@nx/eslint-plugin');
const typescriptEslintPlugin = require('@typescript-eslint/eslint-plugin');
const js = require('@eslint/js');
const typescriptEslintParser = require('@typescript-eslint/parser');
const graphqlEslintPlugin = require('@graphql-eslint/eslint-plugin');
const vitest = require('@vitest/eslint-plugin');

module.exports = [
  {
    files: ['**/*.test.ts'],
    plugins: {
      vitest,
    },
    languageOptions: {
      globals: {
        ...vitest.environments.env.globals,
      },
    },
    rules: {
      ...vitest.configs.recommended.rules,
      'vitest/max-nested-describe': ['error', { max: 3 }],
    },
  },
  js.configs.recommended,
  {
    plugins: {
      '@typescript-eslint': typescriptEslintPlugin,
      '@nx': nxEslintPlugin,
    },
  },
  {
    files: ['**/*.ts', '**/*.tsx', '**/*.js', '**/*.jsx'],
    ...solidTsConfig,
    languageOptions: {
      parser: typescriptEslintParser,
      parserOptions: {
        ecmaVersion: 'latest',
        sourceType: 'module',
      },
    },
  },
  {
    rules: {
      'no-unused-vars': 'off',
      'no-undef': 'off',
      '@typescript-eslint/no-unused-vars': [
        'error',
        { argsIgnorePattern: '^_' },
      ],
      '@typescript-eslint/no-unused-expressions': ['warn'],
      '@nx/enforce-module-boundaries': 'off',
    },
  },
  {
    files: ['**/*.{graphqls,graphql}'],
    ignores: [],
    languageOptions: {
      parser: graphqlEslintPlugin.parser,
      parserOptions: {
        graphQLConfig: {
          schema: 'libs/**/src/**/*.graphqls',
          documents: 'libs/**/src/**/*.graphql',
        },
      },
    },
    plugins: {
      '@graphql-eslint': graphqlEslintPlugin,
    },
    rules: graphqlEslintPlugin.configs['flat/schema-recommended'].rules,
  },
  {
    files: ['**/*.graphqls'],
    rules: {
      '@graphql-eslint/require-description': ['warn'],
    },
  },
  {
    files: ['**/*.graphql'],
    ignores: [],
    rules: graphqlEslintPlugin.configs['flat/operations-recommended'].rules,
  },
  {
    ignores: ['**/vite.config.*.timestamp*', '**/vitest.config.*.timestamp*'],
  },
];
