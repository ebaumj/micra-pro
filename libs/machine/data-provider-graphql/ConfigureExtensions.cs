using HotChocolate.Execution.Configuration;
using MicraPro.Machine.DataProviderGraphQl.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.Machine.DataProviderGraphQl;

public static class ConfigureExtensions
{
    public static IRequestExecutorBuilder AddMachineDataProviderGraphQlTypes(
        this IRequestExecutorBuilder builder
    ) => builder.AddDataProviderGraphQlTypes();

    public static IServiceCollection AddMachineDataProviderGraphQlServices(
        this IServiceCollection services,
        IConfiguration configurationManager
    ) => services.AddSingleton<MachineScanContainerService>();
}
