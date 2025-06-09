import {
  createMutation,
  createQuery,
  Duration,
} from '@micra-pro/shared/utils-ts';
import {
  AvailableAssetsDocument,
  AvailableAssetsPollDocument,
  AvailableAssetsPollQuery,
  AvailableAssetsPollQueryVariables,
  AvailableAssetsSubscription,
  AvailableAssetsSubscriptionVariables,
  CreateAssetDocument,
  CreateAssetMutation,
  CreateAssetMutationVariables,
  IsAssetPollingDocument,
  IsAssetPollingPollDocument,
  IsAssetPollingPollQuery,
  IsAssetPollingPollQueryVariables,
  IsAssetPollingSubscription,
  IsAssetPollingSubscriptionVariables,
  PollAssetDocument,
  PollAssetMutation,
  PollAssetMutationVariables,
  UnfinishedAssetsDocument,
  UnfinishedAssetsPollDocument,
  UnfinishedAssetsPollQuery,
  UnfinishedAssetsPollQueryVariables,
  UnfinishedAssetsSubscription,
  UnfinishedAssetsSubscriptionVariables,
} from './generated/graphql';
import { Accessor, createMemo } from 'solid-js';

export type AssetsAccessor = {
  pollAsset: (id: string) => void;
  assets: Accessor<
    {
      id: string;
      path: string;
    }[]
  >;
  create: () => Promise<{ id: string; uploadPath: string }>;
  unfinished: Accessor<{ id: string; uploadPath: string }[]>;
  isPolling: (id: string) => Accessor<boolean>;
};

export const createAssetAccessor = (pollTime: Duration): AssetsAccessor => {
  const query = createQuery<
    AvailableAssetsPollQuery,
    AvailableAssetsPollQueryVariables
  >(AvailableAssetsPollDocument, () => ({}));
  query.subscribeToMore<
    AvailableAssetsSubscription,
    AvailableAssetsSubscriptionVariables
  >(
    AvailableAssetsDocument,
    () => ({}),
    (newData, _, setData) =>
      setData('availableAssets', newData.availableAssetsChanged),
  );
  const unfinishedQuery = createQuery<
    UnfinishedAssetsPollQuery,
    UnfinishedAssetsPollQueryVariables
  >(UnfinishedAssetsPollDocument, () => ({}));
  unfinishedQuery.subscribeToMore<
    UnfinishedAssetsSubscription,
    UnfinishedAssetsSubscriptionVariables
  >(
    UnfinishedAssetsDocument,
    () => ({}),
    (newData, _, setData) =>
      setData('unfinishedAssets', newData.unfinishedAssetsChanged),
  );
  const createAssetMutation = createMutation<
    CreateAssetMutation,
    CreateAssetMutationVariables
  >(CreateAssetDocument);
  const pollMutation = createMutation<
    PollAssetMutation,
    PollAssetMutationVariables
  >(PollAssetDocument);

  const createPollingSubscription = (id: string) => {
    const pollingQuery = createQuery<
      IsAssetPollingPollQuery,
      IsAssetPollingPollQueryVariables
    >(IsAssetPollingPollDocument, () => ({ id: id }));
    pollingQuery.subscribeToMore<
      IsAssetPollingSubscription,
      IsAssetPollingSubscriptionVariables
    >(
      IsAssetPollingDocument,
      () => ({ id: id }),
      (newData, _, setData) =>
        setData('isAssetPolling', newData.isAssetPolling),
    );
    const isPolling = createMemo(
      () => pollingQuery.resource.latest?.isAssetPolling ?? true,
    );
    return isPolling;
  };

  return {
    assets: () =>
      query.resource.latest?.availableAssets.map((a) => ({
        id: a.id,
        path: a.path,
      })) ?? [],
    create: () =>
      new Promise<{ id: string; uploadPath: string }>((resolve, reject) =>
        createAssetMutation({})
          .then((result) => {
            if (result.createAsset.assetUploadQuery)
              resolve({
                id: result.createAsset.assetUploadQuery.assetId,
                uploadPath: result.createAsset.assetUploadQuery.uploadPath,
              });
            else reject();
          })
          .catch((e) => reject(e)),
      ),
    unfinished: () =>
      unfinishedQuery.resource.latest?.unfinishedAssets.map((a) => ({
        id: a.assetId,
        uploadPath: a.uploadPath,
      })) ?? [],
    pollAsset: (id: string) =>
      pollMutation({ id: id, timeout: pollTime.toISOString() }),
    isPolling: (id: string) => createPollingSubscription(id),
  };
};
