using PaymentGatewayLib.Interfaces;
using PaymentGatewayLib.Models;

namespace PaymentGatewayLib.Implementations
{
    public class MpesaPaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly string _shortcode;
        private readonly string _passkey;
        public MpesaPaymentGateway(string consumerKey, string consumerSecret, string shortcode, string passkey, bool useSandbox = true) { _consumerKey = consumerKey ?? throw new ArgumentNullException(nameof(consumerKey)); _consumerSecret = consumerSecret ?? throw new ArgumentNullException(nameof(consumerSecret)); _shortcode = shortcode ?? throw new ArgumentNullException(nameof(shortcode)); _passkey = passkey ?? throw new ArgumentNullException(nameof(passkey)); }
        public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request) { try { ValidatePaymentRequest(request); return Task.FromResult(new PaymentResult { Success = true, TransactionId = Guid.NewGuid().ToString(), Status = "Completed", Amount = request.Amount, Currency = request.Currency, ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(CreateErrorResult($"M-Pesa Error: {ex.Message}", "exception")); } }
        public Task<RefundResult> RefundPaymentAsync(RefundRequest request) { try { ValidateRefundRequest(request); return Task.FromResult(new RefundResult { Success = true, RefundId = Guid.NewGuid().ToString(), OriginalTransactionId = request.TransactionId, Status = "refunded", ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(new RefundResult { Success = false, ErrorMessage = ex.Message, ProcessedAt = DateTime.UtcNow }); } }
        public Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId) => Task.FromResult(new TransactionStatusResult { TransactionId = transactionId, Status = "unknown" });
        public Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request) { ValidatePaymentIntentRequest(request); return Task.FromResult(new PaymentIntentResult { Success = true, IntentId = Guid.NewGuid().ToString(), CreatedAt = DateTime.UtcNow }); }
        public Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null) => Task.FromResult(new CaptureResult { Success = true, TransactionId = authorizationId, ProcessedAt = DateTime.UtcNow });
        public Task<VoidResult> VoidPaymentAsync(string authorizationId) => Task.FromResult(new VoidResult { Success = true, AuthorizationId = authorizationId, Status = "voided", ProcessedAt = DateTime.UtcNow });
        public Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request) { ValidateCustomerProfileRequest(request); return Task.FromResult(new CustomerProfileResult { Success = true, CustomerId = Guid.NewGuid().ToString(), Email = request.Email, Name = request.Name, CreatedAt = DateTime.UtcNow }); }
        public Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate) => Task.FromResult(Enumerable.Empty<TransactionSummary>());
    }
    public class FlutterwavePaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly string _publicKey;
        private readonly string _secretKey;
        private readonly string _encryptionKey;
        public FlutterwavePaymentGateway(string publicKey, string secretKey, string encryptionKey, bool useTestMode = true) { _publicKey = publicKey ?? throw new ArgumentNullException(nameof(publicKey)); _secretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey)); _encryptionKey = encryptionKey ?? throw new ArgumentNullException(nameof(encryptionKey)); }
        public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request) { try { ValidatePaymentRequest(request); return Task.FromResult(new PaymentResult { Success = true, TransactionId = Guid.NewGuid().ToString(), Status = "successful", Amount = request.Amount, Currency = request.Currency, ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(CreateErrorResult($"Flutterwave Error: {ex.Message}", "exception")); } }
        public Task<RefundResult> RefundPaymentAsync(RefundRequest request) { try { ValidateRefundRequest(request); return Task.FromResult(new RefundResult { Success = true, RefundId = Guid.NewGuid().ToString(), OriginalTransactionId = request.TransactionId, Status = "refunded", ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(new RefundResult { Success = false, ErrorMessage = ex.Message, ProcessedAt = DateTime.UtcNow }); } }
        public Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId) => Task.FromResult(new TransactionStatusResult { TransactionId = transactionId, Status = "unknown" });
        public Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request) { ValidatePaymentIntentRequest(request); return Task.FromResult(new PaymentIntentResult { Success = true, IntentId = Guid.NewGuid().ToString(), CreatedAt = DateTime.UtcNow }); }
        public Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null) => Task.FromResult(new CaptureResult { Success = true, TransactionId = authorizationId, ProcessedAt = DateTime.UtcNow });
        public Task<VoidResult> VoidPaymentAsync(string authorizationId) => Task.FromResult(new VoidResult { Success = true, AuthorizationId = authorizationId, Status = "voided", ProcessedAt = DateTime.UtcNow });
        public Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request) { ValidateCustomerProfileRequest(request); return Task.FromResult(new CustomerProfileResult { Success = true, CustomerId = Guid.NewGuid().ToString(), Email = request.Email, Name = request.Name, CreatedAt = DateTime.UtcNow }); }
        public Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate) => Task.FromResult(Enumerable.Empty<TransactionSummary>());
    }
    public class PesapalPaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        public PesapalPaymentGateway(string consumerKey, string consumerSecret, bool useSandbox = true) { _consumerKey = consumerKey ?? throw new ArgumentNullException(nameof(consumerKey)); _consumerSecret = consumerSecret ?? throw new ArgumentNullException(nameof(consumerSecret)); }
        public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request) { try { ValidatePaymentRequest(request); return Task.FromResult(new PaymentResult { Success = true, TransactionId = Guid.NewGuid().ToString(), Status = "COMPLETED", Amount = request.Amount, Currency = request.Currency, ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(CreateErrorResult($"Pesapal Error: {ex.Message}", "exception")); } }
        public Task<RefundResult> RefundPaymentAsync(RefundRequest request) { try { ValidateRefundRequest(request); return Task.FromResult(new RefundResult { Success = true, RefundId = Guid.NewGuid().ToString(), OriginalTransactionId = request.TransactionId, Status = "refunded", ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(new RefundResult { Success = false, ErrorMessage = ex.Message, ProcessedAt = DateTime.UtcNow }); } }
        public Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId) => Task.FromResult(new TransactionStatusResult { TransactionId = transactionId, Status = "unknown" });
        public Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request) { ValidatePaymentIntentRequest(request); return Task.FromResult(new PaymentIntentResult { Success = true, IntentId = Guid.NewGuid().ToString(), CreatedAt = DateTime.UtcNow }); }
        public Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null) => Task.FromResult(new CaptureResult { Success = true, TransactionId = authorizationId, ProcessedAt = DateTime.UtcNow });
        public Task<VoidResult> VoidPaymentAsync(string authorizationId) => Task.FromResult(new VoidResult { Success = true, AuthorizationId = authorizationId, Status = "voided", ProcessedAt = DateTime.UtcNow });
        public Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request) { ValidateCustomerProfileRequest(request); return Task.FromResult(new CustomerProfileResult { Success = true, CustomerId = Guid.NewGuid().ToString(), Email = request.Email, Name = request.Name, CreatedAt = DateTime.UtcNow }); }
        public Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate) => Task.FromResult(Enumerable.Empty<TransactionSummary>());
    }
    public class PaystackPaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly string _publicKey;
        private readonly string _secretKey;
        public PaystackPaymentGateway(string publicKey, string secretKey, bool useTestMode = true) { _publicKey = publicKey ?? throw new ArgumentNullException(nameof(publicKey)); _secretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey)); }
        public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request) { try { ValidatePaymentRequest(request); return Task.FromResult(new PaymentResult { Success = true, TransactionId = Guid.NewGuid().ToString(), Status = "success", Amount = request.Amount, Currency = request.Currency, ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(CreateErrorResult($"Paystack Error: {ex.Message}", "exception")); } }
        public Task<RefundResult> RefundPaymentAsync(RefundRequest request) { try { ValidateRefundRequest(request); return Task.FromResult(new RefundResult { Success = true, RefundId = Guid.NewGuid().ToString(), OriginalTransactionId = request.TransactionId, Status = "refunded", ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(new RefundResult { Success = false, ErrorMessage = ex.Message, ProcessedAt = DateTime.UtcNow }); } }
        public Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId) => Task.FromResult(new TransactionStatusResult { TransactionId = transactionId, Status = "unknown" });
        public Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request) { ValidatePaymentIntentRequest(request); return Task.FromResult(new PaymentIntentResult { Success = true, IntentId = Guid.NewGuid().ToString(), CreatedAt = DateTime.UtcNow }); }
        public Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null) => Task.FromResult(new CaptureResult { Success = true, TransactionId = authorizationId, ProcessedAt = DateTime.UtcNow });
        public Task<VoidResult> VoidPaymentAsync(string authorizationId) => Task.FromResult(new VoidResult { Success = true, AuthorizationId = authorizationId, Status = "voided", ProcessedAt = DateTime.UtcNow });
        public Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request) { ValidateCustomerProfileRequest(request); return Task.FromResult(new CustomerProfileResult { Success = true, CustomerId = Guid.NewGuid().ToString(), Email = request.Email, Name = request.Name, CreatedAt = DateTime.UtcNow }); }
        public Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate) => Task.FromResult(Enumerable.Empty<TransactionSummary>());
    }
    public class IPayAfricaPaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly string _vendorId;
        private readonly string _hashKey;
        public IPayAfricaPaymentGateway(string vendorId, string hashKey, bool useTestMode = true) { _vendorId = vendorId ?? throw new ArgumentNullException(nameof(vendorId)); _hashKey = hashKey ?? throw new ArgumentNullException(nameof(hashKey)); }
        public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request) { try { ValidatePaymentRequest(request); return Task.FromResult(new PaymentResult { Success = true, TransactionId = Guid.NewGuid().ToString(), Status = "aei7p7yrx4ae34", Amount = request.Amount, Currency = request.Currency, ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(CreateErrorResult($"iPay Africa Error: {ex.Message}", "exception")); } }
        public Task<RefundResult> RefundPaymentAsync(RefundRequest request) { try { ValidateRefundRequest(request); return Task.FromResult(new RefundResult { Success = true, RefundId = Guid.NewGuid().ToString(), OriginalTransactionId = request.TransactionId, Status = "refunded", ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(new RefundResult { Success = false, ErrorMessage = ex.Message, ProcessedAt = DateTime.UtcNow }); } }
        public Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId) => Task.FromResult(new TransactionStatusResult { TransactionId = transactionId, Status = "unknown" });
        public Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request) { ValidatePaymentIntentRequest(request); return Task.FromResult(new PaymentIntentResult { Success = true, IntentId = Guid.NewGuid().ToString(), CreatedAt = DateTime.UtcNow }); }
        public Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null) => Task.FromResult(new CaptureResult { Success = true, TransactionId = authorizationId, ProcessedAt = DateTime.UtcNow });
        public Task<VoidResult> VoidPaymentAsync(string authorizationId) => Task.FromResult(new VoidResult { Success = true, AuthorizationId = authorizationId, Status = "voided", ProcessedAt = DateTime.UtcNow });
        public Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request) { ValidateCustomerProfileRequest(request); return Task.FromResult(new CustomerProfileResult { Success = true, CustomerId = Guid.NewGuid().ToString(), Email = request.Email, Name = request.Name, CreatedAt = DateTime.UtcNow }); }
        public Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate) => Task.FromResult(Enumerable.Empty<TransactionSummary>());
    }
    public class DpoGroupPaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly string _companyToken;
        private readonly string _serviceType;
        public DpoGroupPaymentGateway(string companyToken, string serviceType, bool useTestMode = true) { _companyToken = companyToken ?? throw new ArgumentNullException(nameof(companyToken)); _serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType)); }
        public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request) { try { ValidatePaymentRequest(request); return Task.FromResult(new PaymentResult { Success = true, TransactionId = Guid.NewGuid().ToString(), Status = "Approved", Amount = request.Amount, Currency = request.Currency, ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(CreateErrorResult($"DPO Group Error: {ex.Message}", "exception")); } }
        public Task<RefundResult> RefundPaymentAsync(RefundRequest request) { try { ValidateRefundRequest(request); return Task.FromResult(new RefundResult { Success = true, RefundId = Guid.NewGuid().ToString(), OriginalTransactionId = request.TransactionId, Status = "refunded", ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(new RefundResult { Success = false, ErrorMessage = ex.Message, ProcessedAt = DateTime.UtcNow }); } }
        public Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId) => Task.FromResult(new TransactionStatusResult { TransactionId = transactionId, Status = "unknown" });
        public Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request) { ValidatePaymentIntentRequest(request); return Task.FromResult(new PaymentIntentResult { Success = true, IntentId = Guid.NewGuid().ToString(), CreatedAt = DateTime.UtcNow }); }
        public Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null) => Task.FromResult(new CaptureResult { Success = true, TransactionId = authorizationId, ProcessedAt = DateTime.UtcNow });
        public Task<VoidResult> VoidPaymentAsync(string authorizationId) => Task.FromResult(new VoidResult { Success = true, AuthorizationId = authorizationId, Status = "voided", ProcessedAt = DateTime.UtcNow });
        public Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request) { ValidateCustomerProfileRequest(request); return Task.FromResult(new CustomerProfileResult { Success = true, CustomerId = Guid.NewGuid().ToString(), Email = request.Email, Name = request.Name, CreatedAt = DateTime.UtcNow }); }
        public Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate) => Task.FromResult(Enumerable.Empty<TransactionSummary>());
    }
    public class IntaSendPaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly string _publishableKey;
        private readonly string _secretKey;
        public IntaSendPaymentGateway(string publishableKey, string secretKey, bool useTestMode = true) { _publishableKey = publishableKey ?? throw new ArgumentNullException(nameof(publishableKey)); _secretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey)); }
        public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request) { try { ValidatePaymentRequest(request); return Task.FromResult(new PaymentResult { Success = true, TransactionId = Guid.NewGuid().ToString(), Status = "COMPLETE", Amount = request.Amount, Currency = request.Currency, ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(CreateErrorResult($"IntaSend Error: {ex.Message}", "exception")); } }
        public Task<RefundResult> RefundPaymentAsync(RefundRequest request) { try { ValidateRefundRequest(request); return Task.FromResult(new RefundResult { Success = true, RefundId = Guid.NewGuid().ToString(), OriginalTransactionId = request.TransactionId, Status = "refunded", ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(new RefundResult { Success = false, ErrorMessage = ex.Message, ProcessedAt = DateTime.UtcNow }); } }
        public Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId) => Task.FromResult(new TransactionStatusResult { TransactionId = transactionId, Status = "unknown" });
        public Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request) { ValidatePaymentIntentRequest(request); return Task.FromResult(new PaymentIntentResult { Success = true, IntentId = Guid.NewGuid().ToString(), CreatedAt = DateTime.UtcNow }); }
        public Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null) => Task.FromResult(new CaptureResult { Success = true, TransactionId = authorizationId, ProcessedAt = DateTime.UtcNow });
        public Task<VoidResult> VoidPaymentAsync(string authorizationId) => Task.FromResult(new VoidResult { Success = true, AuthorizationId = authorizationId, Status = "voided", ProcessedAt = DateTime.UtcNow });
        public Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request) { ValidateCustomerProfileRequest(request); return Task.FromResult(new CustomerProfileResult { Success = true, CustomerId = Guid.NewGuid().ToString(), Email = request.Email, Name = request.Name, CreatedAt = DateTime.UtcNow }); }
        public Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate) => Task.FromResult(Enumerable.Empty<TransactionSummary>());
    }
    public class JamboPayPaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly string _merchantId;
        private readonly string _apiKey;
        public JamboPayPaymentGateway(string merchantId, string apiKey, bool useTestMode = true) { _merchantId = merchantId ?? throw new ArgumentNullException(nameof(merchantId)); _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey)); }
        public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request) { try { ValidatePaymentRequest(request); return Task.FromResult(new PaymentResult { Success = true, TransactionId = Guid.NewGuid().ToString(), Status = "Successful", Amount = request.Amount, Currency = request.Currency, ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(CreateErrorResult($"JamboPay Error: {ex.Message}", "exception")); } }
        public Task<RefundResult> RefundPaymentAsync(RefundRequest request) { try { ValidateRefundRequest(request); return Task.FromResult(new RefundResult { Success = true, RefundId = Guid.NewGuid().ToString(), OriginalTransactionId = request.TransactionId, Status = "refunded", ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(new RefundResult { Success = false, ErrorMessage = ex.Message, ProcessedAt = DateTime.UtcNow }); } }
        public Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId) => Task.FromResult(new TransactionStatusResult { TransactionId = transactionId, Status = "unknown" });
        public Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request) { ValidatePaymentIntentRequest(request); return Task.FromResult(new PaymentIntentResult { Success = true, IntentId = Guid.NewGuid().ToString(), CreatedAt = DateTime.UtcNow }); }
        public Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null) => Task.FromResult(new CaptureResult { Success = true, TransactionId = authorizationId, ProcessedAt = DateTime.UtcNow });
        public Task<VoidResult> VoidPaymentAsync(string authorizationId) => Task.FromResult(new VoidResult { Success = true, AuthorizationId = authorizationId, Status = "voided", ProcessedAt = DateTime.UtcNow });
        public Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request) { ValidateCustomerProfileRequest(request); return Task.FromResult(new CustomerProfileResult { Success = true, CustomerId = Guid.NewGuid().ToString(), Email = request.Email, Name = request.Name, CreatedAt = DateTime.UtcNow }); }
        public Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate) => Task.FromResult(Enumerable.Empty<TransactionSummary>());
    }
    public class KopoKopoPaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _apiKey;
        public KopoKopoPaymentGateway(string clientId, string clientSecret, string apiKey, bool useSandbox = true) { _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId)); _clientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret)); _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey)); }
        public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request) { try { ValidatePaymentRequest(request); return Task.FromResult(new PaymentResult { Success = true, TransactionId = Guid.NewGuid().ToString(), Status = "Success", Amount = request.Amount, Currency = request.Currency, ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(CreateErrorResult($"KopoKopo Error: {ex.Message}", "exception")); } }
        public Task<RefundResult> RefundPaymentAsync(RefundRequest request) { try { ValidateRefundRequest(request); return Task.FromResult(new RefundResult { Success = true, RefundId = Guid.NewGuid().ToString(), OriginalTransactionId = request.TransactionId, Status = "refunded", ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(new RefundResult { Success = false, ErrorMessage = ex.Message, ProcessedAt = DateTime.UtcNow }); } }
        public Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId) => Task.FromResult(new TransactionStatusResult { TransactionId = transactionId, Status = "unknown" });
        public Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request) { ValidatePaymentIntentRequest(request); return Task.FromResult(new PaymentIntentResult { Success = true, IntentId = Guid.NewGuid().ToString(), CreatedAt = DateTime.UtcNow }); }
        public Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null) => Task.FromResult(new CaptureResult { Success = true, TransactionId = authorizationId, ProcessedAt = DateTime.UtcNow });
        public Task<VoidResult> VoidPaymentAsync(string authorizationId) => Task.FromResult(new VoidResult { Success = true, AuthorizationId = authorizationId, Status = "voided", ProcessedAt = DateTime.UtcNow });
        public Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request) { ValidateCustomerProfileRequest(request); return Task.FromResult(new CustomerProfileResult { Success = true, CustomerId = Guid.NewGuid().ToString(), Email = request.Email, Name = request.Name, CreatedAt = DateTime.UtcNow }); }
        public Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate) => Task.FromResult(Enumerable.Empty<TransactionSummary>());
    }
    public class AfricasTalkingPaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly string _username;
        private readonly string _apiKey;
        private readonly string _productName;
        public AfricasTalkingPaymentGateway(string username, string apiKey, string productName, bool useSandbox = true) { _username = username ?? throw new ArgumentNullException(nameof(username)); _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey)); _productName = productName ?? throw new ArgumentNullException(nameof(productName)); }
        public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request) { try { ValidatePaymentRequest(request); return Task.FromResult(new PaymentResult { Success = true, TransactionId = Guid.NewGuid().ToString(), Status = "Success", Amount = request.Amount, Currency = request.Currency, ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(CreateErrorResult($"Africa's Talking Error: {ex.Message}", "exception")); } }
        public Task<RefundResult> RefundPaymentAsync(RefundRequest request) { try { ValidateRefundRequest(request); return Task.FromResult(new RefundResult { Success = true, RefundId = Guid.NewGuid().ToString(), OriginalTransactionId = request.TransactionId, Status = "refunded", ProcessedAt = DateTime.UtcNow }); } catch (Exception ex) { return Task.FromResult(new RefundResult { Success = false, ErrorMessage = ex.Message, ProcessedAt = DateTime.UtcNow }); } }
        public Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId) => Task.FromResult(new TransactionStatusResult { TransactionId = transactionId, Status = "unknown" });
        public Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request) { ValidatePaymentIntentRequest(request); return Task.FromResult(new PaymentIntentResult { Success = true, IntentId = Guid.NewGuid().ToString(), CreatedAt = DateTime.UtcNow }); }
        public Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null) => Task.FromResult(new CaptureResult { Success = true, TransactionId = authorizationId, ProcessedAt = DateTime.UtcNow });
        public Task<VoidResult> VoidPaymentAsync(string authorizationId) => Task.FromResult(new VoidResult { Success = true, AuthorizationId = authorizationId, Status = "voided", ProcessedAt = DateTime.UtcNow });
        public Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request) { ValidateCustomerProfileRequest(request); return Task.FromResult(new CustomerProfileResult { Success = true, CustomerId = Guid.NewGuid().ToString(), Email = request.Email, Name = request.Name, CreatedAt = DateTime.UtcNow }); }
        public Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate) => Task.FromResult(Enumerable.Empty<TransactionSummary>());
    }
}
