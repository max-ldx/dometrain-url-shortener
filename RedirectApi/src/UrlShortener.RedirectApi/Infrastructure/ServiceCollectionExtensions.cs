using Azure.Identity;
using Microsoft.Azure.Cosmos;
using StackExchange.Redis;

namespace UrlShortener.RedirectApi.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUrlReader(this IServiceCollection services,
        string cosmosConnectionString,
        string databaseName, string containerName,
        string redisConnectionString)
    {
        services.AddSingleton<CosmosClient>(_ => new CosmosClient(cosmosConnectionString));

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var options = ConfigurationOptions.Parse(redisConnectionString);
            options.ConfigureForAzureWithTokenCredentialAsync(new DefaultAzureCredential()).GetAwaiter().GetResult();

            return ConnectionMultiplexer.Connect(options);
        });

        services.AddSingleton<IShortenedUrlReader>(s =>
        {
            var cosmosClient = s.GetRequiredService<CosmosClient>();
            var container = cosmosClient.GetContainer(databaseName, containerName);
            var connectionMultiplexer = s.GetRequiredService<IConnectionMultiplexer>();

            return new RedisUrlReader(new CosmosShortenedUrlReader(container), connectionMultiplexer,
                s.GetRequiredService<ILogger<RedisUrlReader>>());
        });

        return services;
    }
}