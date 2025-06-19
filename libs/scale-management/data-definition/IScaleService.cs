using MicraPro.ScaleManagement.DataDefinition.ValueObjects;

namespace MicraPro.ScaleManagement.DataDefinition;

public interface IScaleService
{
    IObservable<bool> IsScanning { get; }
    public IObservable<BluetoothScale> DetectedScales { get; }
    Task ScanAsync(TimeSpan scanTime, CancellationToken ct);
    Task<IScale> AddScaleAsync(string name, string identifier, CancellationToken ct);
    Task<Guid> RemoveScaleAsync(Guid scaleId, CancellationToken ct);
    Task<IEnumerable<IScale>> GetScalesAsync(CancellationToken ct);
    Task<IScale> GetScaleAsync(Guid scaleId, CancellationToken ct);
    Task<IScale> RenameScaleAsync(Guid scaleId, string name, CancellationToken ct);
}
