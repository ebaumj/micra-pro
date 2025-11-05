import {
  createContext,
  createEffect,
  createSignal,
  onMount,
  ParentComponent,
  useContext,
} from 'solid-js';

const DarkModeContext = createContext<{
  darkMode: () => boolean;
  setDarkMode: (value: boolean) => void;
}>();

export const useDarkModeContext = () => {
  const ctx = useContext(DarkModeContext);
  if (!ctx)
    throw new Error('useDarkModeContext must be used within DarkModeProvider!');
  return ctx;
};

export const selectPicturesForMode = (picturesRoot: { _dark: any }) => {
  const darkMode = useDarkModeContext();
  return () => (darkMode.darkMode() ? picturesRoot._dark : picturesRoot);
};

export const DarkModeProvider: ParentComponent = (props) => {
  const [darkMode, setDarkMode] = createSignal(false);
  onMount(() => {
    setDarkMode(localStorage['theme'] === 'dark');
    createEffect(() => {
      const theme = darkMode() ? 'dark' : 'light';
      localStorage['theme'] = theme;
      document.documentElement.dataset['theme'] = theme;
    });
  });
  return (
    <DarkModeContext.Provider value={{ darkMode, setDarkMode }}>
      {props.children}
    </DarkModeContext.Provider>
  );
};
