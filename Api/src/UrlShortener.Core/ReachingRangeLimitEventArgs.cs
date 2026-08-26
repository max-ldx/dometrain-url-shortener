namespace UrlShortener.Core;

public class ReachingRangeLimitEventArgs : EventArgs
{
    public long Token { get; set; }

    public long RangeLimit { get; set; }
}