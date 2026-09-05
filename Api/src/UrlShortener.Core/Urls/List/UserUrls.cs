namespace UrlShortener.Core.Urls.List;

public record UserUrls(IEnumerable<UserUrlItem> Urls, string? ContinuationToken = null);