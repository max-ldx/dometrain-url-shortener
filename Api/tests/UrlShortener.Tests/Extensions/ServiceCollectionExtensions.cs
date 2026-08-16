using Microsoft.Extensions.DependencyInjection;

namespace UrlShortener.Tests.Extensions;

public static class ServiceCollectionExtensions
{
    public static void Remove<T>(this IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(descriptor => descriptor.ServiceType == typeof(T));
        if (descriptor is not null) services.Remove(descriptor);
    }
}