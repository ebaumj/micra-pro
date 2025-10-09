/* @refresh reload */
import { render } from 'solid-js/web';
import './index.css';
import config from './config';
import { GraphQlProvider } from '@micra-pro/shared/utils-ts';
import App from './App';
import { TranslationProvider } from './TranslationProvider';
import { KeyboardProvider } from '@micra-pro/shared/ui';
import { AssetContextProvider } from '@micra-pro/asset-management/feature';
import { RecipeHubClientProvider } from '@micra-pro/recipe-hub/client';

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
                <App />
              </AssetContextProvider>
            </KeyboardProvider>
          </TranslationProvider>
        </RecipeHubClientProvider>
      </GraphQlProvider>
    </>
  ),
  root,
);
