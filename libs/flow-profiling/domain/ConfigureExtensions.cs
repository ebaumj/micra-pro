using MicraPro.FlowProfiling.DataDefinition;
using MicraPro.FlowProfiling.Domain.Interfaces;
using MicraPro.FlowProfiling.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.FlowProfiling.Domain;

public static class ConfigureExtensions
{
    public static IServiceCollection AddFlowProfilingDomainServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services
            .AddSingleton<IFlowRampGeneratorService, FlowRampGeneratorService>()
            .AddSingleton<IFlowProfilingService, FlowProfilingService>();
    }
}
