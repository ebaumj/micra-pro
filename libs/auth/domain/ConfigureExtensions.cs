using MicraPro.Auth.DataDefinition;
using MicraPro.Auth.Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.Auth.Domain;

public static class ConfigureExtensions
{
    public static IServiceCollection AddAuthDomainServices(
        this IServiceCollection collection,
        IConfiguration configurationManager
    )
    {
        DotNetEnv.Env.Load();
        var runtimeOptions = new AuthDomainRuntimeOptions()
        {
            Audience = configurationManager
                .GetSection(AuthDomainOptions.SectionName)
                .Get<AuthDomainOptions>()!
                .Audience,
            PrivateKey = Environment
                .GetEnvironmentVariable("JWT_PRIVATE_KEY")!
                .Select(c => (byte)c)
                .ToArray(),
        };
        collection
            .Configure<AuthDomainOptions>(
                configurationManager.GetSection(AuthDomainOptions.SectionName)
            )
            .Configure<AuthDomainRuntimeOptions>(options =>
            {
                options.Audience = runtimeOptions.Audience;
                options.PrivateKey = runtimeOptions.PrivateKey;
            })
            .AddTransient<IJwtCreatorService, JwtCreatorService>()
            .AddScoped<IAccountManagementService, NoAccountManagementService>()
            .AddScoped<IRolePermissionService<Permission>, RolePermissionService>()
            .AddScoped<IAuthorizationService<Permission>, AuthorizationService<Permission>>()
            .AddScoped<IOAuthProviderService, OAuthProviderService>()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    JwtCreatorService.CreateAccessValidationParameters(
                        runtimeOptions,
                        configurationManager
                            .GetSection(AuthDomainOptions.SectionName)
                            .Get<AuthDomainOptions>()!
                            .JwtValidIssuers
                    );
            });
        return collection;
    }
}
