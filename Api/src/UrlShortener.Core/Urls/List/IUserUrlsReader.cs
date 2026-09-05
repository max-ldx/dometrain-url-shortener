namespace UrlShortener.Core.Urls.List;

public interface IUserUrlsReader
{
    Task<UserUrls> GetAsync(string createdBy, int pageSize, string? continuationToken,
        CancellationToken cancellationToken);
}