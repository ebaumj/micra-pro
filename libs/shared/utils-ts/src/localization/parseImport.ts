import { TranslationResource } from './TranslationProvider';

type LocaleImport = {
  default: { [key: string]: string | { defaultMessage: string } };
};

function assertLocaleImport(
  value: unknown,
  namespace: string,
): asserts value is LocaleImport {
  if (
    typeof value !== 'object' ||
    value === null ||
    !('default' in value) ||
    typeof value.default !== 'object' ||
    value.default === null
  ) {
    throw new Error('Invalid locale import');
  }
  Object.entries(value.default).forEach(
    ([translationKey, translationValue]) => {
      if (
        typeof translationValue !== 'string' &&
        !(
          typeof translationValue === 'object' &&
          translationValue !== null &&
          'defaultMessage' in translationValue
        )
      ) {
        throw new Error(
          'Invalid locale import. Value must be a string or an object with a defaultMessage property. Key was: ' +
            translationKey,
        );
      }

      if (!translationKey.startsWith(namespace + '.')) {
        throw new Error(
          `Key "${translationKey}" does not start with the namespace "${namespace}"`,
        );
      }
    },
  );
}

export function parseImport(
  namespace: string,
  imported: Record<string, unknown>,
): TranslationResource {
  const resources = Object.fromEntries(
    Object.entries(imported).map(([fileName, value]) => {
      assertLocaleImport(value, namespace);

      const localeName = fileName.split('/').pop()?.replace('.json', '');
      if (!localeName) {
        throw new Error('Invalid locale file name');
      }

      return [localeName, value.default];
    }),
  );

  return {
    namespace: namespace,
    resources: resources,
  };
}
