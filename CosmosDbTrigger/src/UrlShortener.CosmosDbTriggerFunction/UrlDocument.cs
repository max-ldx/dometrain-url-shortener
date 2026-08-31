namespace UrlShortener.CosmosDbTriggerFunction;

public class UrlDocument
{
    public string Id { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public string LongUrl { get; set; }
}