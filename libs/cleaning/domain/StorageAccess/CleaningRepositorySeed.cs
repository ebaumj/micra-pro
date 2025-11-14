using MicraPro.Cleaning.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MicraPro.Cleaning.Domain.StorageAccess;

public class CleaningRepositorySeed(
    IServiceScopeFactory serviceScopeFactory,
    ICleaningDefaultsProvider cleaningDefaultsProvider
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var repository = serviceScopeFactory
            .CreateScope()
            .ServiceProvider.GetRequiredService<ICleaningRepository>();
        if (!await repository.IsCleaningSetAsync(cancellationToken))
            await repository.SetCleaningSequenceAsync(
                cleaningDefaultsProvider.DefaultSequence,
                cancellationToken
            );
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
