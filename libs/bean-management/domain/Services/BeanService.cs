using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.Domain.StorageAccess;
using MicraPro.BeanManagement.Domain.ValueObjects;

namespace MicraPro.BeanManagement.Domain.Services;

public class BeanService(IBeanRepository beanRepository) : IBeanService
{
    public async Task<IBean> AddBeanAsync(
        BeanProperties properties,
        Guid roasteryId,
        CancellationToken ct
    )
    {
        var entity = new BeanDb(
            properties.Name,
            roasteryId,
            properties.CountryCode,
            properties.AssetId ?? Guid.Empty
        );
        await beanRepository.AddAsync(entity, ct);
        await beanRepository.SaveAsync(ct);
        return new Bean(entity);
    }

    public async Task<IEnumerable<IBean>> GetBeansAsync(CancellationToken ct) =>
        (await beanRepository.GetAllAsync(ct)).Select(entity => new Bean(entity));

    public async Task<IBean> GetBeanAsync(Guid beanId, CancellationToken ct) =>
        new Bean(await beanRepository.GetByIdAsync(beanId, ct));

    public async Task<IBean> UpdateBeanAsync(
        Guid beanId,
        BeanProperties properties,
        CancellationToken ct
    ) =>
        new Bean(
            await beanRepository.UpdateAsync(
                beanId,
                properties.Name,
                properties.CountryCode,
                properties.AssetId ?? Guid.Empty,
                ct
            )
        );

    public async Task<Guid> RemoveBeanAsync(Guid beanId, CancellationToken ct)
    {
        await beanRepository.DeleteAsync(beanId, ct);
        await beanRepository.SaveAsync(ct);
        return beanId;
    }
}
