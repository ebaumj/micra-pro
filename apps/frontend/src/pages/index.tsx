import {
  ScaleSelector,
  ScaleSelectorContextProvider,
} from '@micra-pro/scale-management/feature';
import { Icon } from '@micra-pro/shared/ui';
import { A } from '@solidjs/router';
import { ParentComponent } from 'solid-js';
import { T } from '../generated/language-types';
import { LanguageSelector } from '@micra-pro/shared/utils-ts';

function MainScreen() {
  return (
    <Layout>
      <div class="h-full w-full p-4">Main</div>
    </Layout>
  );
}

const Layout: ParentComponent = (props) => {
  return (
    <ScaleSelectorContextProvider>
      <div class="relative h-full w-full">
        <A href="/menu" class="absolute z-10 ml-3 mt-1 active:opacity-50">
          <Icon iconName="menu" class="text-5xl" />
        </A>
        <div class="absolute flex h-full w-full flex-col">
          <div class="flex h-16 w-full items-center gap-2 pl-20 pr-2 shadow-md">
            <div class="w-full" />
            <div class="text-sm">
              <T key="scale" />:
            </div>
            <ScaleSelector class="w-64" />
            <LanguageSelector class="w-40" />
          </div>
          <div class="h-full w-full">{props.children}</div>
        </div>
      </div>
    </ScaleSelectorContextProvider>
  );
};

export default MainScreen;
