using Newtonsoft.Json;

namespace UrlShortener.Infrastructure;

public class ShortenedUrlEntity(
    string longUrl,
    string shortUrl,
    DateTimeOffset createdOn)
{
    public string LongUrl { get; } = longUrl;

    [JsonProperty(PropertyName = "id")] // Cosmos DB Unique Identifier
    public string ShortUrl { get; } = shortUrl;

    public DateTimeOffset CreatedOn { get; } = createdOn;
}