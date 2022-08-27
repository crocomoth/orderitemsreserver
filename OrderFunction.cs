using Azure;
using Azure.Messaging.EventGrid;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OrderItemsReserver
{
    public class OrderFunction
    {
        private readonly BlobStorageConnector _storageConnector;
        private readonly CosmosConnectorService _cosmosConnectorService;

        public OrderFunction(BlobStorageConnector storageConnector, CosmosConnectorService cosmosConnectorService)
        {
            _storageConnector = storageConnector;
            _cosmosConnectorService = cosmosConnectorService;
        }

        [FunctionName("OrderFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("OrderFunction function processed a request.");

            int tries = 0;
            const int MaxRetries = 3;
            var exceptions = new List<Exception>();

            do
            {
                try
                {
                    var content = GetRequestBody(req);
                    await _storageConnector.WriteDataToBlob(content).ConfigureAwait(false);
                    break;

                    //var order = JsonConvert.DeserializeObject<OrderItem>(content);
                    //await _cosmosConnectorService.WriteDataToStorage(order);
                }
                catch (Exception e)
                {
                    log.LogError(e, e.Message);
                    exceptions.Add(e);
                    tries++;
                }
            } while (tries < MaxRetries);

            if (exceptions.Any())
            {
                // azure Logic app
                return new BadRequestResult();
            }
            else
            {
                return new OkResult();
            }
        }

        [FunctionName("ServiceBusCallback")]
        public async Task<IActionResult> ServiceBus([ServiceBusTrigger("mainqueue", Connection = "ServiceBusConnection")] string content, IOptions<CosmosOptions> options, ILogger log)
        {
            log.LogInformation("ServiceBusCallback function processed a request.");
            int tries = 0;
            const int MaxRetries = 3;
            var exceptions = new List<Exception>();

            do
            {
                try
                {
                    await _storageConnector.WriteDataToBlob(content).ConfigureAwait(false);
                    break;
                }
                catch (Exception e)
                {
                    log.LogError(e, e.Message);
                    exceptions.Add(e);
                    tries++;
                }
            } while (tries < MaxRetries);

            if (exceptions.Any())
            {
                // azure Logic app
                var httpClient = new HttpClient();
                await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, options.Value.LogicAppUrl));
            }

            return new OkResult();
        }

        public static string GetRequestBody(HttpRequest req)
        {
            var bodyStream = new StreamReader(req.Body);
            bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
            var bodyText = bodyStream.ReadToEnd();
            return bodyText;
        }
    }
}
