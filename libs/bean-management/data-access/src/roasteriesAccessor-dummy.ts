import { RoasteryProperties } from './generated/graphql';
import { Accessor, createEffect, createSignal } from 'solid-js';
import { v4 as uuid } from 'uuid';

export { type RoasteryProperties } from './generated/graphql';

type RoasteryData = {
  id: string;
  properties: RoasteryProperties;
};

type Roastery = RoasteryData & {
  update: (properties: RoasteryProperties, callback?: () => void) => void;
  remove: (callback?: () => void) => void;
  isUpdating: Accessor<boolean>;
  isDeleting: Accessor<boolean>;
};

type RoasteriesAccessor = {
  roasteries: Accessor<Roastery[]>;
  add: (properties: RoasteryProperties, callback?: () => void) => void;
  isLoading: Accessor<boolean>;
};

export const createRoasteriesAccessor = (): RoasteriesAccessor => {
  const [roasteries, setRoasteries] = createSignal<RoasteryData[]>([]);
  const storage = localStorage['RoasteriesTable'];
  if (storage)
    try {
      setRoasteries(JSON.parse(storage).roasteries);
    } catch {
      // Data Format does not match
    }
  createEffect(
    () =>
      (localStorage['RoasteriesTable'] = JSON.stringify({
        roasteries: roasteries(),
      })),
  );

  return {
    roasteries: () =>
      roasteries().map((r) => ({
        id: r.id,
        properties: r.properties,
        update: (properties: RoasteryProperties, callback?: () => void) => {
          setRoasteries((list) =>
            list.map((e) => {
              if (e.id === r.id)
                return {
                  ...e,
                  properties: JSON.parse(JSON.stringify(properties)),
                };
              return e;
            }),
          );
          callback?.();
        },
        remove: (callback?: () => void) => {
          setRoasteries((list) => list.filter((e) => e.id !== r.id));
          callback?.();
        },
        isUpdating: () => false,
        isDeleting: () => false,
      })) ?? [],
    add: (properties: RoasteryProperties, callback?: () => void) => {
      setRoasteries((list) =>
        list.concat([
          { id: uuid(), properties: JSON.parse(JSON.stringify(properties)) },
        ]),
      );
      callback?.();
    },
    isLoading: () => false,
  };
};
