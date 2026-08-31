namespace UrlShortener.Core.Urls.List;

public record ListUrlsRequest(string Author, int? pageSize, string? continuationToken);