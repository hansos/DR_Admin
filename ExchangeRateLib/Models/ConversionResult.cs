namespace ExchangeRateLib.Models
{
    public class ConversionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? FromCurrency { get; set; }
        public string? ToCurrency { get; set; }
        public decimal? FromAmount { get; set; }
        public decimal? ToAmount { get; set; }
        public decimal? Rate { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Timestamp { get; set; }
        public string? ErrorCode { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
