import { Accessor, createContext, ParentComponent, useContext } from 'solid-js';
import {
  cleaningAccessor,
  CleaningState,
} from '@micra-pro/cleaning/data-access';
import moment from 'moment';

const CleaningContext = createContext<{
  start: () => Promise<void>;
  stop: () => Promise<void>;
  setReminder: (reminder: boolean) => void;
  setInterval: (interval: moment.Duration) => Promise<void>;
  state: Accessor<CleaningState>;
  isRunning: Accessor<boolean>;
  reminder: Accessor<boolean>;
  lastDate: Accessor<Date>;
  interval: Accessor<moment.Duration>;
}>();

export const useCleaningContext = () => {
  const ctx = useContext(CleaningContext);
  if (!ctx) throw new Error('Cleaning Context not found!');
  return ctx;
};

export const CleaningContextProvider: ParentComponent = (props) => {
  const accessor = cleaningAccessor();
  return (
    <CleaningContext.Provider
      value={{
        start: accessor.start,
        stop: accessor.stop,
        setReminder: accessor.setDoRemind,
        setInterval: (interval: moment.Duration) =>
          accessor.setInterval(interval.toISOString()),
        state: accessor.state,
        isRunning: accessor.isCleaning,
        reminder: accessor.doRemind,
        lastDate: accessor.lastCleaningDate,
        interval: () => moment.duration(accessor.interval()),
      }}
    >
      {props.children}
    </CleaningContext.Provider>
  );
};
