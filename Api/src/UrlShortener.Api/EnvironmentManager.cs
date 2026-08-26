namespace UrlShortener.Api;

public class EnvironmentManager : IEnvironmentManager
{
    public void FatalError()
    {
        Environment.Exit(1);
    }
}