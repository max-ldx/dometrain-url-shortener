namespace UrlShortener.Core;

public class ShortUrlGenerator(TokenProvider tokenProvider)
{
    public string GenerateUniqueUrl() => tokenProvider.GetToken().EncodeToBase62();
}