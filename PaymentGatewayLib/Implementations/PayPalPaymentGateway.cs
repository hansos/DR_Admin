using PaymentGatewayLib.Infrastructure.Settings;
using PaymentGatewayLib.Interfaces;
using PaymentGatewayLib.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PaymentGatewayLib.Implementations
{
    /// <summary>
    /// PayPal payment gateway implementation
    /// </summary>
    public class PayPalPaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;
        private string? _accessToken;
        private DateTime _tokenExpiry;

        public PayPalPaymentGateway(string clientId, string clientSecret, bool useSandbox = true)
        {
            _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
            _clientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
            _apiBaseUrl = useSandbox 
                ? "https://api-m.sandbox.paypal.com" 
                : "https://api-m.paypal.com";
            
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_apiBaseUrl)
            };
        }

        /// <summary>
        /// Processes a payment transaction using PayPal
        /// </summary>
        public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
        {
            try
            {
                ValidatePaymentRequest(request);
                await EnsureAccessTokenAsync();

                var orderRequest = new
                {
                    intent = request.CaptureImmediately ? "CAPTURE" : "AUTHORIZE",
                    purchase_units = new[]
                    {
                        new
                        {
                            amount = new
                            {
                                currency_code = request.Currency.ToUpper(),
                                value = request.Amount.ToString("F2")
                            },
                            description = request.Description,
                            reference_id = request.ReferenceId,
                            payee = new
                            {
                                email_address = request.CustomerEmail
                            }
                        }
                    },
                    payment_source = new
                    {
                        token = new
                        {
                            id = request.PaymentMethodToken,
                            type = "PAYMENT_METHOD_TOKEN"
                        }
                    }
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(orderRequest),
                    Encoding.UTF8,
                    "application/json"
                );

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                var response = await _httpClient.PostAsync("/v2/checkout/orders", jsonContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var order = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var orderId = order.GetProperty("id").GetString() ?? string.Empty;
                    var status = order.GetProperty("status").GetString() ?? string.Empty;

                    return new PaymentResult
                    {
                        Success = true,
                        TransactionId = orderId,
                        Status = status,
                        Amount = request.Amount,
                        Currency = request.Currency,
                        ProcessedAt = DateTime.UtcNow,
                        RawResponse = responseBody
                    };
                }
                else
                {
                    var error = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    
                    return new PaymentResult
                    {
                        Success = false,
                        ErrorMessage = error.TryGetProperty("message", out var msg) 
                            ? msg.GetString() ?? "Unknown error" 
                            : "Unknown error",
                        ErrorCode = error.TryGetProperty("name", out var name) 
                            ? name.GetString() ?? string.Empty 
                            : string.Empty,
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
                await EnsureAccessTokenAsync();

                var refundRequest = new
                {
                    amount = request.Amount.HasValue ? new
                    {
                        currency_code = "USD", // Should be from original transaction
                        value = request.Amount.Value.ToString("F2")
                    } : null,
                    note_to_payer = request.Reason
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(refundRequest),
                    Encoding.UTF8,
                    "application/json"
                );

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                var response = await _httpClient.PostAsync($"/v2/payments/captures/{request.TransactionId}/refund", jsonContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var refund = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    
                    return new RefundResult
                    {
                        Success = true,
                        RefundId = refund.GetProperty("id").GetString() ?? string.Empty,
                        OriginalTransactionId = request.TransactionId,
                        Status = refund.GetProperty("status").GetString() ?? string.Empty,
                        ProcessedAt = DateTime.UtcNow,
                        RawResponse = responseBody
                    };
                }
                else
                {
                    var error = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    
                    return new RefundResult
                    {
                        Success = false,
                        ErrorMessage = error.TryGetProperty("message", out var msg) 
                            ? msg.GetString() ?? "Unknown error" 
                            : "Unknown error",
                        ErrorCode = error.TryGetProperty("name", out var name) 
                            ? name.GetString() ?? string.Empty 
                            : string.Empty,
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

                await EnsureAccessTokenAsync();

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                var response = await _httpClient.GetAsync($"/v2/checkout/orders/{transactionId}");
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var order = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var purchaseUnits = order.GetProperty("purchase_units");
                    var firstUnit = purchaseUnits[0];
                    var amount = firstUnit.GetProperty("amount");
                    
                    return new TransactionStatusResult
                    {
                        TransactionId = order.GetProperty("id").GetString() ?? string.Empty,
                        Status = order.GetProperty("status").GetString() ?? string.Empty,
                        Amount = decimal.Parse(amount.GetProperty("value").GetString() ?? "0"),
                        Currency = amount.GetProperty("currency_code").GetString() ?? string.Empty,
                        CreatedAt = DateTime.TryParse(order.GetProperty("create_time").GetString(), out var created) 
                            ? created 
                            : DateTime.UtcNow,
                        UpdatedAt = DateTime.TryParse(order.GetProperty("update_time").GetString(), out var updated) 
                            ? updated 
                            : DateTime.UtcNow
                    };
                }
                else
                {
                    throw new Exception($"Failed to retrieve transaction status: {responseBody}");
                }
            }
            catch (Exception)
            {
                return new TransactionStatusResult
                {
                    TransactionId = transactionId,
                    Status = "error"
                };
            }
        }

        /// <summary>
        /// Creates a payment intent (order) for later processing
        /// </summary>
        public async Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request)
        {
            try
            {
                ValidatePaymentIntentRequest(request);
                await EnsureAccessTokenAsync();

                var orderRequest = new
                {
                    intent = request.AutomaticCapture ? "CAPTURE" : "AUTHORIZE",
                    purchase_units = new[]
                    {
                        new
                        {
                            amount = new
                            {
                                currency_code = request.Currency.ToUpper(),
                                value = request.Amount.ToString("F2")
                            },
                            description = request.Description
                        }
                    }
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(orderRequest),
                    Encoding.UTF8,
                    "application/json"
                );

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                var response = await _httpClient.PostAsync("/v2/checkout/orders", jsonContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var order = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    
                    return new PaymentIntentResult
                    {
                        Success = true,
                        IntentId = order.GetProperty("id").GetString() ?? string.Empty,
                        Status = order.GetProperty("status").GetString() ?? string.Empty,
                        Amount = request.Amount,
                        Currency = request.Currency,
                        CreatedAt = DateTime.UtcNow
                    };
                }
                else
                {
                    var error = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    
                    return new PaymentIntentResult
                    {
                        Success = false,
                        ErrorMessage = error.TryGetProperty("message", out var msg) 
                            ? msg.GetString() ?? "Unknown error" 
                            : "Unknown error",
                        ErrorCode = error.TryGetProperty("name", out var name) 
                            ? name.GetString() ?? string.Empty 
                            : string.Empty,
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

                await EnsureAccessTokenAsync();

                object captureRequest;
                if (amount.HasValue)
                {
                    captureRequest = new
                    {
                        amount = new
                        {
                            currency_code = "USD",
                            value = amount.Value.ToString("F2")
                        }
                    };
                }
                else
                {
                    captureRequest = new { };
                }

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(captureRequest),
                    Encoding.UTF8,
                    "application/json"
                );

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                var response = await _httpClient.PostAsync($"/v2/payments/authorizations/{authorizationId}/capture", jsonContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var capture = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    
                    return new CaptureResult
                    {
                        Success = true,
                        TransactionId = capture.GetProperty("id").GetString() ?? string.Empty,
                        AuthorizationId = authorizationId,
                        Status = capture.GetProperty("status").GetString() ?? string.Empty,
                        ProcessedAt = DateTime.UtcNow
                    };
                }
                else
                {
                    var error = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    
                    return new CaptureResult
                    {
                        Success = false,
                        ErrorMessage = error.TryGetProperty("message", out var msg) 
                            ? msg.GetString() ?? "Unknown error" 
                            : "Unknown error",
                        ErrorCode = error.TryGetProperty("name", out var name) 
                            ? name.GetString() ?? string.Empty 
                            : string.Empty,
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

                await EnsureAccessTokenAsync();

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                var response = await _httpClient.PostAsync($"/v2/payments/authorizations/{authorizationId}/void", null);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return new VoidResult
                    {
                        Success = true,
                        AuthorizationId = authorizationId,
                        Status = "voided",
                        ProcessedAt = DateTime.UtcNow
                    };
                }
                else
                {
                    var error = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    
                    return new VoidResult
                    {
                        Success = false,
                        ErrorMessage = error.TryGetProperty("message", out var msg) 
                            ? msg.GetString() ?? "Unknown error" 
                            : "Unknown error",
                        ErrorCode = error.TryGetProperty("name", out var name) 
                            ? name.GetString() ?? string.Empty 
                            : string.Empty,
                        ProcessedAt = DateTime.UtcNow
                    };
                }
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
        /// Creates a customer profile in PayPal (not directly supported, returns basic info)
        /// </summary>
        public async Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request)
        {
            try
            {
                ValidateCustomerProfileRequest(request);

                // PayPal doesn't have a direct customer profile creation API like Stripe
                // This would typically be handled through PayPal's vault API or customer info storage
                // For now, we'll return a success with the email as the customer ID
                
                return new CustomerProfileResult
                {
                    Success = true,
                    CustomerId = Guid.NewGuid().ToString(), // Generate a temporary ID
                    Email = request.Email,
                    Name = request.Name,
                    CreatedAt = DateTime.UtcNow
                };
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
                await EnsureAccessTokenAsync();

                var startDateStr = startDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
                var endDateStr = endDate.ToString("yyyy-MM-ddTHH:mm:ssZ");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                var response = await _httpClient.GetAsync($"/v1/reporting/transactions?start_date={startDateStr}&end_date={endDateStr}");
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    
                    if (result.TryGetProperty("transaction_details", out var transactions))
                    {
                        var transactionList = new List<TransactionSummary>();

                        foreach (var txn in transactions.EnumerateArray())
                        {
                            var transactionInfo = txn.GetProperty("transaction_info");
                            
                            transactionList.Add(new TransactionSummary
                            {
                                TransactionId = transactionInfo.GetProperty("transaction_id").GetString() ?? string.Empty,
                                Type = transactionInfo.TryGetProperty("transaction_event_code", out var eventCode) 
                                    ? eventCode.GetString() ?? "payment" 
                                    : "payment",
                                Status = transactionInfo.GetProperty("transaction_status").GetString() ?? string.Empty,
                                CreatedAt = DateTime.TryParse(
                                    transactionInfo.GetProperty("transaction_initiation_date").GetString(), 
                                    out var created) ? created : DateTime.UtcNow,
                                UpdatedAt = DateTime.TryParse(
                                    transactionInfo.GetProperty("transaction_updated_date").GetString(), 
                                    out var updated) ? updated : DateTime.UtcNow
                            });
                        }

                        return transactionList;
                    }
                }

                return Enumerable.Empty<TransactionSummary>();
            }
            catch (Exception)
            {
                return Enumerable.Empty<TransactionSummary>();
            }
        }

        private async Task EnsureAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry)
            {
                return; // Token is still valid
            }

            var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var response = await _httpClient.PostAsync("/v1/oauth2/token", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
                _accessToken = tokenResponse.GetProperty("access_token").GetString();
                var expiresIn = tokenResponse.GetProperty("expires_in").GetInt32();
                _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn - 60); // Refresh 60 seconds early
            }
            else
            {
                throw new Exception($"Failed to obtain access token: {responseBody}");
            }
        }
    }
}
