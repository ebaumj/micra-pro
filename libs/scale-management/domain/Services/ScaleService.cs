using System.Reactive.Linq;
using System.Text.Json;
using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.DataDefinition.ValueObjects;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.ScaleManagement.Domain.ScaleImplementations;
using MicraPro.ScaleManagement.Domain.StorageAccess;

namespace MicraPro.ScaleManagement.Domain.Services;

public class ScaleService(
    IBluetoothService bluetoothService,
    IScaleRepository scaleRepository,
    IScaleImplementationCollectionService scaleImplementationCollectionService,
    ScaleImplementationMemoryService scaleImplementationMemoryService
) : IScaleService
{
    public IObservable<BluetoothScale> DetectedScales =>
        Observable
            .FromAsync(scaleRepository.GetScaleAsync)
            .SelectMany(known =>
                bluetoothService
                    .DetectedDevices.Where(d =>
                        scaleImplementationCollectionService.Implementations.Any(i =>
                        {
                            if (!i.Filter(d))
                                return false;
                            scaleImplementationMemoryService.SetImplementation(d.Id, i.Name);
                            return true;
                        })
                    )
                    .Where(d => d.Id != known?.Identifier)
                    .Select(d => new BluetoothScale(d.Name, d.Id))
            );

    public Task ScanAsync(TimeSpan scanTime, CancellationToken ct) =>
        bluetoothService.DiscoverAsync(scanTime, ct);

    public IObservable<bool> IsScanning => bluetoothService.IsScanning;

    public async Task<IScale> AddOrUpdateScaleAsync(string identifier, CancellationToken ct)
    {
        var valueObject = new ScaleDb(
            identifier,
            scaleImplementationMemoryService.GetImplementation(identifier)
        );
        await scaleRepository.AddOrUpdateScaleAsync(valueObject, ct);
        return scaleImplementationCollectionService.CreateScale(valueObject);
    }

    public Task RemoveScaleAsync(CancellationToken ct) => scaleRepository.DeleteScaleAsync(ct);

    public async Task<IScale?> GetScaleAsync(CancellationToken ct)
    {
        var value = await scaleRepository.GetScaleAsync(ct);
        return value == null ? null : scaleImplementationCollectionService.CreateScale(value);
    }
}
