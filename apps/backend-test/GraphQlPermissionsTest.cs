using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Language;
using HotChocolate.Types;
using MicraPro.Auth.DataDefinition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace MicraPro.Backend.Test;

public class GraphQlPermissionsTest
{
    private static string? GetAuthorizeDirectivePolicy(Directive dir) =>
        dir.AsSyntaxNode()
            .Arguments.FirstOrDefault(arg => arg.Name.Value == "policy")
            ?.Value.Value?.ToString();

    private static IEnumerable<ObjectField> FilterUnauthorizedFields(
        FieldCollection<ObjectField> collection
    ) =>
        collection.Where(q =>
            // Has One Authorize Directive
            q.Directives.Count(dir => dir.AsSyntaxNode().Name.Value == "authorize") != 1
            ||
            // Authorize Directive has required Policy
            GetAuthorizeDirectivePolicy(
                q.Directives.FirstOrDefault(dir => dir.AsSyntaxNode().Name.Value == "authorize")!
            )
                is not RequiredPermissionsAttribute.AuthorizePolicyName
            ||
            // A"Required Permission" Policy has at least one Permission
            !(
                GetAuthorizeDirectivePolicy(
                    q.Directives.FirstOrDefault(dir =>
                        dir.AsSyntaxNode().Name.Value == "authorize"
                    )!
                ) is RequiredPermissionsAttribute.AuthorizePolicyName
                && (
                    (ListValueNode)
                        q
                            .Directives.FirstOrDefault(dir =>
                                dir.AsSyntaxNode().Name.Value == "authorize"
                            )!
                            .AsSyntaxNode()
                            .Arguments.FirstOrDefault(arg => arg.Name.Value == "roles")
                            ?.Value!
                )
                    .Items
                    .Count > 0
            )
        );

    [Fact]
    public async Task AllFieldsNeedPermissionsTest()
    {
        var executor = new ServiceCollection()
            .AddScoped<ILogger<ConfigureGraphQlExtensions.ErrorLogger>>(_ =>
                Mock.Of<ILogger<ConfigureGraphQlExtensions.ErrorLogger>>()
            )
            .AddGraphQlServices(ConfigMicraProBackendMock.Create())
            .AddSingleton(sp => new RequestExecutorProxy(
                sp.GetRequiredService<IRequestExecutorResolver>(),
                Schema.DefaultName
            ))
            .BuildServiceProvider()
            .GetRequiredService<RequestExecutorProxy>();
        var schema = await executor.GetSchemaAsync(CancellationToken.None);
        var unauthorizedQueries = FilterUnauthorizedFields(schema.QueryType.Fields)
            .Select(q => q.Name)
            .ToArray();
        Assert.Equal(3, unauthorizedQueries.Length);
        Assert.Contains("__schema", unauthorizedQueries);
        Assert.Contains("__type", unauthorizedQueries);
        Assert.Contains("__typename", unauthorizedQueries);
        if (schema.MutationType != null)
        {
            var unauthorizedMutations = FilterUnauthorizedFields(schema.MutationType.Fields)
                .Select(q => q.Name)
                .ToArray();
            Assert.Single(unauthorizedMutations);
            Assert.Contains("__typename", unauthorizedMutations);
        }
        if (schema.SubscriptionType != null)
        {
            var unauthorizedSubscriptions = FilterUnauthorizedFields(schema.SubscriptionType.Fields)
                .Select(q => q.Name)
                .ToArray();
            Assert.Single(unauthorizedSubscriptions);
            Assert.Contains("__typename", unauthorizedSubscriptions);
        }
    }
}
