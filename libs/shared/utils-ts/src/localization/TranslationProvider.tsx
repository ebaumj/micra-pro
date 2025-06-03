import i18next, { type TFunction } from 'i18next';
import {
  createContext,
  createSignal,
  useContext,
  Accessor,
  createMemo,
  createEffect,
  type ParentComponent,
} from 'solid-js';
import ICU from 'i18next-icu';

export type TranslationContextType = {
  t: Accessor<TFunction>;
  changeLanguage: (language: string) => Promise<void>;
  language: Accessor<string>;
  languages: Accessor<readonly string[]>;
};

const TranslationContext = createContext<TranslationContextType>();

export type TranslationResource = {
  namespace: string;
  resources: {
    [key: string]: {
      [key: string]: string | { defaultMessage: string };
    };
  };
};

export const useTranslationContext = () => {
  const context = useContext(TranslationContext);
  if (!context)
    throw new Error(
      'TranslationContext not found. Please wrap your component in a TranslationProvider',
    );
  return context;
};

export const TranslationProvider: ParentComponent<{
  resources: TranslationResource[];
  language: string;
}> = (props) => {
  const resourcesTransformed = createMemo(() =>
    transformResources(props.resources),
  );

  const [lan, setLan] = createSignal('');
  createEffect(() => setLan(props.language));

  const instance = createMemo(() => {
    const instance = i18next.createInstance();
    instance.use(ICU);

    instance.init({
      lng: props.language,
      fallbackLng: 'en',
      resources: resourcesTransformed(),
    });

    return instance;
  });

  const [translate, setTranslate] = createSignal<TFunction>(
    // will get updated with effect below, so it can be safely ignored here.
    // eslint-disable-next-line solid/reactivity
    instance().t,
  );

  createEffect(() => {
    const t = instance().t;
    setTranslate(() => t);
  });

  async function changeLanguage(language: string) {
    const t = await instance().changeLanguage(language);
    setTranslate(() => t);
    setLan(language);
  }

  const availableLangauages = (): string[] => {
    const res = instance().options.resources;
    return res ? Object.keys(res) : [];
  };

  const context = {
    changeLanguage,
    t: translate,
    language: lan,
    languages: availableLangauages,
  };

  return (
    <TranslationContext.Provider value={context} children={props.children} />
  );
};

const transformResources = (resources: TranslationResource[]) => {
  return resources.reduce(
    (acc, { namespace, resources }) => {
      for (const [lng, content] of Object.entries(resources)) {
        acc[lng] = acc[lng] || {};

        acc[lng][namespace] = Object.fromEntries(
          Object.entries(content).map(([key, value]) => [
            key.replace(namespace + '.', ''),
            typeof value === 'string' ? value : value.defaultMessage,
          ]),
        );
      }
      return acc;
    },
    {} as {
      [lng: string]: { [namespace: string]: { [key: string]: string } };
    },
  );
};
