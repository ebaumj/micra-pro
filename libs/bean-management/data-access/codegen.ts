import type { CodegenConfig } from '@graphql-codegen/cli';

const config: CodegenConfig = {
  overwrite: true,
  schema: '../data-definition-graphql/src/generated/schema.graphqls',
  generates: {
    'src/generated/graphql.ts': {
      documents: 'src/**/*.graphql',
      plugins: [
        'typescript-react-apollo',
        'typescript-operations',
        'typescript',
      ],
      config: {
        withHooks: false,
        gqlImport: 'graphql-tag',
        skipTypename: true,
        scalars: {
          Byte: 'number',
          UUID: 'string',
          DateTime: 'string',
        },
      },
    },
  },
};

export default config;
