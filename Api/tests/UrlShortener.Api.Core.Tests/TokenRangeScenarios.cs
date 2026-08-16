using UrlShortener.Core;

namespace UrlShortener.Api.Core.Tests;

public class TokenRangeScenarios
{
    [Fact]
    public void WhenStartTokenIsGreaterThanEndTokenTheThrowsException()
    {
        var act = () => new TokenRange(10, 5);
        
        act.Should().Throw<ArgumentException>().WithMessage("End must be greater than or equal to start.");
    }
}