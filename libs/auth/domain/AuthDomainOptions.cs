namespace MicraPro.Auth.Domain;

public class AuthDomainOptions
{
    public static string SectionName { get; } =
        typeof(AuthDomainOptions).Namespace!.Replace('.', ':');
    public string JwtIssuer { get; set; } = string.Empty;
    public string[] JwtValidIssuers { get; set; } = [];
    public string JwtTokenLifeTimeInMinutes { get; set; } = "0";
    public string JwtRefreshTokenLifeTimeInMinutes { get; set; } = "0";
    public string Audience { get; set; } = string.Empty;
    public string PrivateKey { get; set; } = string.Empty;
}
