public sealed record Order(
    string? OrderId,
    string? CustomerId,
    decimal Amount);