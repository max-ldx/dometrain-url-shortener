using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UrlShortener.CosmosDbTriggerFunction;

public class HealthCheck(HealthCheckService healthCheck)
{
    [Function(nameof(HealthCheck))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "healthz")]
        HttpRequestData req,
        FunctionContext context)
    {
        var healthStatus = await healthCheck.CheckHealthAsync();
        return new OkObjectResult(Enum.GetName(healthStatus.Status));
    }
}