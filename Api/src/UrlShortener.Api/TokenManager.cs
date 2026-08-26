using UrlShortener.Core;

namespace UrlShortener.Api;

public class TokenManager(
    ITokenRangeApiClient client,
    ILogger<TokenManager> logger,
    TokenProvider tokenProvider,
    IEnvironmentManager environmentManager)
    : IHostedService
{
    private readonly string _machineIdentifier = Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID") ?? "unknown";

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting token manager");

        tokenProvider.ReachingRangeLimit += async (sender, args) => await AssignNewRangeAsync(cancellationToken);

        try
        {
            await AssignNewRangeAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "TokenManager failed to start due to an error.");
            environmentManager.FatalError();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping token manager");
        return Task.CompletedTask;
    }

    private async Task AssignNewRangeAsync(CancellationToken cancellationToken)
    {
        var range = await client.AssignRangeAsync(_machineIdentifier, cancellationToken);

        if (range is null) throw new Exception("No tokens assigned");

        tokenProvider.AssignRange(range);
        logger.LogInformation("Assigned range: {Start}-{End}", range.Start, range.End);
    }
}