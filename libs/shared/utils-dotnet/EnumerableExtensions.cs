namespace MicraPro.Shared.UtilsDotnet;

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }

    public static async Task<IEnumerable<T1>> SelectAsync<T, T1>(
        this IEnumerable<T> enumeration,
        Func<T, Task<T1>> func
    )
    {
        return await Task.WhenAll(enumeration.Select(func));
    }
}
