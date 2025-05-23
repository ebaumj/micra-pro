namespace MicraPro.Backend;

internal class BackendOptions
{
    public static string SectionName { get; } = typeof(BackendOptions).Namespace!.Replace('.', ':');

    public bool IncludeExceptionDetails { get; set; }
}
