using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.ScaleManagement.Domain.ScaleImplementations;
using MicraPro.ScaleManagement.Domain.StorageAccess;
using Microsoft.Extensions.Caching.Memory;

namespace MicraPro.ScaleManagement.Domain.Services;

public class ScaleService(
    IBluetoothService bluetoothService,
    IScaleRespository scaleRepository,
    IScaleImplementationCollectionService scaleImplementationCollectionService,
    IMemoryCache cache
) : IScaleService
{
    private static string GetScaleImplementationCacheKey(string identifier) =>
        $"{typeof(ScaleService).FullName}.ScaleImplementations.{identifier}";

    private async Task<IEnumerable<string>> Scan(
        string implementation,
        Guid[] requiredServices,
        CancellationToken ct
    )
    {
        var allDevices = await bluetoothService.ScanDevicesAsync(requiredServices, ct);
        foreach (var device in allDevices)
            cache.Set(GetScaleImplementationCacheKey(device), implementation);
        return allDevices.Select(d => new string(d));
    }

    public async Task<IEnumerable<string>> Scan(CancellationToken ct)
    {
        var knownDevices = await scaleRepository.GetAllAsync(ct);
        return (
            await Task.WhenAll(
                scaleImplementationCollectionService.Implementations.Select(i =>
                    Scan(i.Name, i.RequiredServices, ct)
                )
            )
        )
            .SelectMany(s => s)
            .Where(d => knownDevices.FirstOrDefault(e => e.Identifier == d) == null);
    }

    private async Task<string> FindScaleImplementation(string identifier, CancellationToken ct)
    {
        if (cache.TryGetValue(identifier, out string? implementation))
            return implementation!;
        foreach (var i in scaleImplementationCollectionService.Implementations)
            if (
                await bluetoothService.HasRequiredServiceIdsAsync(
                    identifier,
                    i.RequiredServices,
                    ct
                )
            )
                return i.Name;
        throw new Exception("Could not find scale implementation");
    }

    public async Task<IScale> AddScale(string name, string identifier, CancellationToken ct)
    {
        var valueObject = new ScaleDb(
            identifier,
            name,
            await FindScaleImplementation(identifier, ct)
        );
        await scaleRepository.AddAsync(valueObject, ct);
        await scaleRepository.SaveAsync(ct);
        return scaleImplementationCollectionService.CreateScale(valueObject);
    }

    public async Task<Guid> RemoveScale(Guid scaleId, CancellationToken ct)
    {
        await scaleRepository.DeleteAsync(scaleId, ct);
        await scaleRepository.SaveAsync(ct);
        return scaleId;
    }

    public async Task<IEnumerable<IScale>> GetScales(CancellationToken ct) =>
        (await scaleRepository.GetAllAsync(ct)).Select(
            scaleImplementationCollectionService.CreateScale
        );

    public async Task<IScale> GetScale(Guid scaleId, CancellationToken ct) =>
        scaleImplementationCollectionService.CreateScale(
            await scaleRepository.GetByIdAsync(scaleId, ct)
        );

    public async Task<IScale> RenameScale(Guid scaleId, string name, CancellationToken ct) =>
        scaleImplementationCollectionService.CreateScale(
            await scaleRepository.UpdateName(scaleId, name, ct)
        );
}
