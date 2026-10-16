using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

public sealed class SubmitOrderResult
{
    [HttpResult]
    public IActionResult Response { get; set; } = default!;

    [QueueOutput(
        "orders",
        Connection = "AzureWebJobsStorage")]
    public string? QueueMessage { get; set; }
}