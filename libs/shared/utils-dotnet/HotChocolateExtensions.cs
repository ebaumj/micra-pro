using System.Reactive.Linq;
using HotChocolate.Execution;

namespace MicraPro.Shared.UtilsDotnet;

public static class HotChocolateExtensions
{
    public static ISourceStream<T> ToSourceStream<T>(this IObservable<T> observable)
    {
        return new SourceStreamFromObservable<T>(observable);
    }

    private class SourceStreamFromObservable<T>(IObservable<T> observable) : ISourceStream<T>
    {
        IAsyncEnumerable<T> ISourceStream<T>.ReadEventsAsync()
        {
            return observable.ToAsyncEnumerable();
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        IAsyncEnumerable<object?> ISourceStream.ReadEventsAsync()
        {
            return observable.Select(o => (object?)o).ToAsyncEnumerable();
        }
    }
}
