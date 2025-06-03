import { readConfig, writeConfig } from '@micra-pro/shared/utils-ts';
import {
  Accessor,
  createContext,
  createEffect,
  createRoot,
  createSignal,
  ParentComponent,
  useContext,
} from 'solid-js';

type ScaleSelectorContextType = {
  selectedScale: Accessor<string | undefined>;
  setSelectedScale: (value: string) => Promise<void>;
};

const ScaleSelectorContext = createContext<ScaleSelectorContextType>();

export const useScaleSelectorContext = () => {
  const context = useContext(ScaleSelectorContext);
  if (!context)
    throw new Error(
      'ScaleSelectorContext not found. Please wrap your component in a ScaleSelectorContextProvider',
    );
  return context;
};

export const useSelectedScaleContext = () => ({
  selectedScale: useScaleSelectorContext().selectedScale,
});

export const ScaleSelectorContextProvider: ParentComponent = (props) => {
  const [selectedScale, setSelectedScale] = createSignal<string | undefined>(
    undefined,
  );
  const [next, setNext] = createSignal<string>();
  const [response, setResponse] = createSignal<
    'success' | 'fail' | undefined
  >();

  readConfig<{ id: string }>('SelectedScale')
    .then((s) => setSelectedScale(s.id))
    .catch(() => console.log('No config found for Selected scale'));

  createEffect(() => {
    const value = next();
    if (value)
      writeConfig('SelectedScale', { id: value })
        .then((v) => {
          if (v.id === value) {
            setSelectedScale(v.id);
            setResponse('success');
          } else setResponse('fail');
        })
        .catch(() => setResponse('fail'));
  });

  const setScale = (value: string): Promise<void> => {
    setResponse(undefined);
    setNext(value);
    return createRoot(
      (dispose) =>
        new Promise<void>((resolve, reject) => {
          createEffect(() => {
            switch (response()) {
              case 'success':
                resolve();
                dispose();
                break;
              case 'fail':
                reject();
                dispose();
                break;
            }
          });
        }),
    );
  };

  return (
    <ScaleSelectorContext.Provider
      value={{ selectedScale, setSelectedScale: (value) => setScale(value) }}
      children={props.children}
    />
  );
};
