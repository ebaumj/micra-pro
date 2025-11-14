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
import { SpoutSelectorContextProvider } from '@micra-pro/brew-by-weight/feature';
import { CleaningContextProvider } from '@micra-pro/cleaning/feature';

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
      <div class="flex min-h-screen items-center justify-center bg-black dark:bg-white">
        <div class="absolute top-1/2 -translate-y-80 text-3xl tracking-wider text-gray-500">
          MICRA PRO
        </div>
        <div
          class="bg-background text-foreground relative max-h-screen max-w-screen overflow-hidden text-lg shadow-2xl shadow-gray-500"
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
              <Show when={config.useLostBackendConnectionModal}>
                <BackendConnectionTracker />
              </Show>
              <CleaningContextProvider>
                <SpoutSelectorContextProvider>
                  {props.children}
                  <ToastRegion>
                    <ToastList />
                  </ToastRegion>
                </SpoutSelectorContextProvider>
              </CleaningContextProvider>
            </DialogContextProvider>
          </div>
          <Keyboard
            ref={keyboardElement}
            class="absolute bottom-0 left-0 w-full transition-[transform,opacity] duration-500"
            classList={{
              'translate-y-0 opacity-100': keyboardInfo.isOpen(),
              'translate-y-full opacity-0': !keyboardInfo.isOpen(),
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
