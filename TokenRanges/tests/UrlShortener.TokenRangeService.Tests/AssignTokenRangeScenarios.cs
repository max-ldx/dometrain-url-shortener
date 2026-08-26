using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace UrlShortener.TokenRangeService.Tests;

public class AssignTokenRangeScenarios(Fixture fixture) : IClassFixture<Fixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    public async Task ShouldReturnRangeWhenRequested()
    {
        var response = await _client.PostAsJsonAsync("/assign", new AssignTokenRangeRequest("tests"));
        var tokenRange = await response.Content.ReadFromJsonAsync<TokenRangeResponse>();

        tokenRange?.Start.Should().BeGreaterThan(0);
        tokenRange?.End.Should().BeGreaterThan(tokenRange.Start);
    }

    [Fact]
    public async Task ShouldNotRepeatRangeWhenRequested()
    {
        var requestResponse1 = await _client.PostAsJsonAsync("/assign", new AssignTokenRangeRequest("tests"));
        var requestResponse2 = await _client.PostAsJsonAsync("/assign", new AssignTokenRangeRequest("tests"));

        requestResponse1.StatusCode.Should().Be(HttpStatusCode.OK);
        requestResponse2.StatusCode.Should().Be(HttpStatusCode.OK);

        var tokenRange1 = await requestResponse1.Content.ReadFromJsonAsync<TokenRangeResponse>();
        var tokenRange2 = await requestResponse2.Content.ReadFromJsonAsync<TokenRangeResponse>();

        tokenRange2!.Start.Should().BeGreaterThan(tokenRange1!.End);
    }

    [Fact]
    public async Task ShouldNotRepeatRangeOnMultipleRequests()
    {
        ConcurrentBag<TokenRangeResponse> ranges = [];

        await Parallel.ForEachAsync(Enumerable.Range(1, 100), async (number, cancellationToken) =>
        {
            var response = await _client.PostAsJsonAsync("/assign", new AssignTokenRangeRequest(number.ToString()),
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var range =
                    await response.Content.ReadFromJsonAsync<TokenRangeResponse>(cancellationToken);
                ranges.Add(range!);
            }
        });

        ranges.Should().OnlyHaveUniqueItems(x => x.Start);
        ranges.Should().OnlyHaveUniqueItems(x => x.End);
    }
}