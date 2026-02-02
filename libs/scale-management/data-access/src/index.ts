import * as scan from './scan';
import * as scales from './scales';

import * as scanDummy from './scan-dummy';
import * as scalesDummy from './scales-dummy';

declare const __USE_DUMMIES__: boolean;
const exportValue = __USE_DUMMIES__
  ? {
      ...scanDummy,
      ...scalesDummy,
    }
  : {
      ...scan,
      ...scales,
    };

export const { createScalesAccessor, createScanAccessor, scanForScales } =
  exportValue;
export { type BluetoothScale } from './scan';
export {} from './scales';
