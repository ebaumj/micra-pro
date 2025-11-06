using System.Text.Json;
using MicraPro.Machine.Domain.DatabaseAccess;
using MicraPro.Shared.Domain.KeyValueStore;

namespace MicraPro.Machine.Infrastructure.DatabaseAccess;

public class MachineRepository(IKeyValueStoreProvider keyValueStoreProvider) : IMachineRepository
{
    private static readonly string Namespace =
        $"{typeof(MachineRepository).Namespace!}.{nameof(MachineRepository)}";
    private IKeyValueStore Store => keyValueStoreProvider.GetKeyValueStore(Namespace);

    public Task<string?> GetCurrentMachineAsync(CancellationToken ct) =>
        Store.TryGetAsync("CurrentMachine", ct);

    public Task SetCurrentMachineAsync(string entry, CancellationToken ct) =>
        Store.AddOrUpdateAsync("CurrentMachine", entry, ct);

    public Task RemoveCurrentMachineAsync(CancellationToken ct) =>
        Store.DeleteAsync("CurrentMachine", ct);
}
