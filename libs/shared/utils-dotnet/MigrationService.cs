using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MicraPro.Shared.UtilsDotnet;

public class MigrationService<TDbContext> : IHostedService
    where TDbContext : DbContext
{
    private readonly Lazy<Task> _migrationTask;
    private readonly CancellationTokenSource _cts = new();

    public Task MigrateAsync(CancellationToken ct)
    {
        ct.Register(_cts.Cancel);
        return _migrationTask.Value;
    }

    public MigrationService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<MigrationService<TDbContext>> logger
    )
    {
        _migrationTask = new Lazy<Task>(async () =>
        {
            logger.LogInformation("Migration started for {contextName}", typeof(TDbContext).Name);
            using var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            await dbContext.Database.MigrateAsync(_cts.Token);
            logger.LogInformation("Migration completed for {contextName}", typeof(TDbContext).Name);
        });
    }

    // Make sure the migration task is started as soon as possible
    public Task StartAsync(CancellationToken ct) => MigrateAsync(ct);

    public Task StopAsync(CancellationToken _)
    {
        _cts.Cancel();
        return Task.CompletedTask;
    }
}
