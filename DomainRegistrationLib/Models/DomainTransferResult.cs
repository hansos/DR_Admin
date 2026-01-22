namespace DomainRegistrationLib.Models
{
    public class DomainTransferResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? DomainName { get; set; }
        public string? OrderId { get; set; }
        public string? TransactionId { get; set; }
        public string? TransferStatus { get; set; }
        public decimal? TotalCost { get; set; }
        public string? ErrorCode { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
