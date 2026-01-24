using PaymentGatewayLib.Infrastructure.Settings;
using PaymentGatewayLib.Interfaces;
using PaymentGatewayLib.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PaymentGatewayLib.Implementations
{
    /// <summary>
    /// Stripe payment gateway implementation
    /// </summary>
    public class StripePaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly string _secretKey;
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;

        public StripePaymentGateway(string secretKey, string publishableKey, string apiBaseUrl = "https://api.stripe.com")
        {
            _secretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
            _apiBaseUrl = apiBaseUrl;
            
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_apiBaseUrl)
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);
        }

        /// <summary>
        /// Processes a payment transaction using Stripe
        /// </summary>
        public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
        {
            try
            {
                ValidatePaymentRequest(request);

                var amountInCents = ConvertToSmallestUnit(request.Amount, request.Currency);

                var parameters = new Dictionary<string, string>
                {
                    { "amount", amountInCents.ToString() },
                    { "currency", request.Currency.ToLower() },
                    { "source", request.PaymentMethodToken },
                    { "description", request.Description }
                };

                if (!string.IsNullOrWhiteSpace(request.CustomerEmail))
                {
                    parameters.Add("receipt_email", request.CustomerEmail);
                }

                if (request.Metadata != null && request.Metadata.Any())
                {
                    foreach (var meta in request.Metadata)
                    {
                        parameters.Add($"metadata[{meta.Key}]", meta.Value);
                    }
                }

                parameters.Add("capture", request.CaptureImmediately ? "true" : "false");

                var content = new FormUrlEncodedContent(parameters);
                var response = await _httpClient.PostAsync("/v1/charges", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var charge = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    
                    return new PaymentResult
                    {
                        Success = true,
                        TransactionId = charge.GetProperty("id").GetString() ?? string.Empty,
                        AuthorizationCode = charge.TryGetProperty("authorization_code", out var authCode) 
                            ? authCode.GetString() ?? string.Empty 
                            : string.Empty,
                        Status = charge.GetProperty("status").GetString() ?? string.Empty,
                        Amount = request.Amount,
                        Currency = request.Currency,
                        ProcessedAt = DateTime.UtcNow,
                        RawResponse = responseBody
                    };
                }
                else
                {
                    var error = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var errorObj = error.GetProperty("error");
                    
                    return new PaymentResult
                    {
                        Success = false,
                        ErrorMessage = errorObj.GetProperty("message").GetString() ?? "Unknown error",
                        ErrorCode = errorObj.GetProperty("code").GetString() ?? string.Empty,
                        ProcessedAt = DateTime.UtcNow,
                        RawResponse = responseBody
                    };
                }
            }
            catch (Exception ex)
            {
                return CreateErrorResult($"Error processing payment: {ex.Message}", "exception");
            }
        }

        /// <summary>
        /// Refunds a previous payment transaction
        /// </summary>
        public async Task<RefundResult> RefundPaymentAsync(RefundRequest request)
        {
            try
            {
                ValidateRefundRequest(request);

                var parameters = new Dictionary<string, string>
                {
                    { "charge", request.TransactionId }
                };

                if (request.Amount.HasValue)
                {
                    var amountInCents = ConvertToSmallestUnit(request.Amount.Value, "USD"); // Currency should be from original transaction
                    parameters.Add("amount", amountInCents.ToString());
                }

                if (!string.IsNullOrWhiteSpace(request.Reason))
                {
                    parameters.Add("reason", request.Reason);
                }

                if (request.Metadata != null && request.Metadata.Any())
                {
                    foreach (var meta in request.Metadata)
                    {
                        parameters.Add($"metadata[{meta.Key}]", meta.Value);
                    }
                }

                var content = new FormUrlEncodedContent(parameters);
                var response = await _httpClient.PostAsync("/v1/refunds", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var refund = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    
                    return new RefundResult
                    {
                        Success = true,
                        RefundId = refund.GetProperty("id").GetString() ?? string.Empty,
                        OriginalTransactionId = request.TransactionId,
                        Amount = refund.GetProperty("amount").GetInt64() / 100.0m,
                        Currency = refund.GetProperty("currency").GetString()?.ToUpper() ?? string.Empty,
                        Status = refund.GetProperty("status").GetString() ?? string.Empty,
                        ProcessedAt = DateTime.UtcNow,
                        RawResponse = responseBody
                    };
                }
                else
                {
                    var error = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var errorObj = error.GetProperty("error");
                    
                    return new RefundResult
                    {
                        Success = false,
                        ErrorMessage = errorObj.GetProperty("message").GetString() ?? "Unknown error",
                        ErrorCode = errorObj.GetProperty("code").GetString() ?? string.Empty,
                        ProcessedAt = DateTime.UtcNow,
                        RawResponse = responseBody
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

        /// <summary>
        /// Retrieves the status of a payment transaction
        /// </summary>
        public async Task<TransactionStatusResult> GetTransactionStatusAsync(string transactionId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(transactionId))
                    throw new ArgumentException("Transaction ID is required", nameof(transactionId));

                var response = await _httpClient.GetAsync($"/v1/charges/{transactionId}");
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var charge = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    
                    return new TransactionStatusResult
                    {
                        TransactionId = charge.GetProperty("id").GetString() ?? string.Empty,
                        Status = charge.GetProperty("status").GetString() ?? string.Empty,
                        Amount = charge.GetProperty("amount").GetInt64() / 100.0m,
                        Currency = charge.GetProperty("currency").GetString()?.ToUpper() ?? string.Empty,
                        CreatedAt = DateTimeOffset.FromUnixTimeSeconds(charge.GetProperty("created").GetInt64()).UtcDateTime,
                        UpdatedAt = DateTime.UtcNow,
                        CustomerEmail = charge.TryGetProperty("receipt_email", out var email) 
                            ? email.GetString() ?? string.Empty 
                            : string.Empty,
                        Description = charge.TryGetProperty("description", out var desc) 
                            ? desc.GetString() ?? string.Empty 
                            : string.Empty
                    };
                }
                else
                {
                    throw new Exception($"Failed to retrieve transaction status: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                return new TransactionStatusResult
                {
                    TransactionId = transactionId,
                    Status = "error"
                };
            }
        }

        /// <summary>
        /// Creates a payment intent for later processing
        /// </summary>
        public async Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request)
        {
            try
            {
                ValidatePaymentIntentRequest(request);

                var amountInCents = ConvertToSmallestUnit(request.Amount, request.Currency);

                var parameters = new Dictionary<string, string>
                {
                    { "amount", amountInCents.ToString() },
                    { "currency", request.Currency.ToLower() },
                    { "automatic_payment_methods[enabled]", "true" }
                };

                if (!string.IsNullOrWhiteSpace(request.Description))
                {
                    parameters.Add("description", request.Description);
                }

                if (!string.IsNullOrWhiteSpace(request.CustomerEmail))
                {
                    parameters.Add("receipt_email", request.CustomerEmail);
                }

                if (!string.IsNullOrWhiteSpace(request.CustomerId))
                {
                    parameters.Add("customer", request.CustomerId);
                }

                parameters.Add("capture_method", request.AutomaticCapture ? "automatic" : "manual");

                if (request.Metadata != null && request.Metadata.Any())
                {
                    foreach (var meta in request.Metadata)
                    {
                        parameters.Add($"metadata[{meta.Key}]", meta.Value);
                    }
                }

                var content = new FormUrlEncodedContent(parameters);
                var response = await _httpClient.PostAsync("/v1/payment_intents", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var intent = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    
                    return new PaymentIntentResult
                    {
                        Success = true,
                        IntentId = intent.GetProperty("id").GetString() ?? string.Empty,
                        ClientSecret = intent.GetProperty("client_secret").GetString() ?? string.Empty,
                        Status = intent.GetProperty("status").GetString() ?? string.Empty,
                        Amount = request.Amount,
                        Currency = request.Currency,
                        CreatedAt = DateTimeOffset.FromUnixTimeSeconds(intent.GetProperty("created").GetInt64()).UtcDateTime
                    };
                }
                else
                {
                    var error = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var errorObj = error.GetProperty("error");
                    
                    return new PaymentIntentResult
                    {
                        Success = false,
                        ErrorMessage = errorObj.GetProperty("message").GetString() ?? "Unknown error",
                        ErrorCode = errorObj.GetProperty("code").GetString() ?? string.Empty,
                        CreatedAt = DateTime.UtcNow
                    };
                }
            }
            catch (Exception ex)
            {
                return new PaymentIntentResult
                {
                    Success = false,
                    ErrorMessage = $"Error creating payment intent: {ex.Message}",
                    ErrorCode = "exception",
                    CreatedAt = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Captures an authorized payment
        /// </summary>
        public async Task<CaptureResult> CapturePaymentAsync(string authorizationId, decimal? amount = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(authorizationId))
                    throw new ArgumentException("Authorization ID is required", nameof(authorizationId));

                var parameters = new Dictionary<string, string>();
                
                if (amount.HasValue)
                {
                    var amountInCents = ConvertToSmallestUnit(amount.Value, "USD");
                    parameters.Add("amount", amountInCents.ToString());
                }

                var content = new FormUrlEncodedContent(parameters);
                var response = await _httpClient.PostAsync($"/v1/charges/{authorizationId}/capture", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var charge = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    
                    return new CaptureResult
                    {
                        Success = true,
                        TransactionId = charge.GetProperty("id").GetString() ?? string.Empty,
                        AuthorizationId = authorizationId,
                        Amount = charge.GetProperty("amount").GetInt64() / 100.0m,
                        Currency = charge.GetProperty("currency").GetString()?.ToUpper() ?? string.Empty,
                        Status = charge.GetProperty("status").GetString() ?? string.Empty,
                        ProcessedAt = DateTime.UtcNow
                    };
                }
                else
                {
                    var error = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var errorObj = error.GetProperty("error");
                    
                    return new CaptureResult
                    {
                        Success = false,
                        ErrorMessage = errorObj.GetProperty("message").GetString() ?? "Unknown error",
                        ErrorCode = errorObj.GetProperty("code").GetString() ?? string.Empty,
                        ProcessedAt = DateTime.UtcNow
                    };
                }
            }
            catch (Exception ex)
            {
                return new CaptureResult
                {
                    Success = false,
                    ErrorMessage = $"Error capturing payment: {ex.Message}",
                    ErrorCode = "exception",
                    ProcessedAt = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Voids an authorized payment before capture
        /// </summary>
        public async Task<VoidResult> VoidPaymentAsync(string authorizationId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(authorizationId))
                    throw new ArgumentException("Authorization ID is required", nameof(authorizationId));

                // In Stripe, voiding is done through refunding an uncaptured charge
                var refundRequest = new RefundRequest
                {
                    TransactionId = authorizationId
                };

                var refundResult = await RefundPaymentAsync(refundRequest);

                return new VoidResult
                {
                    Success = refundResult.Success,
                    AuthorizationId = authorizationId,
                    Status = refundResult.Success ? "voided" : "failed",
                    ErrorMessage = refundResult.ErrorMessage,
                    ErrorCode = refundResult.ErrorCode,
                    ProcessedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new VoidResult
                {
                    Success = false,
                    ErrorMessage = $"Error voiding payment: {ex.Message}",
                    ErrorCode = "exception",
                    ProcessedAt = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Creates a customer profile in Stripe
        /// </summary>
        public async Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request)
        {
            try
            {
                ValidateCustomerProfileRequest(request);

                var parameters = new Dictionary<string, string>
                {
                    { "email", request.Email },
                    { "name", request.Name }
                };

                if (!string.IsNullOrWhiteSpace(request.Phone))
                {
                    parameters.Add("phone", request.Phone);
                }

                if (!string.IsNullOrWhiteSpace(request.Description))
                {
                    parameters.Add("description", request.Description);
                }

                if (request.Address != null)
                {
                    if (!string.IsNullOrWhiteSpace(request.Address.Line1))
                        parameters.Add("address[line1]", request.Address.Line1);
                    if (!string.IsNullOrWhiteSpace(request.Address.Line2))
                        parameters.Add("address[line2]", request.Address.Line2);
                    if (!string.IsNullOrWhiteSpace(request.Address.City))
                        parameters.Add("address[city]", request.Address.City);
                    if (!string.IsNullOrWhiteSpace(request.Address.State))
                        parameters.Add("address[state]", request.Address.State);
                    if (!string.IsNullOrWhiteSpace(request.Address.PostalCode))
                        parameters.Add("address[postal_code]", request.Address.PostalCode);
                    if (!string.IsNullOrWhiteSpace(request.Address.Country))
                        parameters.Add("address[country]", request.Address.Country);
                }

                if (request.Metadata != null && request.Metadata.Any())
                {
                    foreach (var meta in request.Metadata)
                    {
                        parameters.Add($"metadata[{meta.Key}]", meta.Value);
                    }
                }

                var content = new FormUrlEncodedContent(parameters);
                var response = await _httpClient.PostAsync("/v1/customers", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var customer = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    
                    return new CustomerProfileResult
                    {
                        Success = true,
                        CustomerId = customer.GetProperty("id").GetString() ?? string.Empty,
                        Email = customer.GetProperty("email").GetString() ?? string.Empty,
                        Name = customer.TryGetProperty("name", out var name) ? name.GetString() ?? string.Empty : string.Empty,
                        CreatedAt = DateTimeOffset.FromUnixTimeSeconds(customer.GetProperty("created").GetInt64()).UtcDateTime
                    };
                }
                else
                {
                    var error = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var errorObj = error.GetProperty("error");
                    
                    return new CustomerProfileResult
                    {
                        Success = false,
                        ErrorMessage = errorObj.GetProperty("message").GetString() ?? "Unknown error",
                        ErrorCode = errorObj.GetProperty("code").GetString() ?? string.Empty,
                        CreatedAt = DateTime.UtcNow
                    };
                }
            }
            catch (Exception ex)
            {
                return new CustomerProfileResult
                {
                    Success = false,
                    ErrorMessage = $"Error creating customer profile: {ex.Message}",
                    ErrorCode = "exception",
                    CreatedAt = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Retrieves list of transactions for a specific period
        /// </summary>
        public async Task<IEnumerable<TransactionSummary>> GetTransactionsAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var startTimestamp = new DateTimeOffset(startDate).ToUnixTimeSeconds();
                var endTimestamp = new DateTimeOffset(endDate).ToUnixTimeSeconds();

                var response = await _httpClient.GetAsync($"/v1/charges?created[gte]={startTimestamp}&created[lte]={endTimestamp}&limit=100");
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var charges = result.GetProperty("data");

                    var transactions = new List<TransactionSummary>();

                    foreach (var charge in charges.EnumerateArray())
                    {
                        transactions.Add(new TransactionSummary
                        {
                            TransactionId = charge.GetProperty("id").GetString() ?? string.Empty,
                            Type = "payment",
                            Status = charge.GetProperty("status").GetString() ?? string.Empty,
                            Amount = charge.GetProperty("amount").GetInt64() / 100.0m,
                            Currency = charge.GetProperty("currency").GetString()?.ToUpper() ?? string.Empty,
                            CustomerEmail = charge.TryGetProperty("receipt_email", out var email) 
                                ? email.GetString() ?? string.Empty 
                                : string.Empty,
                            Description = charge.TryGetProperty("description", out var desc) 
                                ? desc.GetString() ?? string.Empty 
                                : string.Empty,
                            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(charge.GetProperty("created").GetInt64()).UtcDateTime,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }

                    return transactions;
                }
                else
                {
                    return Enumerable.Empty<TransactionSummary>();
                }
            }
            catch (Exception)
            {
                return Enumerable.Empty<TransactionSummary>();
            }
        }

        private long ConvertToSmallestUnit(decimal amount, string currency)
        {
            // Most currencies use 2 decimal places (cents)
            // Some currencies like JPY don't use decimal places
            var zeroDecimalCurrencies = new[] { "JPY", "KRW", "VND", "CLP" };
            
            if (zeroDecimalCurrencies.Contains(currency.ToUpper()))
            {
                return (long)amount;
            }
            
            return (long)(amount * 100);
        }
    }
}
