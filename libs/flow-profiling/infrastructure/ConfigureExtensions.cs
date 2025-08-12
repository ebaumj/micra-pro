using MicraPro.FlowProfiling.Domain.HardwareAccess;
using MicraPro.FlowProfiling.Infrastructure.HardwareAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.FlowProfiling.Infrastructure;

public static class ConfigureExtensions
{
    public static IServiceCollection AddFlowProfilingInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services
            .Configure<FlowProfilingInfrastructureOptions>(
                configuration.GetSection(FlowProfilingInfrastructureOptions.SectionName)
            )
            .AddSingleton<DummyFlowRegulator>()
            .AddSingleton<IFlowRegulator>(sp => sp.GetRequiredService<DummyFlowRegulator>())
            .AddSingleton<IFlowPublisher>(sp => sp.GetRequiredService<DummyFlowRegulator>());
    }
}
