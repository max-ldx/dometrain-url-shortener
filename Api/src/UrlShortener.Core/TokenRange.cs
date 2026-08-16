namespace UrlShortener.Core;

public record TokenRange
{
    public long Start { get; }
    public long End { get; }

    public TokenRange(long start, long end)
    {
        if (end < start) throw new ArgumentException("End must be greater than or equal to start.");
        Start = start;
        End = end;
    }
}