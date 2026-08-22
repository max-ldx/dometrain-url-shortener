using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using UrlShortener.Api;
using UrlShortener.Core;

namespace UrlShortener.Tests;

public class TokenManagerScenarios
{
    [Fact]
    public async Task ShouldCallAPIOnStart()
    {
        var tokenRangeApiClient = Substitute.For<ITokenRangeApiClient>();

        tokenRangeApiClient.AssignRangeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new TokenRange(1, 10));

        var tokenManager = new TokenManager(tokenRangeApiClient, Substitute.For<ILogger<TokenManager>>(),
            Substitute.For<TokenProvider>(), Substitute.For<IEnvironmentManager>());

        await tokenManager.StartAsync(CancellationToken.None);

        await tokenRangeApiClient.Received().AssignRangeAsync(Arg.Any<string>(), CancellationToken.None);
    }

    [Fact]
    public async Task ShouldThrowExceptionWhenNoTokensAssigned()
    {
        var tokenRangeApiClient = Substitute.For<ITokenRangeApiClient>();
        var environmentManager = Substitute.For<IEnvironmentManager>();

        var tokenManager = new TokenManager(tokenRangeApiClient, Substitute.For<ILogger<TokenManager>>(),
            Substitute.For<TokenProvider>(), environmentManager);

        await tokenManager.StartAsync(CancellationToken.None);

        environmentManager.Received().FatalError();
    }
}