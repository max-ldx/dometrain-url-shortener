using Microsoft.Extensions.Logging;
using NSubstitute;
using StackExchange.Redis;
using UrlShortener.RedirectApi.Infrastructure;

namespace UrlShortener.RedirectApi.Tests;

[Collection("Api collection")]
public class RedisCacheScenarios(ApiFixture fixture)
{
    private readonly IConnectionMultiplexer _connectionMultiplexer =
        ConnectionMultiplexer.Connect(fixture.RedisConnectionString);

    [Fact]
    public async Task ShouldGetFromReaderIfNotInCache()
    {
        var reader = Substitute.For<IShortenedUrlReader>();
        reader.GetLongUrlAsync("short", Arg.Any<CancellationToken>())
            .Returns(new ReadLongUrlResponse(true, "http://google.com"));
        var cache = new RedisUrlReader(reader, _connectionMultiplexer, Substitute.For<ILogger<RedisUrlReader>>());

        _ = await cache.GetLongUrlAsync("short", CancellationToken.None);

        await reader.Received().GetLongUrlAsync("short", Arg.Any<CancellationToken>());
    }
}