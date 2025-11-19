using System.Text.Json;
using MicraPro.Cleaning.DataDefinition.ValueObjects;
using MicraPro.Cleaning.Domain.StorageAccess;
using MicraPro.Shared.Domain.KeyValueStore;

namespace MicraPro.Cleaning.Infrastructure.StorageAccess;

public class CleaningRepository(IKeyValueStoreProvider keyValueStoreProvider) : ICleaningRepository
{
    private interface IWithDefault<out T>
    {
        public static abstract T Default { get; }
    }

    private record Sequence(CleaningCycle[] Data) : IWithDefault<Sequence>
    {
        public static Sequence Default => new([]);
    }

    private record Interval(TimeSpan Data) : IWithDefault<Interval>
    {
        public static Interval Default => new(TimeSpan.Zero);
    }

    private record LastCleaningDate(DateTime Data) : IWithDefault<LastCleaningDate>
    {
        public static LastCleaningDate Default => new(new DateTime(2000, 1, 1));
    }

    private static readonly string Namespace =
        $"{typeof(CleaningRepository).Namespace!}.{nameof(CleaningRepository)}";

    private readonly IKeyValueStore _store = keyValueStoreProvider.GetKeyValueStore(Namespace);
    private const string SequenceKey = "Sequence";
    private const string IntervalKey = "Interval";
    private const string LastCleaningDateKey = "LastCleaningDate";

    private async Task<T> Get<T>(string key, CancellationToken ct)
        where T : class, IWithDefault<T>
    {
        var value = await _store.TryGetAsync(key, ct);
        if (value is null)
            return T.Default;
        try
        {
            return JsonSerializer.Deserialize<T>(value) ?? T.Default;
        }
        catch
        {
            return T.Default;
        }
    }

    public async Task<CleaningCycle[]> GetCleaningSequenceAsync(CancellationToken ct) =>
        (await Get<Sequence>(SequenceKey, ct)).Data;

    public Task SetCleaningSequenceAsync(CleaningCycle[] sequence, CancellationToken ct) =>
        _store.AddOrUpdateAsync(SequenceKey, JsonSerializer.Serialize(new Sequence(sequence)), ct);

    public async Task<TimeSpan> GetCleaningIntervalAsync(CancellationToken ct) =>
        (await Get<Interval>(IntervalKey, ct)).Data;

    public Task SetCleaningIntervalAsync(TimeSpan interval, CancellationToken ct) =>
        _store.AddOrUpdateAsync(IntervalKey, JsonSerializer.Serialize(new Interval(interval)), ct);

    public async Task<DateTime> GetLastCleaningTimeAsync(CancellationToken ct) =>
        (await Get<LastCleaningDate>(LastCleaningDateKey, ct)).Data;

    public Task SetLastCleaningTimeAsync(DateTime time, CancellationToken ct) =>
        _store.AddOrUpdateAsync(
            LastCleaningDateKey,
            JsonSerializer.Serialize(new LastCleaningDate(time)),
            ct
        );

    public async Task<bool> IsCleaningSetAsync(CancellationToken ct) =>
        await _store.TryGetAsync(SequenceKey, ct) != null;
}
