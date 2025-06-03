import { createContext, ParentComponent, useContext } from 'solid-js';
import {
  createAssetAccessor,
  type AssetsAccessor,
} from '@micra-pro/asset-management/data-access';
import moment from 'moment';

const AssetContext = createContext<{ accessor: AssetsAccessor }>();
const PollTimeMinutes = 10;

export const useAssetAccessor = () => {
  const ctx = useContext(AssetContext);
  if (!ctx) throw new Error('Can^t find Asset Context!');
  return ctx.accessor;
};

export const AssetContextProvider: ParentComponent<{}> = (props) => (
  <AssetContext.Provider
    value={{
      accessor: createAssetAccessor(
        moment.duration(PollTimeMinutes, 'minutes'),
      ),
    }}
  >
    {props.children}
  </AssetContext.Provider>
);
