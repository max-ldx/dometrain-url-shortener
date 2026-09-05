using UrlShortener.Core.Extensions;

namespace UrlShortener.Core.Urls.List;

public class ListUrlsHandler(IUserUrlsReader userUrlsReader, RedirectLinkBuilder redirectLinkBuilder)
{
    private const int MaxPageSize = 20;

    public async Task<ListUrlsResponse> HandleAsync(ListUrlsRequest request, CancellationToken cancellationToken)
    {
        var urls = await userUrlsReader.GetAsync(request.Author,
            int.Min(request.PageSize ?? MaxPageSize, MaxPageSize),
            request.ContinuationToken,
            cancellationToken);
        
        return urls.MapToResponse(redirectLinkBuilder);
    }
}