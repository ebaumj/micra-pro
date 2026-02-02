import * as accessor from './accessor';

import * as accessorDummy from './accessor-dummy';

declare const __USE_DUMMIES__: boolean;
const exportValue = __USE_DUMMIES__
  ? {
      ...accessorDummy,
    }
  : {
      ...accessor,
    };

export const { createAssetAccessor } = exportValue;
export { type AssetsAccessor } from './accessor';
