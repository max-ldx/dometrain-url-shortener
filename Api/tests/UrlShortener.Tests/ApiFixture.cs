using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Api;
using UrlShortener.Core.Urls.Add;
using UrlShortener.Core.Urls.List;
using UrlShortener.Libraries.Testing.Extensions;
using UrlShortener.Tests.TestDoubles;

namespace UrlShortener.Tests;

public class ApiFixture : WebApplicationFactory<IApiAssemblyMarker>
{
    public ApiFixture()
    {
        Environment.SetEnvironmentVariable("RedirectService__Endpoint", "https://urlshortener.tests/r/");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
            {
                var inMemoryUrlDataStore = new InMemoryUrlDataStore();

                services.Remove<IUrlDataStore>();
                services.AddSingleton<IUrlDataStore>(inMemoryUrlDataStore);

                services.Remove<IUserUrlsReader>();
                services.AddSingleton<IUserUrlsReader>(inMemoryUrlDataStore);

                services.Remove<ITokenRangeApiClient>();
                services.AddSingleton<ITokenRangeApiClient, FakeTokenRangeApiClient>();

                services.AddAuthentication("TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });

                services.AddAuthorizationBuilder()
                    .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build())
                    .SetFallbackPolicy(null);
            }
        );

        base.ConfigureWebHost(builder);
    }
}