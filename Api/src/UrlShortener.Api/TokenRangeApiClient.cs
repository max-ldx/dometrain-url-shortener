using UrlShortener.Core;

namespace UrlShortener.Api;

public class TokenRangeApiClient(IHttpClientFactory httpClientFactory) : ITokenRangeApiClient
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("TokenRangeService");

    public async Task<TokenRange?> AssignRangeAsync(string machineKey, CancellationToken cancellationToken)
    {
        var response = await _client.PostAsJsonAsync("assign", new { Key = machineKey, }, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to assign new token range");
        }

        var range = await response.Content.ReadFromJsonAsync<TokenRange>(cancellationToken: cancellationToken);

        return range;
    }
}