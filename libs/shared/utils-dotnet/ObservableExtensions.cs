using System.Reactive.Linq;

namespace MicraPro.Shared.UtilsDotnet;

public static class ObservableExtensions
{
    public static IObservable<T> NotNull<T>(this IObservable<T?> source) =>
        source.Where(v => v != null)!;
}
