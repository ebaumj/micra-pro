import * as cleaningParameters from './cleaning-parameters';
import * as cleaning from './cleaning';

import * as cleaningParametersDummy from './cleaning-parameters-dummy';
import * as cleaningDummy from './cleaning-dummy';

declare const __USE_DUMMIES__: boolean;
const exportValue = __USE_DUMMIES__
  ? {
      ...cleaningParametersDummy,
      ...cleaningDummy,
    }
  : {
      ...cleaningParameters,
      ...cleaning,
    };

export const { cleaningAccessor, cleaningParametersAccessor } = exportValue;
export { type CleaningCycle } from './cleaning-parameters';
export { type CleaningState } from './cleaning';
