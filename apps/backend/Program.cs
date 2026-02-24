using MicraPro.Backend;
using Microsoft.Extensions.Hosting.Systemd;

SettingsMigration.MigrateSettings();

var builder = Host.CreateDefaultBuilder(args);
builder.UseSystemd().ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());

if (SystemdHelpers.IsSystemdService())
{
    builder.ConfigureLogging(b => b.ClearProviders().AddJournal());
}

builder.Build().Run();
