export { GraphQlProvider } from './graphql-client/GraphQlClientProvider';
export { createQuery } from './graphql-client/createQuery';
export { createMutation } from './graphql-client/createMutation';
export * from './localization/parseImport';
export * from './localization/TranslationProvider';
export * from './localization/useNamespaceTranslationContext';
export * from './date-time/dateString';
export * from './typescript/typescript-extensions';

import * as configStorage from './config-storage/config-storage';
import * as connectionTest from './connection-test/connection-test';
import * as systemAccess from './system-access/system-access';

import * as configStorageDummy from './config-storage/config-storage-dummy';
import * as connectionTestDummy from './connection-test/connection-test-dummy';
import * as systemAccessDummy from './system-access/system-access-dummy';

declare const __USE_DUMMIES__: boolean;
const exportValue = __USE_DUMMIES__
  ? {
      ...configStorageDummy,
      ...connectionTestDummy,
      ...systemAccessDummy,
    }
  : {
      ...configStorage,
      ...connectionTest,
      ...systemAccess,
    };

export const {
  createConfigAccessor,
  createSystemAccessor,
  testConnection,
  updateAccess,
  wifiAccess,
} = exportValue;
export { type ConfigKey } from './config-storage/config-storage';
export {} from './connection-test/connection-test';
export {} from './system-access/system-access';
