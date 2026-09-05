using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using UrlShortener.Api;
using UrlShortener.Api.Extensions;
using UrlShortener.Core.Urls.Add;
using UrlShortener.Core.Urls.List;
using UrlShortener.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

var keyVaultName = builder.Configuration["KeyVaultName"];
if (!string.IsNullOrEmpty(keyVaultName))
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential()
    );

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services
    .AddSingleton(TimeProvider.System)
    .AddSingleton<IEnvironmentManager, EnvironmentManager>();

builder.Services
    .AddUrlFeature()
    .AddListUrlsFeature()
    .AddCosmosUrlDataStore(builder.Configuration);

builder.Services.AddHttpClient("TokenRangeService",
    client => client.BaseAddress = new Uri(builder.Configuration["TokenRangeService:Endpoint"]!));

builder.Services.AddSingleton<ITokenRangeApiClient, TokenRangeApiClient>();
builder.Services.AddHostedService<TokenManager>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
        {
            builder.Configuration.Bind("AzureAd", options);
            options.TokenValidationParameters.NameClaimType = "name";
        },
        options => builder.Configuration.Bind("AzureAd", options));

builder.Services
    .AddAuthorizationBuilder()
    .AddPolicy("AuthZPolicy", policyBuilder =>
    {
        policyBuilder.Requirements.Add(new ScopeAuthorizationRequirement
        {
            RequiredScopesConfigurationKey = "AzureAd:Scopes"
        });
    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();

    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
    {
        if (builder.Configuration["WebAppEndpoints"] is null)
            return;

        var origins = builder.Configuration["WebAppEndpoints"]!.Split(",");

        policy
            .WithOrigins([.. origins])
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.UseCors("AllowWebApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "API").AllowAnonymous();

app.MapPost("/api/urls",
    async (AddUrlHandler handler, AddUrlRequest request, HttpContext context, CancellationToken cancellationToken) =>
    {
        var email = context.User.GetUserEmail();

        var requestWithUser = request with
        {
            CreatedBy = email
        };

        var result = await handler.HandleAsync(requestWithUser, cancellationToken);

        return !result.Succeeded
            ? Results.BadRequest(result.Error)
            : Results.Created($"/api/urls/{result.Value!.ShortUrl}", result.Value);
    });

app.MapGet("/api/urls",
    async (HttpContext context, ListUrlsHandler handler, int? pageSize, string? continuationToken,
        CancellationToken cancellationToken) =>
    {
        var email = context.User.GetUserEmail();

        var request = new ListUrlsRequest(email, pageSize, continuationToken);
        var urls = await handler.HandleAsync(request, cancellationToken);

        return urls;
    });

app.Run();