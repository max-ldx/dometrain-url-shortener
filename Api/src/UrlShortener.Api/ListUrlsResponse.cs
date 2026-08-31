namespace UrlShortener.Api;

public record ListUrlsResponse(IEnumerable<ListUrlItem> Urls, string? ContinuationToken);