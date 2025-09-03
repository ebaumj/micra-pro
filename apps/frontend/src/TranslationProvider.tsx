import { ParentComponent } from 'solid-js';
import { TranslationProvider as LibTranslationProvider } from '@micra-pro/shared/utils-ts';
import { GetRootTranslationConfig } from './generated/language-types';
import { GetScaleManagementTranslationConfig } from '@micra-pro/scale-management/feature';
import { GetBeanManagementTranslationConfig } from '@micra-pro/bean-management/feature';
import { GetBrewByWeightTranslationConfig } from '@micra-pro/brew-by-weight/feature';
import { GetRecipeHubTranslationConfig } from '@micra-pro/recipe-hub/feature';

export const TranslationProvider: ParentComponent = (props) => {
  const resources = [
    GetRootTranslationConfig(),
    GetScaleManagementTranslationConfig(),
    GetBeanManagementTranslationConfig(),
    GetBrewByWeightTranslationConfig(),
    GetRecipeHubTranslationConfig(),
  ];

  return (
    <LibTranslationProvider language="en" resources={resources}>
      {props.children}
    </LibTranslationProvider>
  );
};
