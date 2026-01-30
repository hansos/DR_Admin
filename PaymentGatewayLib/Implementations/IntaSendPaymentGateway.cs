using PaymentGatewayLib.Interfaces;
using PaymentGatewayLib.Models;
using Serilog;

namespace PaymentGatewayLib.Implementations
{
    public class IntaSendPaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly ILogger _logger;
        public IntaSendPaymentGateway(string publishableKey, string secretKey, bool useTestMode) { _logger = Log.ForContext<IntaSendPaymentGateway>(); }

        public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
            => Task.FromResult(new PaymentResult { Success = false, ErrorMessage = "IntaSend gateway not implemented in this build", ProcessedAt = DateTime.UtcNow });

        public Task<RefundResult> RefundPaymentAsync(RefundRequest request)
            => Task.FromResult(new RefundResult { Success = false, ErrorMessage = "IntaSend refund not implemented", ProcessedAt = DateTime.UtcNow });

        public Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId)
            => Task.FromResult(new TransactionStatusResult { TransactionId = transactionId, Status = "unknown" });

        public Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request)
            => Task.FromResult(new PaymentIntentResult { Success = false, ErrorMessage = "Not implemented", CreatedAt = DateTime.UtcNow });

        public Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null)
            => Task.FromResult(new CaptureResult { Success = false, ErrorMessage = "Not implemented", ProcessedAt = DateTime.UtcNow });

        public Task<VoidResult> VoidPaymentAsync(string authorizationId)
            => Task.FromResult(new VoidResult { Success = false, ErrorMessage = "Not implemented", ProcessedAt = DateTime.UtcNow });

        public Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request)
            => Task.FromResult(new CustomerProfileResult { Success = false, ErrorMessage = "Not implemented", CreatedAt = DateTime.UtcNow });

        public Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate)
            => Task.FromResult(Enumerable.Empty<TransactionSummary>());
    }
}
