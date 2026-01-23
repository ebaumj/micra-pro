/* @refresh reload */
import { render } from 'solid-js/web';
import './index.css';
import config from './config';
import { GraphQlProvider } from '@micra-pro/shared/utils-ts';
import App from './App';
import { TranslationProvider } from './TranslationProvider';
import { DarkModeProvider, KeyboardProvider } from '@micra-pro/shared/ui';
import { AssetContextProvider } from '@micra-pro/asset-management/feature';
import { RecipeHubClientProvider } from '@micra-pro/recipe-hub/client';
import { UpdateContextProvider } from './components/UpdateContextProvider';
import { WifiContextProvider } from './components/WifiContextProvider';

const root = document.getElementById('root');

if (!root) throw new Error('Root element not found');

render(
  () => (
    <>
      <GraphQlProvider
        url={config.graphql.httpUri}
        wsUrl={config.graphql.wsUri}
      >
        <RecipeHubClientProvider url={config.recipeHub.uri}>
          <TranslationProvider>
            <KeyboardProvider>
              <AssetContextProvider>
                <DarkModeProvider>
                  <WifiContextProvider>
                    <UpdateContextProvider>
                      <App />
                    </UpdateContextProvider>
                  </WifiContextProvider>
                </DarkModeProvider>
              </AssetContextProvider>
            </KeyboardProvider>
          </TranslationProvider>
        </RecipeHubClientProvider>
      </GraphQlProvider>
    </>
  ),
  root,
);
