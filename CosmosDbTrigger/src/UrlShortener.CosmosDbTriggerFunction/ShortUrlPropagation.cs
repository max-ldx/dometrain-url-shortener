using System;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace UrlShortener.CosmosDbTriggerFunction;

public class ShortUrlPropagation(ILogger<ShortUrlPropagation> logger, Container container)
{
    [Function("ShortUrlPropagation")]
    public async Task Run([CosmosDBTrigger(
            databaseName: "urls",
            containerName: "items",
            Connection = "CosmosDbConnection",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true)]
        IReadOnlyList<UrlDocument> input)
    {
        if (input.Count <= 0) return;
        foreach (var document in input)
        {
            logger.LogInformation("Short Url {ShortUrl}", document.Id);

            try
            {
                await container.UpsertItemAsync(document, new PartitionKey(document.CreatedBy));
            }
            catch (Exception ex)
            {
                logger.LogError("Error writing to Cosmos DB");
                throw;
            }
        }
    }

    public class UrlDocument
    {
        public string Id { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string LongUrl { get; set; }
    }
}