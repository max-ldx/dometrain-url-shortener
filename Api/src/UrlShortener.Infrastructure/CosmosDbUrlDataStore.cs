using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using UrlShortener.Core.Urls.Add;

namespace UrlShortener.Infrastructure;

public class CosmosDbUrlDataStore(Container container) : IUrlDataStore
{
    public async Task AddAsync(ShortenedUrl shortened, CancellationToken cancellationToken)
    {
        var document = (ShortenedUrlCosmos)shortened;

        await container.CreateItemAsync(document, new PartitionKey(document.PartitionKey),
            cancellationToken: cancellationToken);
    }

    internal class ShortenedUrlCosmos(string longUrl, string shortUrl, string createdBy, DateTimeOffset createdOn)
    {
        public string LongUrl { get; } = longUrl;

        [JsonProperty(PropertyName = "id")] // Cosmos DB Unique Identifier
        public string ShortUrl { get; } = shortUrl;

        public DateTimeOffset CreatedOn { get; } = createdOn;

        public string CreatedBy { get; } = createdBy;

        public string PartitionKey => ShortUrl[..1]; // Cosmos DB Partition Key

        public static implicit operator ShortenedUrl(ShortenedUrlCosmos url) =>
            new(new Uri(url.LongUrl), url.ShortUrl, url.CreatedBy, url.CreatedOn);

        public static explicit operator ShortenedUrlCosmos(ShortenedUrl url) =>
            new(url.LongUrl.ToString(), url.ShortUrl, url.CreatedBy, url.CreatedOn);
    }
}