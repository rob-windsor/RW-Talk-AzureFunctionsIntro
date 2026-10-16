using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

public class ProcessOrder
{
    private readonly ILogger<ProcessOrder> _logger;

    public ProcessOrder(ILogger<ProcessOrder> logger)
    {
        _logger = logger;
    }

    [Function("ProcessOrder")]
    public void Run(
        [QueueTrigger(
            "orders",
            Connection = "AzureWebJobsStorage")] string message)
    {
        var order = JsonSerializer.Deserialize<Order>(message)
            ?? throw new InvalidOperationException(
                "Invalid order message.");

        _logger.LogInformation(
            "Processing order {OrderId} for customer {CustomerId}",
            order.OrderId,
            order.CustomerId);

        _logger.LogInformation(
            "Order {OrderId} processed successfully",
            order.OrderId);
    }
}