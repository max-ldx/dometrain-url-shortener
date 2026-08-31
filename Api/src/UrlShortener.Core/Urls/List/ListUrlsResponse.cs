namespace UrlShortener.Core.Urls.List;

public record ListUrlsResponse(IEnumerable<UrlItem> Urls, string? ContinuationToken = null);