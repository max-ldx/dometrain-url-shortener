using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using UrlShortener.RedirectApi.Infrastructure;
using UrlShortener.RedirectApi.Tests.TestDoubles;

namespace UrlShortener.RedirectApi.Tests;

public class RedirectScenarios(ApiFixture fixture) : IClassFixture<ApiFixture>
{
    private readonly HttpClient _client = fixture.CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false
    });

    private InMemoryShortenedUrlReader _storage = fixture.ShortenedUrlReader;

    [Fact]
    public async Task ShouldReturn301RedirectWithUrlWhenShortUrlExists()
    {
        const string shortUrl = "abc123";
        _storage.Add(shortUrl, new ReadLongUrlResponse(true, "https://dometrain.com"));

        var response = await _client.GetAsync($"/r/{shortUrl}");

        response.StatusCode.Should().Be(HttpStatusCode.MovedPermanently);
        response.Headers.Location.Should().Be("https://dometrain.com");
    }

    [Fact]
    public async Task ShouldReturn404NotFoundWhenUrlDoesNotExist()
    {
        const string shortUrl = "non-existing";

        var response = await _client.GetAsync($"/r/{shortUrl}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}