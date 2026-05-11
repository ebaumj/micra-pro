import { backupAccess } from '@micra-pro/shared/utils-ts';
import { createContext, ParentComponent, useContext } from 'solid-js';

const BackupContext = createContext<{
  enabled: () => boolean;
  available: () => {
    restore: () => Promise<void>;
    delete: () => Promise<void>;
    timestamp: Date;
  }[];
  backup: () => Promise<void>;
}>();

export const useBackupContext = () => {
  const ctx = useContext(BackupContext);
  if (!ctx) throw new Error("Can't find Backup Context!");
  return ctx;
};

export const BackupContextProvider: ParentComponent<{}> = (props) => {
  const accessor = backupAccess();
  const value = {
    enabled: accessor.useBackups,
    available: () =>
      accessor.available().map((b) => ({
        timestamp: new Date(b.timestamp),
        restore: () => accessor.restoreData(b.directory),
        delete: () => accessor.deleteBackup(b.directory),
      })),
    backup: accessor.backupData,
  };
  return (
    <BackupContext.Provider value={value}>
      {props.children}
    </BackupContext.Provider>
  );
};
