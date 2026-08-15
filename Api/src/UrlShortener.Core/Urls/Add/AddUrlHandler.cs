namespace UrlShortener.Core.Urls.Add;

public class AddUrlHandler(ShortUrlGenerator shortUrlGenerator, IUrlDataStore urlDataStore, TimeProvider timeProvider)
{
    public async Task<AddUrlResponse> HandleAsync(AddUrlRequest request, CancellationToken cancellationToken)
    {
        var shortened = new ShortenedUrl(
            request.LongUrl, 
            shortUrlGenerator.GenerateUniqueUrl(), 
            request.CreatedBy,
            timeProvider.GetUtcNow());
        
        await urlDataStore.AddAsync(shortened, cancellationToken);
            
        return new AddUrlResponse(request.LongUrl, shortened.ShortUrl);
    }
}