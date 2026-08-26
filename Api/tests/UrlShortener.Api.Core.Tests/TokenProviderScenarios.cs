using System.Collections.Concurrent;
using UrlShortener.Core;

namespace UrlShortener.Api.Core.Tests;

public class TokenProviderScenarios
{
    private readonly TokenProvider _provider = new();

    [Fact]
    public void ShouldGetTheTokenFromStart()
    {
        _provider.AssignRange(5, 10);

        _provider.GetToken().Should().Be(5);
    }

    [Fact]
    public void ShouldIncrementTokenOnGet()
    {
        _provider.AssignRange(5, 10);
        _provider.GetToken();

        _provider.GetToken().Should().Be(6);
    }

    [Fact]
    public void ShouldNotReturnSameTokenTwice()
    {
        ConcurrentBag<long> tokens = [];
        const int start = 1;
        const int end = 1000;
        _provider.AssignRange(start, end);

        Parallel.ForEach(Enumerable.Range(start, end), _ => tokens.Add(_provider.GetToken()));

        tokens.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void ShouldUseMultipleRanges()
    {
        _provider.AssignRange(1, 2);
        _provider.AssignRange(42, 45);
        _provider.GetToken();
        _provider.GetToken();

        var token = _provider.GetToken();

        token.Should().Be(42);
    }

    [Fact]
    public void ShouldTriggerReachingRangeLimitEventWhenRangeIsAt80Percent()
    {
        _provider.AssignRange(1, 10);
        var eventTriggered = false;
        _provider.ReachingRangeLimit += (sender, args) => eventTriggered = true;

        for (var i = 0; i < 8; i++) _provider.GetToken();

        eventTriggered.Should().BeTrue();
    }
}