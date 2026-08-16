using System.Collections.Concurrent;
using UrlShortener.Core;

namespace UrlShortener.Api.Core.Tests;

public class TokenProviderScenarios
{
    [Fact]
    public void ShouldGetTheTokenFromStart()
    {
        var provider = new TokenProvider();

        provider.AssignRange(5, 10);

        provider.GetToken().Should().Be(5);
    }

    [Fact]
    public void ShouldIncrementTokenOnGet()
    {
        var provider = new TokenProvider();

        provider.AssignRange(5, 10);
        provider.GetToken();

        provider.GetToken().Should().Be(6);
    }

    [Fact]
    public void ShouldNotReturnSameTokenTwice()
    {
        var provider = new TokenProvider();
        ConcurrentBag<long> tokens = [];
        const int start = 1;
        const int end = 1000;
        provider.AssignRange(start, end);

        Parallel.ForEach(Enumerable.Range(start, end), _ => tokens.Add(provider.GetToken()));

        tokens.Should().OnlyHaveUniqueItems();
    }
}