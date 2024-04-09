using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json.Nodes;

namespace AzureFunctionVSCS001
{
    public class GetCustomers
    {
        private readonly ILogger _logger;

        public GetCustomers(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetCustomers>();
        }

        [Function("GetCustomers")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation($"GetCustomers started at: {DateTime.UtcNow:o}");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var apiBaseUrl = Environment.GetEnvironmentVariable("NorthwindApiUrl");
                var endpoint = $"{apiBaseUrl}/Customers";

                using (var httpResponse = await client.GetAsync(endpoint))
                {
                    if (httpResponse.IsSuccessStatusCode == false) 
                    {
                        var message = $"GetCustomers: Call to service failed with status {httpResponse.StatusCode}";
                        _logger.LogError(message);
                        var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                        await errorResponse.WriteStringAsync($"Call to service failed with status {httpResponse.StatusCode}");
                        return errorResponse;
                    }

                    var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
                    var valueNode = JsonObject.Parse(jsonResponse)["value"];
                    var okResponse = req.CreateResponse(HttpStatusCode.OK);
                    await okResponse.WriteAsJsonAsync(valueNode.ToJsonString());

                    _logger.LogInformation($"GetCustomers completed at: {DateTime.UtcNow:o}");
                    return okResponse;
                }
            }
        }
    }
}
