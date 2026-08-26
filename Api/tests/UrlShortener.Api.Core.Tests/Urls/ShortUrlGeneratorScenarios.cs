using UrlShortener.Core;
using UrlShortener.Core.Urls;

namespace UrlShortener.Api.Core.Tests.Urls;

public class ShortUrlGeneratorScenarios
{
    [Fact]
    public void ShouldReturnShortUrlForZero()
    {
        var tokenProvider = new TokenProvider();
        tokenProvider.AssignRange(0, 10);
        var shortUrlGenerator = new ShortUrlGenerator(tokenProvider);

        var shortUrl = shortUrlGenerator.GenerateUniqueUrl();

        shortUrl.Should().Be("0");
    }

    [Fact]
    public void ShouldReturnShortUrlFor10001()
    {
        var tokenProvider = new TokenProvider();
        tokenProvider.AssignRange(10_001, 20_000);
        var shortUrlGenerator = new ShortUrlGenerator(tokenProvider);

        var shortUrl = shortUrlGenerator.GenerateUniqueUrl();

        shortUrl.Should().Be("2bJ");
    }
}