import { useTranslationContext } from './TranslationProvider';

export function useNamespaceTranslationContext<TFunction>(namespace: string) {
  const context = useTranslationContext();
  return {
    changeLanguage: context.changeLanguage,
    t:
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      ((key: string, values: any) =>
        context.t()(key, {
          ...values,
          ns: namespace,
        })) as TFunction,
  };
}
