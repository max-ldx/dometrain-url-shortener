namespace UrlShortener.Core.Urls.Add;

public class ShortenedUrl(Uri longUrl, string shortUrl, string createdBy, DateTimeOffset createdOn)
{
    public Uri LongUrl { get; } = longUrl;
    public string ShortUrl { get; } = shortUrl;
    public string CreatedBy { get; } = createdBy;
    public DateTimeOffset CreatedOn { get; } = createdOn;
}