import { createMutation, createQuery } from '@micra-pro/shared/utils-ts';
import {
  AvailableAssetsDocument,
  AvailableAssetsQuery,
  AvailableAssetsQueryVariables,
  CreateAssetDocument,
  CreateAssetMutation,
  CreateAssetMutationVariables,
  RemoveAssetDocument,
  RemoveAssetMutation,
  RemoveAssetMutationVariables,
  SyncAssetsDocument,
  SyncAssetsMutation,
  SyncAssetsMutationVariables,
  UnfinishedAssetsDocument,
  UnfinishedAssetsQuery,
  UnfinishedAssetsQueryVariables,
} from './generated/graphql';
import { Accessor, createSignal } from 'solid-js';

export type AssetsAccessor = {
  assets: () => {
    id: string;
    path: string;
    remove: () => void;
    isRemoving: Accessor<boolean>;
  }[];
  create: () => Promise<{ id: string; uploadPath: string }>;
  refetch: () => Promise<boolean>;
  unfinished: () => { id: string; uploadPath: string }[];
};

export const createAssetAccessor = (): AssetsAccessor => {
  const [removing, setRemoving] = createSignal('');
  const query = createQuery<
    AvailableAssetsQuery,
    AvailableAssetsQueryVariables
  >(AvailableAssetsDocument, () => ({}));
  const unfinishedQuery = createQuery<
    UnfinishedAssetsQuery,
    UnfinishedAssetsQueryVariables
  >(UnfinishedAssetsDocument, () => ({}));
  const removeMutation = createMutation<
    RemoveAssetMutation,
    RemoveAssetMutationVariables
  >(RemoveAssetDocument);
  const createAssetMutation = createMutation<
    CreateAssetMutation,
    CreateAssetMutationVariables
  >(CreateAssetDocument);
  const syncMutation = createMutation<
    SyncAssetsMutation,
    SyncAssetsMutationVariables
  >(SyncAssetsDocument);
  const remove = (id: string) => {
    setRemoving(id);
    removeMutation({ assetId: id })
      .then((result) => {
        if (result.removeAsset.uuid)
          query.setDataStore('availableAssets', (assets) =>
            assets.filter((a) => a.id !== id),
          );
      })
      .finally(() => setRemoving(''));
  };

  const refetch = async () => {
    const result = await syncMutation({});
    await query.resourceActions.refetch();
    await unfinishedQuery.resourceActions.refetch();
    return result.syncAssets.boolean ?? false;
  };

  return {
    assets: () =>
      query.resource.latest?.availableAssets.map((a) => ({
        id: a.id,
        path: a.path,
        remove: () => remove(a.id),
        isRemoving: () => removing() === a.id,
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
    refetch: () => refetch(),
    unfinished: () =>
      unfinishedQuery.resource.latest?.unfinishedAssets.map((a) => ({
        id: a.assetId,
        uploadPath: a.uploadPath,
      })) ?? [],
  };
};
