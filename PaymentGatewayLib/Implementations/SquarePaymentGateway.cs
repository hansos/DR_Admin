using PaymentGatewayLib.Infrastructure.Settings;
using PaymentGatewayLib.Interfaces;
using PaymentGatewayLib.Models;
using Serilog;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PaymentGatewayLib.Implementations
{
    /// <summary>
    /// Square payment gateway implementation
    /// </summary>
    public class SquarePaymentGateway : BasePaymentGateway, IPaymentGateway
    {
        private readonly ILogger _logger;
        private readonly string _accessToken;
        private readonly string _locationId;
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;

        public SquarePaymentGateway(string accessToken, string locationId, bool useSandbox = true)
        {
            _logger = Log.ForContext<SquarePaymentGateway>();
            _accessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
            _locationId = locationId ?? throw new ArgumentNullException(nameof(locationId));
            _apiBaseUrl = useSandbox 
                ? "https://connect.squareupsandbox.com" 
                : "https://connect.squareup.com";
            
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_apiBaseUrl)
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            _httpClient.DefaultRequestHeaders.Add("Square-Version", "2023-10-18");
        }

        /// <summary>
        /// Processes a payment transaction using Square
        /// </summary>
        public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
        {
            try
            {
                ValidatePaymentRequest(request);

                var amountInCents = ConvertToSmallestUnit(request.Amount, request.Currency);

                var paymentRequest = new
                {
                    source_id = request.PaymentMethodToken,
                    idempotency_key = Guid.NewGuid().ToString(),
                    amount_money = new
                    {
                        amount = amountInCents,
                        currency = request.Currency.ToUpper()
                    },
                    autocomplete = request.CaptureImmediately,
                    location_id = _locationId,
                    reference_id = request.ReferenceId,
                    note = request.Description,
                    buyer_email_address = request.CustomerEmail
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(paymentRequest),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync("/v2/payments", jsonContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var payment = result.GetProperty("payment");
                    
                    return new PaymentResult
                    {
                        Success = true,
                        TransactionId = payment.GetProperty("id").GetString() ?? string.Empty,
                        Status = payment.GetProperty("status").GetString() ?? string.Empty,
                        Amount = request.Amount,
                        Currency = request.Currency,
                        ProcessedAt = DateTime.TryParse(
                            payment.GetProperty("created_at").GetString(), 
                            out var created) ? created : DateTime.UtcNow,
                        RawResponse = responseBody
                    };
                }
                else
                {
                    var error = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var errors = error.GetProperty("errors");
                    var firstError = errors[0];
                    
                    return new PaymentResult
                    {
                        Success = false,
                        ErrorMessage = firstError.GetProperty("detail").GetString() ?? "Unknown error",
                        ErrorCode = firstError.GetProperty("code").GetString() ?? string.Empty,
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

                var amountInCents = request.Amount.HasValue 
                    ? ConvertToSmallestUnit(request.Amount.Value, "USD")
                    : (long?)null;

                var refundRequest = new
                {
                    idempotency_key = Guid.NewGuid().ToString(),
                    payment_id = request.TransactionId,
                    amount_money = amountInCents.HasValue ? new
                    {
                        amount = amountInCents.Value,
                        currency = "USD"
                    } : null,
                    reason = request.Reason
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(refundRequest),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync("/v2/refunds", jsonContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var refund = result.GetProperty("refund");
                    var amountMoney = refund.GetProperty("amount_money");
                    
                    return new RefundResult
                    {
                        Success = true,
                        RefundId = refund.GetProperty("id").GetString() ?? string.Empty,
                        OriginalTransactionId = request.TransactionId,
                        Amount = amountMoney.GetProperty("amount").GetInt64() / 100.0m,
                        Currency = amountMoney.GetProperty("currency").GetString() ?? string.Empty,
                        Status = refund.GetProperty("status").GetString() ?? string.Empty,
                        ProcessedAt = DateTime.UtcNow,
                        RawResponse = responseBody
                    };
                }
                else
                {
                    var error = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var errors = error.GetProperty("errors");
                    var firstError = errors[0];
                    
                    return new RefundResult
                    {
                        Success = false,
                        ErrorMessage = firstError.GetProperty("detail").GetString() ?? "Unknown error",
                        ErrorCode = firstError.GetProperty("code").GetString() ?? string.Empty,
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

                var response = await _httpClient.GetAsync($"/v2/payments/{transactionId}");
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var payment = result.GetProperty("payment");
                    var amountMoney = payment.GetProperty("amount_money");
                    
                    return new TransactionStatusResult
                    {
                        TransactionId = payment.GetProperty("id").GetString() ?? string.Empty,
                        Status = payment.GetProperty("status").GetString() ?? string.Empty,
                        Amount = amountMoney.GetProperty("amount").GetInt64() / 100.0m,
                        Currency = amountMoney.GetProperty("currency").GetString() ?? string.Empty,
                        CreatedAt = DateTime.TryParse(
                            payment.GetProperty("created_at").GetString(), 
                            out var created) ? created : DateTime.UtcNow,
                        UpdatedAt = DateTime.TryParse(
                            payment.GetProperty("updated_at").GetString(), 
                            out var updated) ? updated : DateTime.UtcNow,
                        CustomerEmail = payment.TryGetProperty("buyer_email_address", out var email) 
                            ? email.GetString() ?? string.Empty 
                            : string.Empty,
                        Description = payment.TryGetProperty("note", out var note) 
                            ? note.GetString() ?? string.Empty 
                            : string.Empty
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
        /// Creates a payment intent for later processing
        /// </summary>
        public async Task<PaymentIntentResult> CreatePaymentIntentAsync(PaymentIntentRequest request)
        {
            try
            {
                ValidatePaymentIntentRequest(request);

                var amountInCents = ConvertToSmallestUnit(request.Amount, request.Currency);

                // Square uses payments with delayed capture instead of payment intents
                var paymentRequest = new
                {
                    idempotency_key = Guid.NewGuid().ToString(),
                    amount_money = new
                    {
                        amount = amountInCents,
                        currency = request.Currency.ToUpper()
                    },
                    autocomplete = request.AutomaticCapture,
                    location_id = _locationId,
                    note = request.Description,
                    buyer_email_address = request.CustomerEmail
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(paymentRequest),
                    Encoding.UTF8,
                    "application/json"
                );

                // This creates an order that can be used for payment
                var response = await _httpClient.PostAsync("/v2/orders", jsonContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var order = result.GetProperty("order");
                    
                    return new PaymentIntentResult
                    {
                        Success = true,
                        IntentId = order.GetProperty("id").GetString() ?? string.Empty,
                        Status = order.GetProperty("state").GetString() ?? string.Empty,
                        Amount = request.Amount,
                        Currency = request.Currency,
                        CreatedAt = DateTime.TryParse(
                            order.GetProperty("created_at").GetString(), 
                            out var created) ? created : DateTime.UtcNow
                    };
                }
                else
                {
                    var error = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var errors = error.GetProperty("errors");
                    var firstError = errors[0];
                    
                    return new PaymentIntentResult
                    {
                        Success = false,
                        ErrorMessage = firstError.GetProperty("detail").GetString() ?? "Unknown error",
                        ErrorCode = firstError.GetProperty("code").GetString() ?? string.Empty,
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

                var captureRequest = new
                {
                    idempotency_key = Guid.NewGuid().ToString()
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(captureRequest),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync($"/v2/payments/{authorizationId}/complete", jsonContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var payment = result.GetProperty("payment");
                    var amountMoney = payment.GetProperty("amount_money");
                    
                    return new CaptureResult
                    {
                        Success = true,
                        TransactionId = payment.GetProperty("id").GetString() ?? string.Empty,
                        AuthorizationId = authorizationId,
                        Amount = amountMoney.GetProperty("amount").GetInt64() / 100.0m,
                        Currency = amountMoney.GetProperty("currency").GetString() ?? string.Empty,
                        Status = payment.GetProperty("status").GetString() ?? string.Empty,
                        ProcessedAt = DateTime.UtcNow
                    };
                }
                else
                {
                    var error = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var errors = error.GetProperty("errors");
                    var firstError = errors[0];
                    
                    return new CaptureResult
                    {
                        Success = false,
                        ErrorMessage = firstError.GetProperty("detail").GetString() ?? "Unknown error",
                        ErrorCode = firstError.GetProperty("code").GetString() ?? string.Empty,
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

                var response = await _httpClient.PostAsync($"/v2/payments/{authorizationId}/cancel", null);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var payment = result.GetProperty("payment");
                    
                    return new VoidResult
                    {
                        Success = true,
                        AuthorizationId = authorizationId,
                        Status = payment.GetProperty("status").GetString() ?? "voided",
                        ProcessedAt = DateTime.UtcNow
                    };
                }
                else
                {
                    var error = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var errors = error.GetProperty("errors");
                    var firstError = errors[0];
                    
                    return new VoidResult
                    {
                        Success = false,
                        ErrorMessage = firstError.GetProperty("detail").GetString() ?? "Unknown error",
                        ErrorCode = firstError.GetProperty("code").GetString() ?? string.Empty,
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
        /// Creates a customer profile in Square
        /// </summary>
        public async Task<CustomerProfileResult> CreateCustomerProfileAsync(CustomerProfileRequest request)
        {
            try
            {
                ValidateCustomerProfileRequest(request);

                var customerRequest = new
                {
                    idempotency_key = Guid.NewGuid().ToString(),
                    email_address = request.Email,
                    given_name = request.Name.Split(' ').FirstOrDefault() ?? request.Name,
                    family_name = request.Name.Split(' ').Skip(1).FirstOrDefault() ?? string.Empty,
                    phone_number = request.Phone,
                    note = request.Description,
                    address = request.Address != null ? new
                    {
                        address_line_1 = request.Address.Line1,
                        address_line_2 = request.Address.Line2,
                        locality = request.Address.City,
                        administrative_district_level_1 = request.Address.State,
                        postal_code = request.Address.PostalCode,
                        country = request.Address.Country
                    } : null
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(customerRequest),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync("/v2/customers", jsonContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var customer = result.GetProperty("customer");
                    
                    return new CustomerProfileResult
                    {
                        Success = true,
                        CustomerId = customer.GetProperty("id").GetString() ?? string.Empty,
                        Email = customer.TryGetProperty("email_address", out var email) 
                            ? email.GetString() ?? string.Empty 
                            : string.Empty,
                        Name = GetCustomerName(customer),
                        CreatedAt = DateTime.TryParse(
                            customer.GetProperty("created_at").GetString(), 
                            out var created) ? created : DateTime.UtcNow
                    };
                }
                else
                {
                    var error = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var errors = error.GetProperty("errors");
                    var firstError = errors[0];
                    
                    return new CustomerProfileResult
                    {
                        Success = false,
                        ErrorMessage = firstError.GetProperty("detail").GetString() ?? "Unknown error",
                        ErrorCode = firstError.GetProperty("code").GetString() ?? string.Empty,
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
                var beginTime = startDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
                var endTime = endDate.ToString("yyyy-MM-ddTHH:mm:ssZ");

                var searchRequest = new
                {
                    location_ids = new[] { _locationId },
                    begin_time = beginTime,
                    end_time = endTime,
                    limit = 100
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(searchRequest),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync("/v2/payments/search", jsonContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    
                    if (result.TryGetProperty("payments", out var payments))
                    {
                        var transactionList = new List<TransactionSummary>();

                        foreach (var payment in payments.EnumerateArray())
                        {
                            var amountMoney = payment.GetProperty("amount_money");
                            
                            transactionList.Add(new TransactionSummary
                            {
                                TransactionId = payment.GetProperty("id").GetString() ?? string.Empty,
                                Type = "payment",
                                Status = payment.GetProperty("status").GetString() ?? string.Empty,
                                Amount = amountMoney.GetProperty("amount").GetInt64() / 100.0m,
                                Currency = amountMoney.GetProperty("currency").GetString() ?? string.Empty,
                                CustomerEmail = payment.TryGetProperty("buyer_email_address", out var email) 
                                    ? email.GetString() ?? string.Empty 
                                    : string.Empty,
                                Description = payment.TryGetProperty("note", out var note) 
                                    ? note.GetString() ?? string.Empty 
                                    : string.Empty,
                                CreatedAt = DateTime.TryParse(
                                    payment.GetProperty("created_at").GetString(), 
                                    out var created) ? created : DateTime.UtcNow,
                                UpdatedAt = DateTime.TryParse(
                                    payment.GetProperty("updated_at").GetString(), 
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

        private string GetCustomerName(JsonElement customer)
        {
            var givenName = customer.TryGetProperty("given_name", out var given) 
                ? given.GetString() ?? string.Empty 
                : string.Empty;
            var familyName = customer.TryGetProperty("family_name", out var family) 
                ? family.GetString() ?? string.Empty 
                : string.Empty;
            
            return $"{givenName} {familyName}".Trim();
        }
    }
}
