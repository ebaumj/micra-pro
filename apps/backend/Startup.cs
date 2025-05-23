using MicraPro.Auth.DataProvider;
using MicraPro.Auth.Domain;
using MicraPro.ScaleManagement.Domain;
using MicraPro.ScaleManagement.Infrastructure;
using MicraPro.Shared.Domain;
using MicraPro.Shared.Infrastructure;

namespace MicraPro.Backend;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddMemoryCache()
            .AddGraphQlServices(configuration)
            .AddAuthDomainServices(configuration)
            .AddSharedInfrastructureServices()
            .AddScaleManagementDomainServices(configuration)
            .AddScaleManagementInfrastructureServices(configuration)
            .AddCors(options =>
            {
                options.AddDefaultPolicy(b =>
                {
                    b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
    }

    public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
    {
        app.UseAuthDataProvider();
        app.UseWebSockets();
        app.UseCors();
        app.MapGraphQL("/graphql", Schema.DefaultName);
    }
}
