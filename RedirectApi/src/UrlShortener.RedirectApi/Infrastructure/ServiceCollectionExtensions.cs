using Microsoft.Azure.Cosmos;

namespace UrlShortener.RedirectApi.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUrlReader(this IServiceCollection services,
        string cosmosConnectionString,
        string databaseName, string containerName)
    {
        services.AddSingleton<CosmosClient>(s =>
            new CosmosClient(cosmosConnectionString));

        services.AddSingleton<IShortenedUrlReader>(s =>
        {
            var cosmosClient = s.GetRequiredService<CosmosClient>();
            var container = cosmosClient.GetContainer(databaseName, containerName);

            return new CosmosShortenedUrlReader(container);
        });

        return services;
    }
}