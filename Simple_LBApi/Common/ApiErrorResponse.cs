namespace Simple_LBApi.Common
{
    public sealed class ApiErrorResponse
    {
        public required string Message { get; init; }
        public int StatusCode { get; init; }
        public DateTime TimestampUtc { get; init; } = DateTime.UtcNow;
        public string? TraceId { get; init; }
    }
}
