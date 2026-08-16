using Azure.Identity;
using UrlShortener.Api.Extensions;
using UrlShortener.Core.Urls.Add;

var builder = WebApplication.CreateBuilder(args);

var keyVaultName = builder.Configuration["KeyVaultName"];
if (!string.IsNullOrEmpty(keyVaultName))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential()
    );
}

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddUrlFeature();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/api/urls",
    async (AddUrlHandler handler, AddUrlRequest request, CancellationToken cancellationToken) =>
    {
        var requestWithUser = request with
        {
            CreatedBy = "max@test.com"
        };

        var result = await handler.HandleAsync(requestWithUser, cancellationToken);

        return !result.Succeeded
            ? Results.BadRequest(result.Error)
            : Results.Created($"/api/urls/{result.Value!.ShortUrl}", result.Value);
    });

app.Run();