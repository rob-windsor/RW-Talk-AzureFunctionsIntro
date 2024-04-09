using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AzureFunctionVSCS001
{
    public class HelloWorld
    {
        private readonly ILogger _logger;

        public HelloWorld(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HelloWorld>();
        }

        [Function("HelloWorld")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        string? name)
        {
            _logger.LogInformation($"HelloWorld started at: {DateTime.UtcNow:o}");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            name = name ?? "world";
            await response.WriteStringAsync($"Hello, {name}");

            _logger.LogInformation($"HelloWorld completed at: {DateTime.UtcNow:o}");
            return response;
        }
    }
}
