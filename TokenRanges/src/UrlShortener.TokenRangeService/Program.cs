using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using UrlShortener.TokenRangeService;

var builder = WebApplication.CreateBuilder(args);

var keyVaultName = builder.Configuration["KeyVaultName"];
if (!string.IsNullOrEmpty(keyVaultName))
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential()
    );

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration["Postgres:ConnectionString"]!);

builder.Services.AddSingleton(new TokenRangeManager(builder.Configuration["Postgres:ConnectionString"]!));

var applicationInsightsConnectionString =
    builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];

var openTelemetry = builder.Services.AddOpenTelemetry();

if (!string.IsNullOrWhiteSpace(applicationInsightsConnectionString))
{
    openTelemetry.UseAzureMonitor(options =>
        options.ConnectionString = applicationInsightsConnectionString);
}

var app = builder.Build();

app.UseHttpsRedirection();

app.MapHealthChecks("/healthz");

app.MapGet("/", () => "Token Range Service");

app.MapPost("/assign", async (AssignTokenRangeRequest request, TokenRangeManager manager) =>
{
    var range = await manager.AssignRangeAsync(request.Key);

    return range;
});

app.Run();