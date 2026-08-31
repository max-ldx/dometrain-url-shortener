namespace UrlShortener.Core.Urls.List;

public class ListUrlsHandler(IUserUrlsReader userUrlsReader)
{
    private const int MaxPageSize = 20;

    public async Task<ListUrlsResponse> HandleAsync(ListUrlsRequest request, CancellationToken cancellationToken)
    {
        return await userUrlsReader.GetAsync(request.Author, int.Min(request.pageSize ?? MaxPageSize, MaxPageSize),
            request.continuationToken,
            cancellationToken);
    }
}