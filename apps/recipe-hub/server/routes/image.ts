import { getImages } from '@micra-pro/recipe-hub/database';

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
