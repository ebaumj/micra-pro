using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.Shared.DataProviderGraphQl.Test;

public static class SchemaBaseTests
{
    public static async Task<string> GenerateSchemaTest(Action<IRequestExecutorBuilder> addTypes)
    {
        var collection = new ServiceCollection();
        var builder = collection.AddSharedGraphQlServer(true).AddAuthorization();
        addTypes(builder);

        var executor = collection
            .AddSingleton(sp => new RequestExecutorProxy(
                sp.GetRequiredService<IRequestExecutorResolver>(),
                Schema.DefaultName
            ))
            .BuildServiceProvider()
            .GetRequiredService<RequestExecutorProxy>();
        return (await executor.GetSchemaAsync(CancellationToken.None))
            .ToString()
            .Replace("\\/", "/");
    }
}
