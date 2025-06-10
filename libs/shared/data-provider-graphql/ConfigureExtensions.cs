using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.Shared.DataProviderGraphQl;

public static class ConfigureExtensions
{
    public static IRequestExecutorBuilder AddSharedGraphQlServer(
        this IServiceCollection services,
        bool includeExceptionDetails,
        TimeSpan executionTimeout
    ) =>
        services
            .AddGraphQLServer(disableCostAnalyzer: true)
            .ModifyRequestOptions(o =>
            {
                o.IncludeExceptionDetails = includeExceptionDetails;
                o.ExecutionTimeout = executionTimeout;
            })
            .AddInMemorySubscriptions()
            .AddMutationConventions()
            .AddQueryConventions()
            .BindRuntimeType<uint, UnsignedIntType>();

    public static IRequestExecutorBuilder AddSharedDataProviderGraphQlTypes(
        this IRequestExecutorBuilder builder
    )
    {
        builder = builder
            .AddTypeExtension(typeof(ConfigurationMutations))
            .AddTypeExtension(typeof(ConfigurationQueries))
            .AddTypeExtension(typeof(ConnectionTestQueries));
        builder.ConfigureSchema(b =>
            b.TryAddRootType(
                () => new ObjectType(d => d.Name(OperationTypeNames.Query)),
                HotChocolate.Language.OperationType.Query
            )
        );
        builder.ConfigureSchema(b =>
            b.TryAddRootType(
                () => new ObjectType(d => d.Name(OperationTypeNames.Mutation)),
                HotChocolate.Language.OperationType.Mutation
            )
        );
        return builder;
    }
}
