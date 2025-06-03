import { Component, Show } from 'solid-js';
import { Portal } from 'solid-js/web';
import picturesImport from './generated/pictures-import';

export const SplashScreen: Component<{ show: boolean }> = (props) => (
  <Show when={props.show}>
    <Portal>
      <div class="fixed bottom-0 left-0 right-0 top-0 z-50 flex items-center justify-center bg-background">
        <img
          class="scale-[0.4] animate-pulse"
          src={picturesImport.logo_large}
        />
      </div>
    </Portal>
  </Show>
);

export default SplashScreen;
