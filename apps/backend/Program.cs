using MicraPro.Backend;
using MicraPro.Shared.Infrastructure;
using Microsoft.Extensions.Hosting.Systemd;

SettingsMigration.MigrateSettings();

var builder = Host.CreateDefaultBuilder(args);
builder.UseSystemd().ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());

if (SystemdHelpers.IsSystemdService())
    builder.ConfigureLogging(b => b.ClearProviders().AddJournal());

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    BackupService.RestoreDatabaseFile(config);
}

host.Run();
