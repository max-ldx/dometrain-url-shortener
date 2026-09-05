namespace UrlShortener.Core.Urls.List;

public record UrlItem(
    string Id,
    Uri ShortUrl,
    Uri LongUrl,
    DateTimeOffset CreatedOn);