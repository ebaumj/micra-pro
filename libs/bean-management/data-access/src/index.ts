import * as roasteriesAccessor from './roasteriesAccessor';
import * as beansAccessor from './beansAccessor';
import * as recipesAccessor from './recipesAccessor';
import * as dataFetcher from './dataFetcher';
import * as grinderSettings from './grinderSettings';

import * as roasteriesAccessorDummy from './roasteriesAccessor-dummy';
import * as beansAccessorDummy from './beansAccessor-dummy';
import * as recipesAccessorDummy from './recipesAccessor-dummy';
import * as dataFetcherDummy from './dataFetcher-dummy';
import * as grinderSettingsDummy from './grinderSettings-dummy';

declare const __USE_DUMMIES__: boolean;
const exportValue = __USE_DUMMIES__
  ? {
      ...roasteriesAccessorDummy,
      ...beansAccessorDummy,
      ...recipesAccessorDummy,
      ...dataFetcherDummy,
      ...grinderSettingsDummy,
    }
  : {
      ...roasteriesAccessor,
      ...beansAccessor,
      ...recipesAccessor,
      ...dataFetcher,
      ...grinderSettings,
    };

export const {
  createBeansAccessor,
  createRecipesAccessor,
  createRoasteriesAccessor,
  fetchBeansLevel,
  fetchRoasteriesLevel,
  getGrinderSettingsAccessor,
} = exportValue;
export { type RoasteryProperties } from './roasteriesAccessor';
export { type BeanProperties } from './beansAccessor';
export { type EspressoProperties, type V60Properties } from './recipesAccessor';
export { type Roastery, type Bean } from './dataFetcher';
export {} from './grinderSettings';
