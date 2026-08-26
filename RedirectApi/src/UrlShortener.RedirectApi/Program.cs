using Azure.Identity;
using UrlShortener.RedirectApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var keyVaultName = builder.Configuration["KeyVaultName"];
if (!string.IsNullOrEmpty(keyVaultName))
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential()
    );

builder.Services.AddUrlReader(builder.Configuration["CosmosDb:ConnectionString"]!,
    builder.Configuration["DatabaseName"]!, builder.Configuration["ContainerName"]!,
    builder.Configuration["Redis:ConnectionString"]!);

var app = builder.Build();

app.MapGet("/", () => "Redirect API");

app.MapGet("r/{shortUrl}",
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