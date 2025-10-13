import { getImages } from '../utils/database/database_access';

export default defineEventHandler(async () => {
  const runtimeConfig = useRuntimeConfig();
  const images = await getImages(
    runtimeConfig.secrets.databaseConnectionString,
  );
  return {
    images: images
      .filter((i) => !!i.link)
      .map((i) => ({ ...i, link: i.link! })),
  };
});
