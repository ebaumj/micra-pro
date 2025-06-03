import { createMemo, ParentComponent, Show } from 'solid-js';
import BackendConnectionTracker from './BackendConnectionTracker';
import {
  DialogContextProvider,
  Keyboard,
  useKeyboardInfo,
} from '@micra-pro/shared/ui';
import config from './config';
import { MainMenu } from './pages/menu/MainMenu';
import { Navigate, Route, Router } from '@solidjs/router';
import MainScreen from './pages';
import { ToastRegion, ToastList } from '@micra-pro/shared/ui';
import { ScaleSelectorContextProvider } from '@micra-pro/scale-management/feature';

const AppLayout: ParentComponent = (props) => {
  const keyboardInfo = useKeyboardInfo();
  let keyboardElement!: HTMLDivElement;
  let main!: HTMLDivElement;

  const mainContentTranslate = createMemo(() => {
    const inputPosition = keyboardInfo.inputPosition();
    if (!inputPosition || !keyboardElement || !main) return 0;

    const bottomInput = inputPosition.bottom - main.getBoundingClientRect().top;
    const topKeyboard =
      config.display.resolution.height -
      keyboardElement.getBoundingClientRect().height;

    const overlap = 20 + bottomInput - topKeyboard;

    return overlap > 0 ? -overlap : 0;
  });

  return (
    <>
      <Show when={config.useLostBackendConnectionModal}>
        <BackendConnectionTracker />
      </Show>
      <div class="flex min-h-screen items-center justify-center bg-primary">
        <div class="absolute top-1/2 -translate-y-[450px] text-3xl tracking-wider text-gray-400 dark:text-gray-600">
          MICRA PRO
        </div>
        <div
          class="max-w-screen relative max-h-screen overflow-hidden bg-background text-lg shadow-2xl shadow-gray-400 dark:shadow-gray-800"
          style={{
            width: `${config.display.resolution.width}px`,
            height: `${config.display.resolution.height}px`,
          }}
        >
          <div
            ref={main}
            class="h-full overflow-hidden transition-transform duration-500"
            style={{
              transform: `translateY(${mainContentTranslate()}px)`,
            }}
          >
            <DialogContextProvider>
              <ScaleSelectorContextProvider>
                {props.children}
                <ToastRegion>
                  <ToastList />
                </ToastRegion>
              </ScaleSelectorContextProvider>
            </DialogContextProvider>
          </div>
          <Keyboard
            ref={keyboardElement}
            class="absolute bottom-0 left-0 w-full transition-[transform,opacity] duration-500"
            classList={{
              'translate-y-0 opacity-100': keyboardInfo.isOpen(),
              'translate-y-[100%] opacity-0': !keyboardInfo.isOpen(),
            }}
          />
        </div>
      </div>
    </>
  );
};

export const App = () => {
  return (
    <Router root={AppLayout}>
      <Route path="*" component={() => <Navigate href="/" />} />
      <Route path="/" component={MainScreen} />
      <MainMenu />
    </Router>
  );
};

export default App;
