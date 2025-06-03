import { useTranslationContext } from './TranslationProvider';

export function useNamespaceTranslationContext<TFunction>(namespace: string) {
  const context = useTranslationContext();
  return {
    changeLanguage: context.changeLanguage,
    t: ((key: string, values: any) =>
      context.t()(key, {
        ...values,
        ns: namespace,
      })) as TFunction,
  };
}
