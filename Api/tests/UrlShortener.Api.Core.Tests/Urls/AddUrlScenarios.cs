using Microsoft.Extensions.Time.Testing;
using UrlShortener.Api.Core.Tests.TestDoubles;
using UrlShortener.Core;
using UrlShortener.Core.Urls;
using UrlShortener.Core.Urls.Add;

namespace UrlShortener.Api.Core.Tests.Urls;

public class AddUrlScenarios
{
    private readonly AddUrlHandler _handler;
    private readonly InMemoryUrlDataStore _urlDataStore;
    private readonly FakeTimeProvider _timeProvider;

    public AddUrlScenarios()
    {
        var tokenProvider = new TokenProvider();
        tokenProvider.AssignRange(1, 5);
        var shortUrlGenerator = new ShortUrlGenerator(tokenProvider);
        _timeProvider = new FakeTimeProvider();
        _urlDataStore = new InMemoryUrlDataStore();
        _handler = new AddUrlHandler(shortUrlGenerator, _urlDataStore, _timeProvider);
    }

    [Fact]
    public async Task ShouldReturnShortenedUrl()
    {
        var request = CreateAddUrlRequest();
        var response = await _handler.HandleAsync(request, CancellationToken.None);

        response.ShortUrl.Should().NotBeEmpty();
        response.ShortUrl.Should().Be("1");
    }

    [Fact]
    public async Task ShouldSaveShortUrl()
    {
        var request = CreateAddUrlRequest();
        
        var response = await _handler.HandleAsync(request, CancellationToken.None);
        
        _urlDataStore.Should().ContainKey(response.ShortUrl);
    }
    
    [Fact]
    public async Task ShouldSaveShortUrlWithCreatedByAndCreatedOn()
    {
        var request = CreateAddUrlRequest();
        
        var response = await _handler.HandleAsync(request, CancellationToken.None);
        
        _urlDataStore.Should().ContainKey(response.ShortUrl);
        _urlDataStore[response.ShortUrl].CreatedBy.Should().Be(request.CreatedBy);
        _urlDataStore[response.ShortUrl].CreatedOn.Should().Be(_timeProvider.GetUtcNow());
    }

    private static AddUrlRequest CreateAddUrlRequest()
    {
        return new AddUrlRequest(new Uri("https://dometrain.com"), "max@test.com");
    }
}