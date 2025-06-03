import { createContext, ParentComponent, useContext } from 'solid-js';
import { createStore } from 'solid-js/store';
import { Layouts, keyboards } from './Keyboards';

export type ActiveKeysType = 'caps-lock' | 'shift' | 'alt-gr';

type KeyboardStore = {
  currentInput: HTMLInputElement | null;
  active: {
    [key in ActiveKeysType]: boolean;
  };
  layout: Layouts;
};

const createKeyboardContext = () => {
  const [store, setStore] = createStore<KeyboardStore>({
    currentInput: null,
    active: {
      'caps-lock': false,
      shift: false,
      'alt-gr': false,
    },
    layout: 'en',
  });

  const CanToggleActive = (key: ActiveKeysType) => {
    if (key === 'shift') return !store.active['alt-gr'];
    if (key === 'alt-gr') return !store.active.shift;
    return true;
  };

  return {
    isOpen: () => store.currentInput !== null,
    input: () => store.currentInput,
    inputPosition: () => store.currentInput?.getBoundingClientRect(),

    inputFocused: (input: HTMLInputElement) => {
      setStore('currentInput', input);
    },
    inputBlurred: () => {
      setStore('currentInput', null);
    },

    activeKeys: () => store.active,
    canToggleActive: CanToggleActive,
    toggleActive: (key: ActiveKeysType) =>
      CanToggleActive(key) && setStore('active', key, (value) => !value),

    currentLayout: () => store.layout,
    availableLayouts: () => Object.keys(keyboards) as Layouts[],
    setLayout: (layout: Layouts) => {
      setStore('layout', layout);
    },
  };
};

export type KeyboardContextTypeInternal = ReturnType<
  typeof createKeyboardContext
>;

export type KeyboardContextType = Pick<
  KeyboardContextTypeInternal,
  'isOpen' | 'inputPosition'
>;

const KeyboardContext = createContext<KeyboardContextTypeInternal>();

export const useKeyboardInternal = () => {
  const context = useContext(KeyboardContext);
  if (context === undefined) {
    throw new Error(`useKeyboard must be used within a KeyboardProvider`);
  }
  return context;
};

export const useKeyboardInfo = useKeyboardInternal as () => KeyboardContextType;

export const KeyboardProvider: ParentComponent = (props) => {
  const value = createKeyboardContext();
  return (
    <KeyboardContext.Provider value={value}>
      {props.children}
    </KeyboardContext.Provider>
  );
};
