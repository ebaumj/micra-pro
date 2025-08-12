using System.Runtime.InteropServices;
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
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            services
                .AddSingleton<FlowRegulator>()
                .AddSingleton<IFlowRegulator>(sp => sp.GetRequiredService<FlowRegulator>())
                .AddSingleton<IFlowPublisher>(sp => sp.GetRequiredService<FlowRegulator>());
        else
            services
                .AddSingleton<DummyFlowRegulator>()
                .AddSingleton<IFlowRegulator>(sp => sp.GetRequiredService<DummyFlowRegulator>())
                .AddSingleton<IFlowPublisher>(sp => sp.GetRequiredService<DummyFlowRegulator>());
        return services.Configure<FlowProfilingInfrastructureOptions>(
            configuration.GetSection(FlowProfilingInfrastructureOptions.SectionName)
        );
    }
}
