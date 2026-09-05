using Microsoft.Extensions.Time.Testing;
using UrlShortener.Api.Core.Tests.TestDoubles;
using UrlShortener.Core;
using UrlShortener.Core.Urls;
using UrlShortener.Core.Urls.Add;

namespace UrlShortener.Api.Core.Tests.Urls;

public class AddUrlScenarios
{
    private readonly AddUrlHandler _handler;
    private readonly FakeTimeProvider _timeProvider;
    private readonly InMemoryUrlDataStore _urlDataStore;

    public AddUrlScenarios()
    {
        var tokenProvider = new TokenProvider();
        tokenProvider.AssignRange(1, 5);
        var shortUrlGenerator = new ShortUrlGenerator(tokenProvider);
        _timeProvider = new FakeTimeProvider();
        _urlDataStore = new InMemoryUrlDataStore();
        _handler = new AddUrlHandler(shortUrlGenerator, _urlDataStore, _timeProvider,
            new RedirectLinkBuilder(new Uri("https://tests/")));
    }

    [Fact]
    public async Task ShouldReturnShortenedUrl()
    {
        var request = CreateAddUrlRequest();
        var response = await _handler.HandleAsync(request, CancellationToken.None);

        response.Succeeded.Should().BeTrue();
        response.Value!.Id.Should().NotBeEmpty();
        response.Value!.Id.Should().Be("1");
    }

    [Fact]
    public async Task ShouldSaveShortUrl()
    {
        var request = CreateAddUrlRequest();

        var response = await _handler.HandleAsync(request, CancellationToken.None);

        response.Succeeded.Should().BeTrue();
        _urlDataStore.Should().ContainKey(response.Value!.Id);
    }

    [Fact]
    public async Task ShouldSaveShortUrlWithCreatedByAndCreatedOn()
    {
        var request = CreateAddUrlRequest();

        var response = await _handler.HandleAsync(request, CancellationToken.None);

        response.Succeeded.Should().BeTrue();
        _urlDataStore.Should().ContainKey(response.Value!.Id);
        _urlDataStore[response.Value!.Id].CreatedBy.Should().Be(request.CreatedBy);
        _urlDataStore[response.Value!.Id].CreatedOn.Should().Be(_timeProvider.GetUtcNow());
    }

    [Fact]
    public async Task ShouldReturnErrorIfCreatedByIsEmpty()
    {
        var request = CreateAddUrlRequest(string.Empty);

        var response = await _handler.HandleAsync(request, CancellationToken.None);

        response.Succeeded.Should().BeFalse();
        response.Error.Code.Should().Be("missing_value");
    }

    private static AddUrlRequest CreateAddUrlRequest(string createdBy = "max@test.com")
    {
        return new AddUrlRequest(new Uri("https://dometrain.com"), createdBy);
    }
}