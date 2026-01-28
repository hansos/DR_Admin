namespace ExchangeRateLib.Models
{
    public class TimeSeriesResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? BaseCurrency { get; set; }
        public string? TargetCurrency { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Dictionary<DateTime, decimal> Rates { get; set; } = new();
        public string? ErrorCode { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
