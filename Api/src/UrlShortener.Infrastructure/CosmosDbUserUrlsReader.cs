using Microsoft.Azure.Cosmos;
using UrlShortener.Core.Urls.List;

namespace UrlShortener.Infrastructure;

public class CosmosDbUserUrlsReader(Container container) : IUserUrlsReader
{
    private const int PAGE_SIZE = 20;

    public async Task<ListUrlsResponse> GetAsync(string createdBy, CancellationToken cancellationToken)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.PartitionKey = @partitionKey")
            .WithParameter("@partitionKey", createdBy);

        var iterator = container.GetItemQueryIterator<ShortenedUrlEntity>(query,
            requestOptions: new QueryRequestOptions()
            {
                PartitionKey = new PartitionKey(createdBy),
                MaxItemCount = PAGE_SIZE
            });

        var results = new List<ShortenedUrlEntity>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            results.AddRange(response);
        }

        return new ListUrlsResponse([.. results.Select(e => new UrlItem(e.ShortUrl, e.LongUrl, e.CreatedOn))]);
    }
}