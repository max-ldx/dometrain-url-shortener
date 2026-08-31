namespace UrlShortener.Core.Urls.List;

public class ListUrlsHandler(IUserUrlsReader userUrlsReader)
{
    public async Task<ListUrlsResponse> HandleAsync(ListUrlsRequest request, CancellationToken cancellationToken)
    {
        return await userUrlsReader.GetAsync(request.Author, cancellationToken);
    }
}