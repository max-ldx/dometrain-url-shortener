using Newtonsoft.Json;

namespace UrlShortener.CosmosDbTriggerFunction;

public class ShortenedUrlEntity(
    string longUrl,
    string shortUrl,
    DateTimeOffset createdOn,
    string createdBy)
{
    public string LongUrl { get; } = longUrl;

    [JsonProperty(PropertyName = "id")] // Cosmos DB Unique Identifier
    public string ShortUrl { get; } = shortUrl;

    public DateTimeOffset CreatedOn { get; } = createdOn;

    [JsonProperty(PropertyName = "PartitionKey")] // Cosmos DB Partition Key
    public string CreatedBy { get; } = createdBy;
}