namespace MicraPro.FlowProfiling.Infrastructure;

public class FlowProfilingInfrastructureOptions
{
    public static string SectionName { get; } =
        typeof(FlowProfilingInfrastructureOptions).Namespace!.Replace('.', ':');
    public bool IsAvailable { get; set; } = false;
}
