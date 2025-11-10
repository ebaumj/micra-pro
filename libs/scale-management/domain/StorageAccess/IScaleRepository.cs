using MicraPro.ScaleManagement.Domain.ScaleImplementations.BookooThemisMini;

namespace MicraPro.ScaleManagement.Domain.StorageAccess;

public interface IScaleRepository
{
    Task<ScaleDb?> GetScaleAsync(CancellationToken ct);
    Task AddOrUpdateScaleAsync(ScaleDb scale, CancellationToken ct);
    Task DeleteScaleAsync(CancellationToken ct);
}
