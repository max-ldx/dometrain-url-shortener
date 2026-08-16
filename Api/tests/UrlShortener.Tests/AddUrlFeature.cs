using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using UrlShortener.Core.Urls.Add;

namespace UrlShortener.Tests;

public class AddUrlFeature(ApiFixture fixture) : IClassFixture<ApiFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    public async Task GivenLongUrlShouldReturnShortUrl()
    {
        var response = await _client.PostAsJsonAsync("/api/urls",
            new AddUrlRequest(new Uri("https://dometrain.com"), ""));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var addUrlResponse = await response.Content.ReadFromJsonAsync<AddUrlResponse>();
        addUrlResponse!.ShortUrl.Should().NotBeNull();
    }
}