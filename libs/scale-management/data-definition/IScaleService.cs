using MicraPro.ScaleManagement.DataDefinition.ValueObjects;

namespace MicraPro.ScaleManagement.DataDefinition;

public interface IScaleService
{
    IObservable<bool> IsScanning { get; }
    public IObservable<BluetoothScale> DetectedScales { get; }
    Task ScanAsync(TimeSpan scanTime, CancellationToken ct);
    Task<IScale> AddScale(string name, string identifier, CancellationToken ct);
    Task<Guid> RemoveScale(Guid scaleId, CancellationToken ct);
    Task<IEnumerable<IScale>> GetScales(CancellationToken ct);
    Task<IScale> GetScale(Guid scaleId, CancellationToken ct);
    Task<IScale> RenameScale(Guid scaleId, string name, CancellationToken ct);
}
