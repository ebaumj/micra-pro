namespace MicraPro.AssetManagement.Infrastructure;

public class AssetManagementInfrastructureOptions
{
    public static string SectionName { get; } =
        typeof(AssetManagementInfrastructureOptions).Namespace!.Replace('.', ':');
    public string LocalFileServerFolder { get; set; } = string.Empty;
    public string LocalFileServerDomain { get; set; } = string.Empty;
    public string LocalFileServerRelativePath { get; set; } = string.Empty;
    public string RemoteFileServerDomain { get; set; } = string.Empty;
    public string JwtIssuer { get; set; } = string.Empty;
    public string JwtTokenLifeTimeInMinutes { get; set; } = string.Empty;
    public string JwtUploadTokenLifeTimeInMinutes { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}
