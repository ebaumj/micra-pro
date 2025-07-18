using HotChocolate.Execution.Configuration;
using MicraPro.AssetManagement.DataProviderGraphQl;
using MicraPro.Auth.DataProvider;
using MicraPro.BeanManagement.DataProviderGraphQl;
using MicraPro.BrewByWeight.DataProviderGraphQl;
using MicraPro.ScaleManagement.DataProviderGraphQl;
using MicraPro.Shared.DataProviderGraphQl;

namespace MicraPro.Backend;

internal static class ConfigureGraphQlExtensions
{
    public static IServiceCollection AddGraphQlServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddSharedGraphQlServer(
                configuration
                    .GetSection(BackendOptions.SectionName)
                    .Get<BackendOptions>()
                    ?.IncludeExceptionDetails ?? false
            )
            .AddErrorFilter<ErrorLogger>()
            .AddAuthDataProviderServices()
            .AddMicraProTypes();
        return services;
    }

    private static IRequestExecutorBuilder AddMicraProTypes(this IRequestExecutorBuilder builder)
    {
        return builder
            .AddSharedDataProviderGraphQlTypes()
            .AddScaleManagementDataProviderGraphQlTypes()
            .AddBeanManagementDataProviderGraphQlTypes()
            .AddAssetManagementDataProviderGraphQlTypes()
            .AddBrewByWeightDataProviderGraphQlTypes();
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class ErrorLogger(ILogger<ErrorLogger> logger) : IErrorFilter
    {
        public IError OnError(IError error)
        {
            if (error.Exception is not null)
                logger.LogError("Uncaught server exception: {exception}", error.Exception);
            else if (error.Code != "AUTH_NOT_AUTHORIZED")
                logger.LogError("Unknown server error: {message}", error.Message);
            return error;
        }
    }
}
