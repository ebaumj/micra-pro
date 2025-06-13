using System.Reactive.Linq;
using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.DataDefinition.ValueObjects;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.ScaleManagement.Domain.ScaleImplementations;
using MicraPro.ScaleManagement.Domain.StorageAccess;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.ScaleManagement.Domain.Services;

public class ScaleService(
    IBluetoothService bluetoothService,
    IScaleRespository scaleRepository,
    IScaleImplementationCollectionService scaleImplementationCollectionService,
    ScaleImplementationMemoryService scaleImplementationMemoryService
) : IScaleService
{
    public IObservable<BluetoothScale> DetectedScales =>
        Observable
            .FromAsync(async ct => await scaleRepository.GetAllAsync(ct))
            .SelectMany(known =>
                bluetoothService
                    .DetectedDevices.Where(d =>
                        scaleImplementationCollectionService.Implementations.Any(i =>
                        {
                            if (
                                !i
                                    .RequiredServices.Select(s => s.ToLower())
                                    .All(s => d.ServiceIds.Select(id => id.ToLower()).Contains(s))
                            )
                                return false;
                            scaleImplementationMemoryService.SetImplementation(d.Id, i.Name);
                            return true;
                        })
                    )
                    .Where(d => known.FirstOrDefault(k => k.Identifier == d.Id) == null)
                    .Select(d => new BluetoothScale(d.Name, d.Id))
            );

    public Task ScanAsync(TimeSpan scanTime, CancellationToken ct) =>
        bluetoothService.DiscoverAsync(scanTime, ct);

    public IObservable<bool> IsScanning => bluetoothService.IsScanning;

    public async Task<IScale> AddScale(string name, string identifier, CancellationToken ct)
    {
        var valueObject = new ScaleDb(
            identifier,
            name,
            scaleImplementationMemoryService.GetImplementation(identifier)
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
