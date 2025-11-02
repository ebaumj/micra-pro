using System.Net;
using System.Net.Sockets;
using MicraPro.AssetManagement.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;

namespace MicraPro.AssetManagement.Infrastructure.Services;

public class AssetServerDomainProvider(IOptions<AssetManagementInfrastructureOptions> options)
    : IAssetServerDomainProvider
{
    public string AssetServerExternDomain =>
        options.Value.RemoteFileServerDomain
        ?? $"http://{Dns.GetHostEntry(Dns.GetHostName())
            .AddressList
            .FirstOrDefault(a => 
                a.AddressFamily == AddressFamily.InterNetwork && a.ToString().StartsWith("192.168.")
                ) ?? throw new Exception("Local IP address not found!")}";

    public string AssetServerLocalDomain =>
        options.Value.RemoteFileServerDomain ?? "http://localhost:80";
}
