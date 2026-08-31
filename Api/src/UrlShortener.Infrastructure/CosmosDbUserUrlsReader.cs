using System.Text;
using Microsoft.Azure.Cosmos;
using UrlShortener.Core.Urls.List;

namespace UrlShortener.Infrastructure;

public class CosmosDbUserUrlsReader(Container container) : IUserUrlsReader
{
    public async Task<ListUrlsResponse> GetAsync(string createdBy, int pageSize, string? continuationToken,
        CancellationToken cancellationToken)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.PartitionKey = @partitionKey")
            .WithParameter("@partitionKey", createdBy);

        var queryContinuationToken = continuationToken is null
            ? null
            : Encoding.UTF8.GetString(Convert.FromBase64String(continuationToken));

        var iterator = container.GetItemQueryIterator<ShortenedUrlEntity>(query,
            continuationToken: queryContinuationToken,
            requestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(createdBy),
                MaxItemCount = pageSize
            });

        var results = new List<ShortenedUrlEntity>();
        string? resultContinuationToken = null;
        var readItemsCount = 0;

        while (readItemsCount < pageSize && iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            results.AddRange(response);
            readItemsCount += response.Count;
            resultContinuationToken = response.ContinuationToken;
        }

        var responseContinuationToken = resultContinuationToken is null
            ? null
            : Convert.ToBase64String(Encoding.UTF8.GetBytes(resultContinuationToken));

        return new ListUrlsResponse([.. results.Select(e => new UrlItem(e.ShortUrl, e.LongUrl, e.CreatedOn))],
            responseContinuationToken);
    }
}