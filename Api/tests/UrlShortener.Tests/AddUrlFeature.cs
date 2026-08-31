using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using UrlShortener.Core.Urls.Add;

namespace UrlShortener.Tests;

[Collection("Api collection")]
public class AddUrlFeature(ApiFixture fixture)
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    public async Task GivenLongUrlShouldReturnShortUrl()
    {
        var response = await _client.PostAsJsonAsync("/api/urls",
            new AddUrlRequest(new Uri("https://dometrain.com"), ""),
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var addUrlResponse =
            await response.Content.ReadFromJsonAsync<AddUrlResponse>(
                cancellationToken: TestContext.Current.CancellationToken);
        addUrlResponse!.ShortUrl.Should().NotBeNull();
    }
}