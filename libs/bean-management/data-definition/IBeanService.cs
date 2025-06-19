using MicraPro.BeanManagement.DataDefinition.ValueObjects;

namespace MicraPro.BeanManagement.DataDefinition;

public interface IBeanService
{
    Task<IBean> AddBeanAsync(BeanProperties properties, Guid roasteryId, CancellationToken ct);
    public Task<IEnumerable<IBean>> GetBeansAsync(CancellationToken ct);
    Task<IBean> UpdateBeanAsync(Guid beanId, BeanProperties properties, CancellationToken ct);
    Task<Guid> RemoveBeanAsync(Guid beanId, CancellationToken ct);
}
