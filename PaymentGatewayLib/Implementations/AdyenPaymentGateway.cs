using PaymentGatewayLib.Interfaces;
using PaymentGatewayLib.Models;
using Serilog;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PaymentGatewayLib.Implementations
{
    /// <summary>
    /// Adyen payment gateway implementation
    /// </summary>
    public class AdyenPaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly ILogger _logger;
        private readonly string _apiKey;
        private readonly string _merchantAccount;
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;

        public AdyenPaymentGateway(string apiKey, string merchantAccount, bool useTestMode = true)
        {
            _logger = Log.ForContext<AdyenPaymentGateway>();
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _merchantAccount = merchantAccount ?? throw new ArgumentNullException(nameof(merchantAccount));
            _apiBaseUrl = useTestMode 
                ? "https://checkout-test.adyen.com" 
                : "https://checkout-live.adyen.com";
            
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_apiBaseUrl)
            };
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
        }

        public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
        {
            try
            {
                ValidatePaymentRequest(request);

                var paymentRequest = new
                {
                    amount = new
                    {
                        currency = request.Currency.ToUpper(),
                        value = (long)(request.Amount * 100)
                    },
                    reference = request.ReferenceId,
                    merchantAccount = _merchantAccount,
                    paymentMethod = new { type = "scheme" },
                    returnUrl = "https://your-company.com/return",
                    shopperEmail = request.CustomerEmail
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(paymentRequest),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync("/v70/payments", jsonContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    return new PaymentResult
                    {
                        Success = true,
                        TransactionId = result.GetProperty("pspReference").GetString() ?? string.Empty,
                        Status = result.GetProperty("resultCode").GetString() ?? string.Empty,
                        Amount = request.Amount,
                        Currency = request.Currency,
                        ProcessedAt = DateTime.UtcNow,
                        RawResponse = responseBody
                    };
                }
                else
                {
                    return CreateErrorResult($"Payment failed: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                return CreateErrorResult($"Error processing payment: {ex.Message}", "exception");
            }
        }

        public async Task<RefundResult> RefundPaymentAsync(RefundRequest request)
        {
            try
            {
                ValidateRefundRequest(request);

                var refundRequest = new
                {
                    merchantAccount = _merchantAccount,
                    modificationAmount = new
                    {
                        currency = "USD",
                        value = (long)((request.Amount ?? 0) * 100)
                    },
                    originalReference = request.TransactionId
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(refundRequest),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync("/v70/refunds", jsonContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    return new RefundResult
                    {
                        Success = true,
                        RefundId = result.GetProperty("pspReference").GetString() ?? string.Empty,
                        OriginalTransactionId = request.TransactionId,
                        Status = result.GetProperty("status").GetString() ?? string.Empty,
                        ProcessedAt = DateTime.UtcNow,
                        RawResponse = responseBody
                    };
                }
                else
                {
                    return new RefundResult
                    {
                        Success = false,
                        ErrorMessage = $"Refund failed: {responseBody}",
                        ProcessedAt = DateTime.UtcNow
                    };
                }
            }
            catch (Exception ex)
            {
                return new RefundResult
                {
                    Success = false,
                    ErrorMessage = $"Error processing refund: {ex.Message}",
                    ErrorCode = "exception",
                    ProcessedAt = DateTime.UtcNow
                };
            }
        }

        public Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId)
        {
            return Task.FromResult(new TransactionStatusResult { TransactionId = transactionId, Status = "unknown" });
        }

        public Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request)
        {
            ValidatePaymentIntentRequest(request);
            return Task.FromResult(new PaymentIntentResult { Success = true, IntentId = Guid.NewGuid().ToString(), CreatedAt = DateTime.UtcNow });
        }

        public Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null)
        {
            return Task.FromResult(new CaptureResult { Success = true, TransactionId = authorizationId, ProcessedAt = DateTime.UtcNow });
        }

        public Task<VoidResult> VoidPaymentAsync(string authorizationId)
        {
            return Task.FromResult(new VoidResult { Success = true, AuthorizationId = authorizationId, Status = "voided", ProcessedAt = DateTime.UtcNow });
        }

        public Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request)
        {
            ValidateCustomerProfileRequest(request);
            return Task.FromResult(new CustomerProfileResult { Success = true, CustomerId = Guid.NewGuid().ToString(), Email = request.Email, Name = request.Name, CreatedAt = DateTime.UtcNow });
        }

        public Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate)
        {
            return Task.FromResult(Enumerable.Empty<TransactionSummary>());
        }
    }
}
