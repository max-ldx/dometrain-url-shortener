using Azure.Identity;
using Azure.Monitor.OpenTelemetry.Exporter;
using HealthChecks.CosmosDb;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using UrlShortener.RedirectApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var keyVaultName = builder.Configuration["KeyVaultName"];
if (!string.IsNullOrEmpty(keyVaultName))
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential()
    );

builder.Services.AddHealthChecks()
    .AddAzureCosmosDB(optionsFactory: _ => new AzureCosmosDbHealthCheckOptions()
    {
        DatabaseId = builder.Configuration["DatabaseName"]!
    })
    .AddRedis(provider => provider.GetRequiredService<IConnectionMultiplexer>(),
        failureStatus: HealthStatus.Degraded);

builder.Services.AddUrlReader(builder.Configuration["CosmosDb:ConnectionString"]!,
    builder.Configuration["DatabaseName"]!, builder.Configuration["ContainerName"]!,
    builder.Configuration["Redis:ConnectionString"]!);

var applicationName = builder.Environment.ApplicationName ?? "RedirectApi";

var telemetryConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName: applicationName))
    .WithTracing(tracing =>
    {
        tracing.AddSource("Azure.Cosmos.Operation");
        tracing.AddHttpClientInstrumentation();
        tracing.AddRedisInstrumentation();

        tracing.AddConsoleExporter();
        if (telemetryConnectionString is not null)
        {
            tracing.AddAzureMonitorTraceExporter(options => options.ConnectionString = telemetryConnectionString);
        }
    })
    .WithMetrics(metrics =>
    {
        metrics.AddConsoleExporter();
        if (telemetryConnectionString is not null)
        {
            metrics.AddAzureMonitorMetricExporter(options => options.ConnectionString = telemetryConnectionString);
        }
    })
    .WithLogging(logging =>
    {
        logging.AddConsoleExporter();
        if (telemetryConnectionString is not null)
        {
            logging.AddAzureMonitorLogExporter(options => options.ConnectionString = telemetryConnectionString);
        }
    });

var app = builder.Build();

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapGet("/", () => "Redirect API");

app.MapGet("/r/{shortUrl}",
    async (string shortUrl, IShortenedUrlReader reader, CancellationToken cancellationToken) =>
    {
        var response = await reader.GetLongUrlAsync(shortUrl, cancellationToken);

        return response switch
        {
            { Found: true, LongUrl: not null } => Results.Redirect(response.LongUrl, true),
            _ => Results.NotFound()
        };
    });

app.Run();