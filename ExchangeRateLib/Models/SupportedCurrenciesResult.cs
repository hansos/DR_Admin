namespace ExchangeRateLib.Models
{
    public class SupportedCurrenciesResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, string> Currencies { get; set; } = new();
        public string? ErrorCode { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
