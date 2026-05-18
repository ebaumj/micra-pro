import * as brewByWeight from './brew-by-weight';
import * as brewByTime from './brew-by-time';
import * as history from './history';

import * as brewByWeightDummy from './brew-by-weight-dummy';
import * as brewByTimeDummy from './brew-by-time-dummy';
import * as historyDummy from './history-dummy';

declare const __USE_DUMMIES__: boolean;
const exportValue = __USE_DUMMIES__
  ? {
      ...brewByWeightDummy,
      ...brewByTimeDummy,
      ...historyDummy,
    }
  : {
      ...brewByWeight,
      ...brewByTime,
      ...history,
    };

export const { StartCoffee, StartCoffeeByTime, createHistoryAccessor } =
  exportValue;
export { Spout } from './brew-by-weight';
export {
  type BrewByWeightHistoryEntry,
  type HistoryEntryProcessFinished,
  type HistoryEntryProcessCancelled,
  type HistoryEntryProcessFailed,
} from './history';
