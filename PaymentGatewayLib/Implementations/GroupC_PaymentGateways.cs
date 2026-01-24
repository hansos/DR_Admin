using PaymentGatewayLib.Interfaces;
using PaymentGatewayLib.Models;

namespace PaymentGatewayLib.Implementations
{
    public class ElavonPaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly string _merchantId;
        private readonly string _userId;
        public ElavonPaymentGateway(string merchantId, string userId, bool useTestMode = true) { _merchantId = merchantId ?? throw new ArgumentNullException(nameof(merchantId)); _userId = userId ?? throw new ArgumentNullException(nameof(userId)); }
        public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request) { try { ValidatePaymentRequest(request); return Task.FromResult(new PaymentResult { Success = true, TransactionId = Guid.NewGuid().ToString(), Status = "approved", Amount = request.Amount, Currency = request.Currency, ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(CreateErrorResult($"Error: {ex.Message}", "exception")); } }
        public Task<RefundResult> RefundPaymentAsync(RefundRequest request) { try { ValidateRefundRequest(request); return Task.FromResult(new RefundResult { Success = true, RefundId = Guid.NewGuid().ToString(), OriginalTransactionId = request.TransactionId, Status = "refunded", ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(new RefundResult { Success = false, ErrorMessage = ex.Message, ProcessedAt = DateTime.UtcNow }); } }
        public Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId) => Task.FromResult(new TransactionStatusResult { TransactionId = transactionId, Status = "unknown" });
        public Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request) { ValidatePaymentIntentRequest(request); return Task.FromResult(new PaymentIntentResult { Success = true, IntentId = Guid.NewGuid().ToString(), CreatedAt = DateTime.UtcNow }); }
        public Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null) => Task.FromResult(new CaptureResult { Success = true, TransactionId = authorizationId, ProcessedAt = DateTime.UtcNow });
        public Task<VoidResult> VoidPaymentAsync(string authorizationId) => Task.FromResult(new VoidResult { Success = true, AuthorizationId = authorizationId, Status = "voided", ProcessedAt = DateTime.UtcNow });
        public Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request) { ValidateCustomerProfileRequest(request); return Task.FromResult(new CustomerProfileResult { Success = true, CustomerId = Guid.NewGuid().ToString(), Email = request.Email, Name = request.Name, CreatedAt = DateTime.UtcNow }); }
        public Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate) => Task.FromResult(Enumerable.Empty<TransactionSummary>());
    }
    public class CybersourcePaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly string _merchantId;
        private readonly string _apiKeyId;
        public CybersourcePaymentGateway(string merchantId, string apiKeyId, bool useTestMode = true) { _merchantId = merchantId ?? throw new ArgumentNullException(nameof(merchantId)); _apiKeyId = apiKeyId ?? throw new ArgumentNullException(nameof(apiKeyId)); }
        public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request) { try { ValidatePaymentRequest(request); return Task.FromResult(new PaymentResult { Success = true, TransactionId = Guid.NewGuid().ToString(), Status = "AUTHORIZED", Amount = request.Amount, Currency = request.Currency, ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(CreateErrorResult($"Error: {ex.Message}", "exception")); } }
        public Task<RefundResult> RefundPaymentAsync(RefundRequest request) { try { ValidateRefundRequest(request); return Task.FromResult(new RefundResult { Success = true, RefundId = Guid.NewGuid().ToString(), OriginalTransactionId = request.TransactionId, Status = "refunded", ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(new RefundResult { Success = false, ErrorMessage = ex.Message, ProcessedAt = DateTime.UtcNow }); } }
        public Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId) => Task.FromResult(new TransactionStatusResult { TransactionId = transactionId, Status = "unknown" });
        public Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request) { ValidatePaymentIntentRequest(request); return Task.FromResult(new PaymentIntentResult { Success = true, IntentId = Guid.NewGuid().ToString(), CreatedAt = DateTime.UtcNow }); }
        public Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null) => Task.FromResult(new CaptureResult { Success = true, TransactionId = authorizationId, ProcessedAt = DateTime.UtcNow });
        public Task<VoidResult> VoidPaymentAsync(string authorizationId) => Task.FromResult(new VoidResult { Success = true, AuthorizationId = authorizationId, Status = "voided", ProcessedAt = DateTime.UtcNow });
        public Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request) { ValidateCustomerProfileRequest(request); return Task.FromResult(new CustomerProfileResult { Success = true, CustomerId = Guid.NewGuid().ToString(), Email = request.Email, Name = request.Name, CreatedAt = DateTime.UtcNow }); }
        public Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate) => Task.FromResult(Enumerable.Empty<TransactionSummary>());
    }
    public class BitPayPaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly string _apiToken;
        public BitPayPaymentGateway(string apiToken, bool useTestMode = true) { _apiToken = apiToken ?? throw new ArgumentNullException(nameof(apiToken)); }
        public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request) { try { ValidatePaymentRequest(request); return Task.FromResult(new PaymentResult { Success = true, TransactionId = Guid.NewGuid().ToString(), Status = "new", Amount = request.Amount, Currency = request.Currency, ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(CreateErrorResult($"Error: {ex.Message}", "exception")); } }
        public Task<RefundResult> RefundPaymentAsync(RefundRequest request) { try { ValidateRefundRequest(request); return Task.FromResult(new RefundResult { Success = true, RefundId = Guid.NewGuid().ToString(), OriginalTransactionId = request.TransactionId, Status = "refunded", ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(new RefundResult { Success = false, ErrorMessage = ex.Message, ProcessedAt = DateTime.UtcNow }); } }
        public Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId) => Task.FromResult(new TransactionStatusResult { TransactionId = transactionId, Status = "unknown" });
        public Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request) { ValidatePaymentIntentRequest(request); return Task.FromResult(new PaymentIntentResult { Success = true, IntentId = Guid.NewGuid().ToString(), CreatedAt = DateTime.UtcNow }); }
        public Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null) => Task.FromResult(new CaptureResult { Success = true, TransactionId = authorizationId, ProcessedAt = DateTime.UtcNow });
        public Task<VoidResult> VoidPaymentAsync(string authorizationId) => Task.FromResult(new VoidResult { Success = true, AuthorizationId = authorizationId, Status = "voided", ProcessedAt = DateTime.UtcNow });
        public Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request) { ValidateCustomerProfileRequest(request); return Task.FromResult(new CustomerProfileResult { Success = true, CustomerId = Guid.NewGuid().ToString(), Email = request.Email, Name = request.Name, CreatedAt = DateTime.UtcNow }); }
        public Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate) => Task.FromResult(Enumerable.Empty<TransactionSummary>());
    }
    public class OpenNodePaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly string _apiKey;
        public OpenNodePaymentGateway(string apiKey, bool useTestMode = true) { _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey)); }
        public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request) { try { ValidatePaymentRequest(request); return Task.FromResult(new PaymentResult { Success = true, TransactionId = Guid.NewGuid().ToString(), Status = "unpaid", Amount = request.Amount, Currency = request.Currency, ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(CreateErrorResult($"Error: {ex.Message}", "exception")); } }
        public Task<RefundResult> RefundPaymentAsync(RefundRequest request) { try { ValidateRefundRequest(request); return Task.FromResult(new RefundResult { Success = true, RefundId = Guid.NewGuid().ToString(), OriginalTransactionId = request.TransactionId, Status = "refunded", ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(new RefundResult { Success = false, ErrorMessage = ex.Message, ProcessedAt = DateTime.UtcNow }); } }
        public Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId) => Task.FromResult(new TransactionStatusResult { TransactionId = transactionId, Status = "unknown" });
        public Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request) { ValidatePaymentIntentRequest(request); return Task.FromResult(new PaymentIntentResult { Success = true, IntentId = Guid.NewGuid().ToString(), CreatedAt = DateTime.UtcNow }); }
        public Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null) => Task.FromResult(new CaptureResult { Success = true, TransactionId = authorizationId, ProcessedAt = DateTime.UtcNow });
        public Task<VoidResult> VoidPaymentAsync(string authorizationId) => Task.FromResult(new VoidResult { Success = true, AuthorizationId = authorizationId, Status = "voided", ProcessedAt = DateTime.UtcNow });
        public Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request) { ValidateCustomerProfileRequest(request); return Task.FromResult(new CustomerProfileResult { Success = true, CustomerId = Guid.NewGuid().ToString(), Email = request.Email, Name = request.Name, CreatedAt = DateTime.UtcNow }); }
        public Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate) => Task.FromResult(Enumerable.Empty<TransactionSummary>());
    }
}
