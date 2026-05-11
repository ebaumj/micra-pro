using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace MicraPro.Shared.Infrastructure;

public class BackupConfig(IOptions<SharedInfrastructureOptions> options) : IBackupConfig
{
    private IBackupConfig.BackupConfig? _config;

    private static bool TryReadConfig(string path, [NotNullWhen(true)] out BackupConfigData? config)
    {
        config = null;
        try
        {
            if (!File.Exists(path))
                return false;
            config = JsonSerializer.Deserialize<BackupConfigData>(File.ReadAllText(path));
            return config != null;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private record BackupConfigData(
        // ReSharper disable once InconsistentNaming
        string server,
        // ReSharper disable once InconsistentNaming
        string directory,
        // ReSharper disable once InconsistentNaming
        string username,
        // ReSharper disable once InconsistentNaming
        string password
    )
    {
        public IBackupConfig.BackupConfig Read() => new(server, directory, username, password);
    }

    public IBackupConfig.BackupConfig? Config
    {
        get
        {
            if (_config != null)
                return _config;
            if (TryReadConfig(options.Value.BackupConfigFile, out var config))
                _config = config.Read();
            return _config;
        }
    }
}
