/* @refresh reload */
import { render } from 'solid-js/web';
import './index.css';
import config from './config';
import { GraphQlProvider } from '@micra-pro/shared/utils-ts';
import App from './App';
import { TranslationProvider } from './TranslationProvider';
import { KeyboardProvider } from '@micra-pro/shared/ui';
import { AssetContextProvider } from '@micra-pro/asset-management/feature';

const root = document.getElementById('root');

if (!root) throw new Error('Root element not found');

render(
  () => (
    <>
      {/* TODO: Add token / auth handling. */}
      <GraphQlProvider
        token={config.graphql.foreverAdminToken}
        url={config.graphql.httpUri}
        wsUrl={config.graphql.wsUri}
      >
        <TranslationProvider>
          <KeyboardProvider>
            <AssetContextProvider>
              <App />
            </AssetContextProvider>
          </KeyboardProvider>
        </TranslationProvider>
      </GraphQlProvider>
    </>
  ),
  root,
);
