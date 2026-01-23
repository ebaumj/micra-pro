import { fetchUpdates } from '@micra-pro/recipe-hub/data-access';
import { updateAccess } from '@micra-pro/shared/utils-ts';
import {
  createContext,
  createSignal,
  onMount,
  ParentComponent,
  useContext,
} from 'solid-js';

const UpdateContext = createContext<{
  currentVersion: () => string;
  newVersion: () =>
    | {
        version: string;
        install: () => Promise<void>;
      }
    | undefined;
}>();
const PollTimeMinutes = 5;

export const useUpdateContext = () => {
  const ctx = useContext(UpdateContext);
  if (!ctx) throw new Error("Can't find Update Context!");
  return ctx;
};

export const UpdateContextProvider: ParentComponent<{}> = (props) => {
  const accessor = updateAccess();
  const [newVersion, setNewVersion] = createSignal<
    | {
        version: string;
        install: () => Promise<void>;
      }
    | undefined
  >();
  const poll = () => {
    fetchUpdates()
      .then((updates) => {
        const sorted = updates
          .filter((u) => parseVersion(u.version))
          .sort((a, b) => (newerVersion(b.version, a.version) ? -1 : 1));
        if (sorted.length < 1) {
          setNewVersion(undefined);
          return;
        }
        const newest = sorted[0];
        if (!newerVersion(accessor.currentVersion(), newest.version)) {
          setNewVersion(undefined);
          return;
        }
        setNewVersion({
          version: newest.version,
          install: () => accessor.installUpdate(newest.link, newest.signature),
        });
      })
      .catch();
    setTimeout(poll, PollTimeMinutes * 60000);
  };
  onMount(poll);
  return (
    <UpdateContext.Provider
      value={{
        currentVersion: () => accessor.currentVersion(),
        newVersion: newVersion,
      }}
    >
      {props.children}
    </UpdateContext.Provider>
  );
};

type VersionSortable = {
  major: number;
  minor: number;
  patch: number;
};

const parseVersion = (value: string): VersionSortable | undefined => {
  const components = value.split('.');
  if (components.length !== 3) return undefined;
  const version = {
    major: Number(components[0]),
    minor: Number(components[1]),
    patch: Number(components[2]),
  };
  if (Number.isNaN(version.major)) return undefined;
  if (Number.isNaN(version.minor)) return undefined;
  if (Number.isNaN(version.patch)) return undefined;
  return version;
};

const newerVersion = (current: string, update: string): boolean => {
  const currentVersion = parseVersion(current);
  const updateVersion = parseVersion(update);
  if (!updateVersion || !currentVersion) return false;
  if (updateVersion.major < currentVersion.major) return false;
  if (updateVersion.major > currentVersion.major) return true;
  if (updateVersion.minor < currentVersion.minor) return false;
  if (updateVersion.minor > currentVersion.minor) return true;
  return updateVersion.patch > currentVersion.patch;
};
