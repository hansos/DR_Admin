using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PaymentGatewayLib.Interfaces;
using PaymentGatewayLib.Models;

namespace PaymentGatewayLib.Implementations
{
    /// <summary>
    /// iPay Africa payment gateway implementation for C2B (Customer to Business) payments.
    /// Implements the iPay Africa C2B API for processing payments in Kenya and other African markets.
    /// See: https://dev.ipayafrica.com/C2B.html
    /// </summary>
    /// <remarks>
    /// This gateway supports:
    /// - Multiple currencies (KES, USD, GBP, EUR, ZAR)
    /// - Mobile money and card payments
    /// - Callback/webhook verification
    /// - Test and live modes
    /// </remarks>
    public class IPayAfricaPaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly string _vendorId;
        private readonly string _hashKey;
        private readonly bool _useTestMode;
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly string _statusEndpoint;
        
        /// <summary>
        /// iPay Africa live payment endpoint
        /// </summary>
        public const string LiveUrl = "https://payments.ipayafrica.com/v3/ke";
        
        /// <summary>
        /// iPay Africa demo/test payment endpoint
        /// </summary>
        public const string DemoUrl = "https://payments.ipayafrica.com/v3/ke";
        
        /// <summary>
        /// Status code returned by iPay Africa for successful transactions
        /// </summary>
        public const string StatusSuccess = "aei7p7yrx4ae34";
        
        /// <summary>
        /// Status code returned by iPay Africa for failed transactions
        /// </summary>
        public const string StatusFailed = "dtfi4p7yty45wq";
        
        /// <summary>
        /// Status code returned by iPay Africa for pending transactions
        /// </summary>
        public const string StatusPending = "bdi6p2yy76etrs";

        /// <summary>
        /// Gets the vendor ID used for iPay Africa authentication
        /// </summary>
        public string VendorId => _vendorId;
        
        /// <summary>
        /// Gets whether the gateway is in test mode
        /// </summary>
        public bool UseTestMode => _useTestMode;
        
        /// <summary>
        /// Gets the payment URL based on the current mode (test or live)
        /// </summary>
        public string PaymentUrl => _useTestMode ? DemoUrl : LiveUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="IPayAfricaPaymentGateway"/> class.
        /// </summary>
        /// <param name="vendorId">The iPay Africa vendor ID</param>
        /// <param name="hashKey">The iPay Africa hash key for security</param>
        /// <param name="useTestMode">Whether to use test mode (default: true)</param>
        /// <exception cref="ArgumentNullException">Thrown when vendorId or hashKey is null</exception>
        public IPayAfricaPaymentGateway(string vendorId, string hashKey, bool useTestMode = true)
            : this(vendorId, hashKey, useTestMode, "https://apis.ipayafrica.com", "/payments/v2/transact/mobilemoney/status")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPayAfricaPaymentGateway"/> class with custom API settings.
        /// </summary>
        /// <param name="vendorId">The iPay Africa vendor ID</param>
        /// <param name="hashKey">The iPay Africa hash key for security</param>
        /// <param name="useTestMode">Whether to use test mode</param>
        /// <param name="apiBaseUrl">The API base URL for transaction status calls</param>
        /// <param name="statusEndpoint">The transaction status endpoint path</param>
        /// <param name="httpClient">Optional HttpClient instance (will create new if not provided)</param>
        /// <exception cref="ArgumentNullException">Thrown when required parameters are null</exception>
        public IPayAfricaPaymentGateway(
            string vendorId, 
            string hashKey, 
            bool useTestMode,
            string apiBaseUrl,
            string statusEndpoint,
            HttpClient httpClient = null)
        {
            _vendorId = vendorId ?? throw new ArgumentNullException(nameof(vendorId));
            _hashKey = hashKey ?? throw new ArgumentNullException(nameof(hashKey));
            _useTestMode = useTestMode;
            _apiBaseUrl = apiBaseUrl ?? "https://apis.ipayafrica.com";
            _statusEndpoint = statusEndpoint ?? "/payments/v2/transact/mobilemoney/status";
            
            _httpClient = httpClient ?? new HttpClient
            {
                BaseAddress = new Uri(_apiBaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
            
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Override base validation to add iPay Africa-specific requirements
        /// </summary>
        protected override void ValidatePaymentRequest(PaymentRequest request)
        {
            base.ValidatePaymentRequest(request);

            // iPay Africa requires customer email
            if (string.IsNullOrWhiteSpace(request.CustomerEmail))
            {
                throw new ArgumentException("Customer email is required for iPay Africa payments", nameof(request));
            }

            // Validate email format
            if (!IsValidEmail(request.CustomerEmail))
            {
                throw new ArgumentException("Invalid email format", nameof(request));
            }

            // iPay Africa supports specific currencies (primarily KES, USD, etc.)
            var supportedCurrencies = GetSupportedCurrencies();
            if (!supportedCurrencies.Contains(request.Currency.ToUpper()))
            {
                throw new ArgumentException($"Currency {request.Currency} is not supported. Supported currencies: {string.Join(", ", supportedCurrencies)}", nameof(request));
            }
        }

        /// <summary>
        /// Validates email address format using .NET mail address validation.
        /// </summary>
        /// <param name="email">The email address to validate</param>
        /// <returns>True if the email is valid, false otherwise</returns>
        protected internal virtual bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
                
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Gets the supported currencies for iPay Africa transactions.
        /// </summary>
        /// <returns>Array of supported ISO currency codes</returns>
        protected internal virtual string[] GetSupportedCurrencies()
        {
            return new[] { "KES", "USD", "GBP", "EUR", "ZAR" };
        }

        /// <summary>
        /// Processes a payment request through iPay Africa.
        /// Generates payment parameters and hash for iPay Africa C2B transaction.
        /// </summary>
        /// <param name="request">The payment request containing transaction details</param>
        /// <returns>A PaymentResult containing payment URL, hash, and transaction details</returns>
        /// <remarks>
        /// This method prepares the payment for iPay Africa but does not execute it.
        /// The returned PaymentResult.Metadata contains:
        /// - payment_url: The iPay Africa endpoint to redirect to
        /// - hash: The security hash for the transaction
        /// - vendor_id: The merchant vendor ID
        /// - order_id: The transaction order ID
        /// Client should redirect the customer to the payment_url with all parameters.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when request validation fails</exception>
        public virtual Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
        {
            try
            {
                ValidatePaymentRequest(request);

                // iPay Africa requires email and phone
                if (string.IsNullOrWhiteSpace(request.CustomerEmail))
                {
                    return Task.FromResult(CreateErrorResult("Customer email is required for iPay Africa", "EMAIL_REQUIRED"));
                }

                // Extract phone from metadata if provided
                string phone = request.Metadata.ContainsKey("phone") ? request.Metadata["phone"] : "";
                
                // Build payment parameters according to iPay Africa C2B API
                var paymentData = BuildPaymentParameters(request, phone);

                // Generate hash for security (iPay Africa hash format: live + vendorId + invoice + amount + telephone + email + vid + currency + p1 + p2 + p3 + p4 + callback_url + hashKey)
                string hash = GenerateHash(paymentData);

                // In a real implementation, you would:
                // 1. Return a redirect URL or HTML form to POST to iPay Africa
                // 2. Store the transaction in your database
                // 3. Wait for iPay callback to confirm payment
                
                // For now, return payment data in metadata for client-side redirect
                var result = new PaymentResult
                {
                    Success = true,
                    TransactionId = paymentData["ivm_oid"],
                    Status = "pending",
                    Amount = request.Amount,
                    Currency = request.Currency,
                    ProcessedAt = DateTime.UtcNow,
                    Metadata = new Dictionary<string, string>
                    {
                        ["payment_url"] = _useTestMode ? DemoUrl : LiveUrl,
                        ["hash"] = hash,
                        ["vendor_id"] = _vendorId,
                        ["order_id"] = paymentData["ivm_oid"],
                        ["email"] = request.CustomerEmail,
                        ["phone"] = phone,
                        ["amount"] = request.Amount.ToString("F2"),
                        ["currency"] = request.Currency,
                        ["callback_url"] = paymentData.ContainsKey("callback_url") ? paymentData["callback_url"] : ""
                    }
                };

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                return Task.FromResult(CreateErrorResult($"iPay Africa Error: {ex.Message}", "exception"));
            }
        }

        /// <summary>
        /// Builds payment parameters dictionary for iPay Africa C2B request.
        /// Includes all required fields according to iPay Africa API specification.
        /// </summary>
        /// <param name="request">The payment request</param>
        /// <param name="phone">Customer phone number (optional)</param>
        /// <returns>Dictionary of iPay Africa payment parameters</returns>
        protected internal virtual Dictionary<string, string> BuildPaymentParameters(PaymentRequest request, string phone)
        {
            var parameters = new Dictionary<string, string>
            {
                ["live"] = _useTestMode ? "0" : "1",
                ["oid"] = string.IsNullOrEmpty(request.ReferenceId) ? Guid.NewGuid().ToString() : request.ReferenceId,
                ["inv"] = string.IsNullOrEmpty(request.ReferenceId) ? Guid.NewGuid().ToString() : request.ReferenceId,
                ["ttl"] = request.Amount.ToString("F2"),
                ["tel"] = phone,
                ["eml"] = request.CustomerEmail,
                ["vid"] = _vendorId,
                ["curr"] = request.Currency,
                ["p1"] = request.Description ?? "",
                ["p2"] = request.Metadata.ContainsKey("p2") ? request.Metadata["p2"] : "",
                ["p3"] = request.Metadata.ContainsKey("p3") ? request.Metadata["p3"] : "",
                ["p4"] = request.Metadata.ContainsKey("p4") ? request.Metadata["p4"] : "",
                ["cbk"] = request.Metadata.ContainsKey("callback_url") ? request.Metadata["callback_url"] : "",
                ["cst"] = "1", // Customer selects payment method
                ["crl"] = "0", // Credit card enabled
                
                // Additional iPay fields
                ["ivm_oid"] = string.IsNullOrEmpty(request.ReferenceId) ? Guid.NewGuid().ToString() : request.ReferenceId,
                ["ivm_inv"] = string.IsNullOrEmpty(request.ReferenceId) ? Guid.NewGuid().ToString() : request.ReferenceId,
                ["ivm_ttl"] = request.Amount.ToString("F2"),
                ["ivm_tel"] = phone,
                ["ivm_eml"] = request.CustomerEmail,
                ["ivm_vid"] = _vendorId,
                ["ivm_curr"] = request.Currency,
                ["ivm_p1"] = request.Description ?? "",
                ["ivm_p2"] = request.Metadata.ContainsKey("p2") ? request.Metadata["p2"] : "",
                ["ivm_p3"] = request.Metadata.ContainsKey("p3") ? request.Metadata["p3"] : "",
                ["ivm_p4"] = request.Metadata.ContainsKey("p4") ? request.Metadata["p4"] : "",
                ["callback_url"] = request.Metadata.ContainsKey("callback_url") ? request.Metadata["callback_url"] : ""
            };

            return parameters;
        }

        /// <summary>
        /// Generates SHA1 hash according to iPay Africa specification.
        /// Hash format: live + oid + inv + ttl + tel + eml + vid + curr + p1 + p2 + p3 + p4 + cbk + hashKey
        /// </summary>
        /// <param name="parameters">Payment parameters dictionary</param>
        /// <returns>Lowercase SHA1 hash string</returns>
        protected internal virtual string GenerateHash(Dictionary<string, string> parameters)
        {
            var dataString = string.Concat(
                parameters["live"],
                parameters["oid"],
                parameters["inv"],
                parameters["ttl"],
                parameters["tel"],
                parameters["eml"],
                parameters["vid"],
                parameters["curr"],
                parameters["p1"],
                parameters["p2"],
                parameters["p3"],
                parameters["p4"],
                parameters["cbk"],
                _hashKey
            );

            using (var sha1 = SHA1.Create())
            {
                var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(dataString));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// Processes a refund for a previous iPay Africa transaction.
        /// </summary>
        /// <param name="request">The refund request containing transaction details</param>
        /// <returns>RefundResult indicating success or failure</returns>
        /// <remarks>
        /// Note: iPay Africa refunds may require manual processing or API integration.
        /// This is a placeholder implementation.
        /// </remarks>
        public virtual Task<RefundResult> RefundPaymentAsync(RefundRequest request)
        {
            try
            {
                ValidateRefundRequest(request);
                return Task.FromResult(new RefundResult
                {
                    Success = true,
                    RefundId = Guid.NewGuid().ToString(),
                    OriginalTransactionId = request.TransactionId,
                    Status = "refunded",
                    ProcessedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new RefundResult { Success = false, ErrorMessage = ex.Message, ProcessedAt = DateTime.UtcNow });
            }
        }

        /// <summary>
        /// Retrieves the status of a transaction from iPay Africa.
        /// Calls the iPay Africa transaction status API to get real-time status.
        /// </summary>
        /// <param name="transactionId">The transaction ID to query</param>
        /// <returns>TransactionStatusResult containing the current status</returns>
        /// <remarks>
        /// This method calls: {ApiBaseUrl}{StatusEndpoint}
        /// Default: https://apis.ipayafrica.com/payments/v2/transact/mobilemoney/status
        /// 
        /// Request format:
        /// {
        ///   "vendor_id": "your-vendor-id",
        ///   "transaction_id": "TXN123456"
        /// }
        /// 
        /// Response format:
        /// {
        ///   "status": "success|pending|failed",
        ///   "transaction_id": "TXN123456",
        ///   "amount": 1000.00,
        ///   "currency": "KES",
        ///   "customer_phone": "+254712345678"
        /// }
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when transactionId is null or empty</exception>
        public virtual async Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
            {
                throw new ArgumentException("Transaction ID cannot be null or empty", nameof(transactionId));
            }

            try
            {
                // Build request payload
                var requestData = new Dictionary<string, string>
                {
                    ["vendor_id"] = _vendorId,
                    ["transaction_id"] = transactionId
                };

                // Serialize to JSON
                var jsonContent = JsonSerializer.Serialize(requestData);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Call iPay Africa status API
                var response = await _httpClient.PostAsync(_statusEndpoint, httpContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var statusData = JsonSerializer.Deserialize<JsonElement>(responseBody);

                    var result = new TransactionStatusResult
                    {
                        TransactionId = transactionId,
                        Status = ParseApiStatus(statusData),
                        Metadata = new Dictionary<string, string>
                        {
                            ["raw_response"] = responseBody
                        }
                    };

                    // Extract additional data if available
                    if (statusData.TryGetProperty("amount", out var amount))
                    {
                        result.Metadata["amount"] = amount.ToString();
                    }

                    if (statusData.TryGetProperty("currency", out var currency))
                    {
                        result.Metadata["currency"] = currency.GetString() ?? "";
                    }

                    if (statusData.TryGetProperty("customer_phone", out var phone))
                    {
                        result.Metadata["customer_phone"] = phone.GetString() ?? "";
                    }

                    if (statusData.TryGetProperty("transaction_date", out var txnDate))
                    {
                        result.Metadata["transaction_date"] = txnDate.GetString() ?? "";
                    }

                    return result;
                }
                else
                {
                    // API call failed
                    return new TransactionStatusResult
                    {
                        TransactionId = transactionId,
                        Status = "unknown",
                        Metadata = new Dictionary<string, string>
                        {
                            ["error"] = $"API returned status code: {response.StatusCode}",
                            ["raw_response"] = responseBody
                        }
                    };
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Network or HTTP error
                return new TransactionStatusResult
                {
                    TransactionId = transactionId,
                    Status = "unknown",
                    Metadata = new Dictionary<string, string>
                    {
                        ["error"] = $"HTTP error: {httpEx.Message}",
                        ["error_type"] = "network"
                    }
                };
            }
            catch (TaskCanceledException timeoutEx)
            {
                // Request timeout
                return new TransactionStatusResult
                {
                    TransactionId = transactionId,
                    Status = "unknown",
                    Metadata = new Dictionary<string, string>
                    {
                        ["error"] = $"Request timeout: {timeoutEx.Message}",
                        ["error_type"] = "timeout"
                    }
                };
            }
            catch (JsonException jsonEx)
            {
                // JSON parsing error
                return new TransactionStatusResult
                {
                    TransactionId = transactionId,
                    Status = "unknown",
                    Metadata = new Dictionary<string, string>
                    {
                        ["error"] = $"JSON parsing error: {jsonEx.Message}",
                        ["error_type"] = "parse"
                    }
                };
            }
            catch (Exception ex)
            {
                // Unexpected error
                return new TransactionStatusResult
                {
                    TransactionId = transactionId,
                    Status = "unknown",
                    Metadata = new Dictionary<string, string>
                    {
                        ["error"] = $"Unexpected error: {ex.Message}",
                        ["error_type"] = "exception"
                    }
                };
            }
        }

        /// <summary>
        /// Parses the status from iPay Africa API response.
        /// Handles various status formats returned by the API.
        /// </summary>
        /// <param name="statusData">JSON response from iPay Africa API</param>
        /// <returns>Normalized status string</returns>
        protected internal virtual string ParseApiStatus(JsonElement statusData)
        {
            // Try to get status field
            if (statusData.TryGetProperty("status", out var statusElement))
            {
                var status = statusElement.GetString()?.ToLower();
                
                // Map iPay status codes to standard statuses
                return status switch
                {
                    "success" or "completed" or "complete" => "success",
                    "pending" or "processing" => "pending",
                    "failed" or "cancelled" or "canceled" => "failed",
                    _ => status ?? "unknown"
                };
            }

            // Try alternate status field names
            if (statusData.TryGetProperty("transaction_status", out var txnStatus))
            {
                return txnStatus.GetString()?.ToLower() ?? "unknown";
            }

            if (statusData.TryGetProperty("payment_status", out var payStatus))
            {
                return payStatus.GetString()?.ToLower() ?? "unknown";
            }

            return "unknown";
        }

        /// <summary>
        /// Verifies iPay Africa callback/webhook data for authenticity.
        /// </summary>
        /// <param name="callbackData">Dictionary containing callback parameters from iPay Africa</param>
        /// <returns>True if callback is valid, false otherwise</returns>
        /// <remarks>
        /// iPay Africa status codes:
        /// - aei7p7yrx4ae34 = Success/Complete
        /// - dtfi4p7yty45wq = Failed
        /// - bdi6p2yy76etrs = Pending
        /// 
        /// In production, this should:
        /// 1. Verify the hash signature
        /// 2. Check vendor ID matches
        /// 3. Validate transaction hasn't been processed
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when callbackData is null</exception>
        public virtual bool VerifyCallback(Dictionary<string, string> callbackData)
        {
            if (callbackData == null)
            {
                throw new ArgumentNullException(nameof(callbackData));
            }
            
            // Must have status and transaction code
            if (!callbackData.ContainsKey("status") || !callbackData.ContainsKey("txncd"))
            {
                return false;
            }

            // Verify hash if present
            if (callbackData.ContainsKey("hash"))
            {
                var receivedHash = callbackData["hash"];
                // In production, regenerate hash from callback data and compare
                // Hash format varies by iPay callback type
                // TODO: Implement actual hash verification
            }
            
            return true;
        }
        
        /// <summary>
        /// Parses iPay Africa status code to human-readable status.
        /// </summary>
        /// <param name="statusCode">The iPay Africa status code</param>
        /// <returns>Human-readable status string</returns>
        public virtual string ParseIPayStatus(string statusCode)
        {
            return statusCode switch
            {
                StatusSuccess => "success",
                StatusFailed => "failed",
                StatusPending => "pending",
                _ => "unknown"
            };
        }

        /// <summary>
        /// Creates a payment intent for deferred payment processing.
        /// </summary>
        /// <param name="request">The payment intent request</param>
        /// <returns>PaymentIntentResult with intent ID</returns>
        /// <remarks>
        /// Note: iPay Africa primarily uses redirect-based payments.
        /// This is a placeholder for future integration.
        /// </remarks>
        public virtual Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request)
        {
            ValidatePaymentIntentRequest(request);
            return Task.FromResult(new PaymentIntentResult { Success = true, IntentId = Guid.NewGuid().ToString(), CreatedAt = DateTime.UtcNow });
        }

        /// <summary>
        /// Captures a previously authorized payment.
        /// </summary>
        /// <param name="authorizationId">The authorization ID to capture</param>
        /// <param name="amount">The amount to capture (optional)</param>
        /// <returns>CaptureResult indicating success or failure</returns>
        /// <remarks>
        /// Note: iPay Africa typically processes payments immediately.
        /// This is a placeholder for compatibility.
        /// </remarks>
        public virtual Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null)
        {
            if (string.IsNullOrWhiteSpace(authorizationId))
            {
                throw new ArgumentException("Authorization ID cannot be null or empty", nameof(authorizationId));
            }
            
            return Task.FromResult(new CaptureResult { Success = true, TransactionId = authorizationId, ProcessedAt = DateTime.UtcNow });
        }

        /// <summary>
        /// Voids a previously authorized payment.
        /// </summary>
        /// <param name="authorizationId">The authorization ID to void</param>
        /// <returns>VoidResult indicating success or failure</returns>
        /// <remarks>
        /// Note: iPay Africa void functionality may require manual processing.
        /// This is a placeholder implementation.
        /// </remarks>
        public virtual Task<VoidResult> VoidPaymentAsync(string authorizationId)
        {
            if (string.IsNullOrWhiteSpace(authorizationId))
            {
                throw new ArgumentException("Authorization ID cannot be null or empty", nameof(authorizationId));
            }
            
            return Task.FromResult(new VoidResult { Success = true, AuthorizationId = authorizationId, Status = "voided", ProcessedAt = DateTime.UtcNow });
        }

        /// <summary>
        /// Creates a customer profile for recurring payments.
        /// </summary>
        /// <param name="request">The customer profile request</param>
        /// <returns>CustomerProfileResult with customer ID</returns>
        /// <remarks>
        /// Note: Customer profile functionality depends on iPay Africa API support.
        /// This is a placeholder implementation.
        /// </remarks>
        public virtual Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request)
        {
            ValidateCustomerProfileRequest(request);
            return Task.FromResult(new CustomerProfileResult { Success = true, CustomerId = Guid.NewGuid().ToString(), Email = request.Email, Name = request.Name, CreatedAt = DateTime.UtcNow });
        }

        /// <summary>
        /// Retrieves transaction history for a date range.
        /// </summary>
        /// <param name="startDate">Start date for transaction query</param>
        /// <param name="endDate">End date for transaction query</param>
        /// <returns>Collection of transaction summaries</returns>
        /// <remarks>
        /// In production, this should call iPay Africa's reporting API or query local database.
        /// Current implementation returns empty results.
        /// </remarks>
        public virtual Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be before end date", nameof(startDate));
            }
            
            // In production, query iPay Africa API or local database
            return Task.FromResult(Enumerable.Empty<TransactionSummary>());
        }

        /// <summary>
        /// Creates an instance of IPayAfricaPaymentGateway from configuration settings.
        /// </summary>
        /// <param name="settings">iPay Africa settings</param>
        /// <param name="httpClient">Optional HttpClient instance</param>
        /// <returns>Configured IPayAfricaPaymentGateway instance</returns>
        /// <exception cref="ArgumentNullException">Thrown when settings is null</exception>
        public static IPayAfricaPaymentGateway FromSettings(Infrastructure.Settings.IPayAfricaSettings settings, HttpClient httpClient = null)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            return new IPayAfricaPaymentGateway(
                vendorId: settings.VendorId,
                hashKey: settings.HashKey,
                useTestMode: settings.UseTestMode,
                apiBaseUrl: settings.ApiBaseUrl,
                statusEndpoint: settings.StatusEndpoint,
                httpClient: httpClient
            );
        }
    }
}
