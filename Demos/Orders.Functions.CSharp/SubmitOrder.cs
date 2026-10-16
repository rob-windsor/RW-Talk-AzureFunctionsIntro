using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

public class SubmitOrder
{
    private readonly ILogger<SubmitOrder> _logger;

    public SubmitOrder(ILogger<SubmitOrder> logger)
    {
        _logger = logger;
    }

    [Function("SubmitOrder")]
    public async Task<SubmitOrderResult> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "post",
            Route = "orders")] HttpRequest req)
    {
        Order? order;

        try
        {
            order = await JsonSerializer.DeserializeAsync<Order>(
                req.Body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch (JsonException)
        {
            return new SubmitOrderResult
            {
                Response = new BadRequestObjectResult(
                    "Invalid JSON request.")
            };
        }

        if (order is null ||
            string.IsNullOrWhiteSpace(order.OrderId) ||
            string.IsNullOrWhiteSpace(order.CustomerId) ||
            order.Amount <= 0)
        {
            return new SubmitOrderResult
            {
                Response = new BadRequestObjectResult(
                    "OrderId, CustomerId, and a positive Amount are required.")
            };
        }

        _logger.LogInformation(
            "Queuing order {OrderId}",
            order.OrderId);

        return new SubmitOrderResult
        {
            Response = new AcceptedResult(
                location: null,
                value: new
                {
                    orderId = order.OrderId,
                    status = "Queued"
                }),
            QueueMessage = JsonSerializer.Serialize(order)
        };
    }
}