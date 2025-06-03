namespace MicraPro.Auth.Domain;

public class AuthDomainRuntimeOptions
{
    public string Audience { get; set; } = string.Empty;
    public byte[] PrivateKey { get; set; } = [];
}
