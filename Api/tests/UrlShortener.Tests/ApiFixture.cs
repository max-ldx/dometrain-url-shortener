using Microsoft.AspNetCore.Mvc.Testing;
using UrlShortener.Api;

namespace UrlShortener.Tests;

public class ApiFixture : WebApplicationFactory<IApiAssemblyMarker>
{
}