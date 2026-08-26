using System.Net;
using Microsoft.Azure.Cosmos;

namespace UrlShortener.RedirectApi.Infrastructure;

public class CosmosShortenedUrlReader(Container container) : IShortenedUrlReader
{
    private readonly Container _container = container;

    public async Task<ReadLongUrlResponse> GetLongUrlAsync(string shortUrl, CancellationToken cancellationToken)
    {
        try
        {
            var record = await _container.ReadItemAsync<CosmosUrlRecord>(
                shortUrl,
                new PartitionKey(shortUrl[..1]),
                cancellationToken: cancellationToken);

            return record switch
            {
                { Resource: not null } => new ReadLongUrlResponse(true, record.Resource.LongUrl),
                _ => new ReadLongUrlResponse(false, null)
            };
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return new ReadLongUrlResponse(false, null);
        }
    }

    private record CosmosUrlRecord(string LongUrl);
}