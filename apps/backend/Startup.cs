using MicraPro.AssetManagement.Domain;
using MicraPro.AssetManagement.Infrastructure;
using MicraPro.BeanManagement.Domain;
using MicraPro.BeanManagement.Infrastructure;
using MicraPro.BrewByWeight.DataProviderGraphQl;
using MicraPro.BrewByWeight.Domain;
using MicraPro.BrewByWeight.Infrastructure;
using MicraPro.ScaleManagement.DataProviderGraphQl;
using MicraPro.ScaleManagement.Domain;
using MicraPro.ScaleManagement.Infrastructure;
using MicraPro.Shared.Infrastructure;

namespace MicraPro.Backend;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddMemoryCache()
            .AddGraphQlServices(configuration)
            .AddSharedInfrastructureServices(configuration)
            .AddScaleManagementDomainServices(configuration)
            .AddScaleManagementInfrastructureServices(configuration)
            .AddScaleManagementDataProviderGraphQlServices(configuration)
            .AddBeanManagementDomainServices(configuration)
            .AddBeanManagementInfrastructureServices(configuration)
            .AddAssetManagementDomainServices(configuration)
            .AddAssetManagementInfrastructureServices(configuration)
            .AddBrewByWeightDomainServices(configuration)
            .AddBrewByWeightInfrastructureServices(configuration)
            .AddBrewByWeightDataProviderGraphQlServices(configuration)
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
        app.UseWebSockets();
        app.UseCors();
        app.MapGraphQL("/graphql", Schema.DefaultName);
    }
}
