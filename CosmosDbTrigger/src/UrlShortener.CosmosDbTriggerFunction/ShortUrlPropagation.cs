using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace UrlShortener.CosmosDbTriggerFunction;

public class ShortUrlPropagation(ILoggerFactory loggerFactory, Container container)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<ShortUrlPropagation>();

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
            _logger.LogInformation("Short Url: {ShortUrl}", document.Id);
            try
            {
                var cosmosDbDocument = new ShortenedUrlEntity(
                    document.LongUrl,
                    document.Id,
                    document.CreatedOn,
                    document.CreatedBy
                );
                await container.UpsertItemAsync(cosmosDbDocument, new PartitionKey(document.CreatedBy));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing to Cosmos DB");
                throw;
            }
        }
    }
}