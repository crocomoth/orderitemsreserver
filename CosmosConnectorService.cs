using Microsoft.Azure.Cosmos;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace OrderItemsReserver
{
    public class CosmosConnectorService
    {
        private const string DataBaseId = "eshop";
        private const string ContainerId = "orders";

        private readonly CosmosOptions _options;

        public CosmosConnectorService(IOptions<CosmosOptions> options)
        {
            _options = options.Value;
        }

        public async Task WriteDataToStorage(OrderItem data)
        {
            try
            {
                CosmosClient client = new(_options.Url, _options.PrimaryKey);
                var database = client.GetDatabase(DataBaseId);
                var container = database.GetContainer(ContainerId);

                //var containerResponse = await database.CreateContainerIfNotExistsAsync(ContainerId, "Id").ConfigureAwait(false);
                //var container = containerResponse.Container;

                var result = await container.CreateItemAsync(data).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return;
            }
        }
    }
}
