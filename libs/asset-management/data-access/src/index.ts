import * as accessor from './accessor';
import * as webhooksAccess from './webhooks';

import * as accessorDummy from './accessor-dummy';
import * as WebhooksAccessDummy from './webhooks-dummy';

declare const __USE_DUMMIES__: boolean;
const exportValue = __USE_DUMMIES__
  ? {
      ...accessorDummy,
      ...WebhooksAccessDummy,
    }
  : {
      ...accessor,
      ...webhooksAccess,
    };

export const { createAssetAccessor, getWebhooksAsync, webhooksAvailable } =
  exportValue;
export { type AssetsAccessor } from './accessor';
