using UrlShortener.TokenRangeService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new TokenRangeManager(builder.Configuration["Postgres:ConnectionString"]!));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () => "Token Range Service");

app.MapPost("/assign", async (AssignTokenRangeRequest request, TokenRangeManager manager) =>
{
    var range = await manager.AssignRangeAsync(request.Key);

    return range;
});

app.Run();