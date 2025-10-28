export const throwInternalServerError = () => {
  throw createError({
    statusCode: 500,
    statusMessage: 'Internal Server Error',
  });
};

export const throwNotFoundError = () => {
  throw createError({
    statusCode: 404,
    statusMessage: 'Not Found',
  });
};
