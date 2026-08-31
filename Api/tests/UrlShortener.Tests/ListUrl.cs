using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using UrlShortener.Api;
using UrlShortener.Core.Urls.Add;

namespace UrlShortener.Tests;

[Collection("Api collection")]
public class ListUrl(ApiFixture fixture)
{
    private const string UrlsEndpoint = "/api/urls";
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    public async Task ShouldReturn200OkWithListOfUrls()
    {
        await AddUrl();

        var response = await _client.GetAsync(UrlsEndpoint, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var urls = await response.Content.ReadFromJsonAsync<ListUrlsResponse>(
            cancellationToken: TestContext.Current.CancellationToken);
        urls?.Urls.Should().NotBeEmpty();
    }


    [Fact]
    public async Task ShouldReturnUrlWhenCreatedFirst()
    {
        var response = await AddUrl("https://testing-in-lists.tests");

        var getResponse = await _client.GetAsync(UrlsEndpoint, TestContext.Current.CancellationToken);
        var urls = await getResponse.Content.ReadFromJsonAsync<ListUrlsResponse>(
            cancellationToken: TestContext.Current.CancellationToken);

        urls?.Urls.Should().Contain(url => response != null && url.ShortUrl == response.ShortUrl);
    }

    private async Task<AddUrlResponse?> AddUrl(string? url = null)
    {
        url ??= "https://testing-in-lists.tests";

        var response = await _client.PostAsJsonAsync(UrlsEndpoint, new AddUrlRequest(new Uri(url), ""));

        return await response.Content.ReadFromJsonAsync<AddUrlResponse>();
    }
}