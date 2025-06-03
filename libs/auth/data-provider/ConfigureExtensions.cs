using HotChocolate.Execution.Configuration;
using MicraPro.Auth.Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.Auth.DataProvider;

public static class ConfigureExtensions
{
    public static void UseAuthDataProvider(this IApplicationBuilder app)
    {
        app.UseRouting().UseAuthentication().UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapPost(
                "/token",
                async (HttpRequest payload, IOAuthProviderService oAuthProvider) =>
                    await oAuthProvider.HandleAuthRequestAsync(payload)
            );
        });
    }

    public static IRequestExecutorBuilder AddAuthDataProviderServices(
        this IRequestExecutorBuilder builder
    )
    {
        builder.Services.AddAuthorization();
        builder.AddSocketSessionInterceptor<AuthSocketInterceptor>();
        builder.AddAuthorizationHandler<AuthorizePermissionHandler>();
        return builder;
    }
}
