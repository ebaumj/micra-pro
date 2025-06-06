const useInternalServerError = () => {
  throw createError({
    statusCode: 500,
    statusMessage: 'Internal Server Error',
  });
};

export default useInternalServerError;
