namespace ExchangeRateLib.Models
{
    public class ExchangeRatesResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? BaseCurrency { get; set; }
        public Dictionary<string, decimal> Rates { get; set; } = new();
        public DateTime? Date { get; set; }
        public DateTime? Timestamp { get; set; }
        public string? ErrorCode { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
