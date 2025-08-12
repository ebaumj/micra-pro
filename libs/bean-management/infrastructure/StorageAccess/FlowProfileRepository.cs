using System.Text.Json;
using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.Domain.StorageAccess;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MicraPro.BeanManagement.Infrastructure.StorageAccess;

public class FlowProfileRepository(MigratedContextProvider<BeanManagementDbContext> contextProvider)
    : BaseSqliteRepository<FlowProfileDb>,
        IFlowProfileRepository
{
    protected override async Task<DbSet<FlowProfileDb>> GetEntitiesAsync(CancellationToken ct) =>
        (await contextProvider.GetContextAsync(ct)).FlowProfileEntries;

    protected override async Task<DbContext> GetContextAsync(CancellationToken ct) =>
        await contextProvider.GetContextAsync(ct);

    public async Task<FlowProfileDb> UpdateAsync(
        Guid profileId,
        double startFlow,
        FlowSetting[] flowSettings,
        CancellationToken ct
    )
    {
        var entity = await GetByIdAsync(profileId, ct);
        entity.StartFlow = startFlow;
        entity.FlowSettings = flowSettings;
        await SaveAsync(ct);
        return entity;
    }

    private static string SerializeSettings(FlowSetting[] flowSettings) =>
        JsonSerializer.Serialize(flowSettings);

    private static FlowSetting[] DeserializeSettings(string json) =>
        JsonSerializer.Deserialize<FlowSetting[]>(json) ?? [];

    public static ValueConverter<FlowSetting[], string> FlowSettingsConverter =>
        new(s => SerializeSettings(s), s => DeserializeSettings(s));
}
