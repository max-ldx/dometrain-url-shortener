namespace UrlShortener.Core.Urls;

public class ShortUrlGenerator(TokenProvider tokenProvider)
{
    public string GenerateUniqueUrl() => tokenProvider.GetToken().EncodeToBase62();
}