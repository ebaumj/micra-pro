using MicraPro.ScaleManagement.DataDefinition.ValueObjects;

namespace MicraPro.ScaleManagement.DataDefinition;

public interface IScaleService
{
    IObservable<bool> IsScanning { get; }
    IObservable<BluetoothScale> DetectedScales { get; }
    Task ScanAsync(TimeSpan scanTime, CancellationToken ct);
    Task<IScale> AddOrUpdateScaleAsync(string identifier, CancellationToken ct);
    Task RemoveScaleAsync(CancellationToken ct);
    Task<IScale?> GetScaleAsync(CancellationToken ct);
}
