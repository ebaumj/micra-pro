import { createMutation, createQuery } from '@micra-pro/shared/utils-ts';
import { Accessor, createEffect, createSignal } from 'solid-js';
import {
  GrinderOffsetDocument,
  GrinderOffsetQuery,
  GrinderOffsetQueryVariables,
  SetGrinderOffsetDocument,
  SetGrinderOffsetMutation,
  SetGrinderOffsetMutationVariables,
} from './generated/graphql';

type Setting<T> = {
  value: Accessor<T>;
  setValue: (newValue: T) => void;
};

export const getGrinderSettingsAccessor = (): {
  offset: Setting<number>;
} => {
  const [value, setValue] = createSignal(0);
  const offsetQuery = createQuery<
    GrinderOffsetQuery,
    GrinderOffsetQueryVariables
  >(GrinderOffsetDocument, () => ({}));
  createEffect(() => {
    const queryValue = offsetQuery.resource.latest;
    if (queryValue) setValue(queryValue.grinderOffset);
  });
  const setOffset = createMutation<
    SetGrinderOffsetMutation,
    SetGrinderOffsetMutationVariables
  >(SetGrinderOffsetDocument);
  return {
    offset: {
      value: value,
      setValue: (newValue: number) =>
        newValue !== value() &&
        setOffset({ grinderOffset: newValue }).then((o) =>
          setValue((v) => o.setGrinderOffset.float ?? v),
        ),
    },
  };
};
