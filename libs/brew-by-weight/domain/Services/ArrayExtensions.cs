namespace MicraPro.BrewByWeight.Domain.Services;

internal static class ArrayExtensions
{
    public static T[] WhereOrAll<T>(this T[] array, Func<T, bool> predicate)
    {
        var filtered = array.Where(predicate).ToArray();
        return filtered.Length != 0 ? filtered : array;
    }

    public static TSource MaxByOrFirst<TSource, TKey>(
        this TSource[] array,
        Func<TSource, TKey> keySelector
    ) => array.MaxBy(keySelector) ?? array.First();
}
