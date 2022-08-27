using BlazorShared;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.eShopWeb.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderItemsReserver;

[assembly: FunctionsStartup(typeof(Startup))]
namespace OrderItemsReserver
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            AddStorageOptions(builder, configuration);
            AddCosmosOptions(builder, configuration);

            builder.Services.AddScoped<BlobStorageConnector, BlobStorageConnector>();
            builder.Services.AddScoped<CosmosConnectorService, CosmosConnectorService>();
        }

        private static void AddStorageOptions(IFunctionsHostBuilder builder, IConfiguration configuration)
        {
            var storageConnectionString = configuration[$"{BaseUrlConfiguration.CONFIG_NAME}:storageBase"];
            var config = new BaseUrlConfiguration()
            {
                StorageBase = storageConnectionString
            };

            var options = Options.Create(config);
            builder.Services.AddTransient(_ => options);
        }

        private static void AddCosmosOptions(IFunctionsHostBuilder builder, IConfiguration configuration)
        {
            var url = configuration[$"{CosmosOptions.Name}:url"];
            var key = configuration[$"{CosmosOptions.Name}:apiKey"];
            var eventGridUrl = configuration[$"{CosmosOptions.Name}:eventgridurl"];
            var eventGridKey = configuration[$"{CosmosOptions.Name}:eventgridkey"];
            var config = new CosmosOptions()
            {
                Url = url,
                PrimaryKey = key,
                EventGridUrl = eventGridUrl,
                EventGridKey = eventGridKey
            };

            var options = Options.Create(config);
            builder.Services.AddTransient(_ => options);
        }
    }
}
