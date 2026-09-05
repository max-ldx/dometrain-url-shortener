namespace UrlShortener.Core.Urls;

public class RedirectLinkBuilder(Uri redirectServiceEndpoint)
{
    public Uri LinkTo(string shortUrl) => new(redirectServiceEndpoint, shortUrl);
}