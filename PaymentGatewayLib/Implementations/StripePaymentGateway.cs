using PaymentGatewayLib.Infrastructure.Settings;
using PaymentGatewayLib.Interfaces;
using PaymentGatewayLib.Models;
using Serilog;
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
        private readonly ILogger _logger;
        private readonly string _secretKey;
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;

        public StripePaymentGateway(string secretKey, string publishableKey, string apiBaseUrl = "https://api.stripe.com")
        {
            _logger = Log.ForContext<StripePaymentGateway>();
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

                if (transactionId.StartsWith("pi_", StringComparison.OrdinalIgnoreCase))
                {
                    var intentResponse = await _httpClient.GetAsync($"/v1/payment_intents/{transactionId}");
                    var intentBody = await intentResponse.Content.ReadAsStringAsync();

                    if (!intentResponse.IsSuccessStatusCode)
                    {
                        throw new Exception($"Failed to retrieve payment intent status: {intentBody}");
                    }

                    var intent = JsonSerializer.Deserialize<JsonElement>(intentBody);
                    var amountValue = intent.TryGetProperty("amount_received", out var amountReceived)
                        ? amountReceived.GetInt64()
                        : intent.GetProperty("amount").GetInt64();

                    return new TransactionStatusResult
                    {
                        TransactionId = intent.GetProperty("id").GetString() ?? string.Empty,
                        Status = intent.GetProperty("status").GetString() ?? string.Empty,
                        Amount = amountValue / 100.0m,
                        Currency = intent.GetProperty("currency").GetString()?.ToUpper() ?? string.Empty,
                        CreatedAt = DateTimeOffset.FromUnixTimeSeconds(intent.GetProperty("created").GetInt64()).UtcDateTime,
                        UpdatedAt = DateTime.UtcNow,
                        Description = intent.TryGetProperty("description", out var piDesc)
                            ? piDesc.GetString() ?? string.Empty
                            : string.Empty
                    };
                }

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
                _logger.Error($"Error retrieving transaction status for {transactionId}: {ex.Message}");
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
                    { "currency", request.Currency.ToLower() }
                };

                var explicitMethodTypes = ResolveStripePaymentMethodTypes(request).ToList();
                if (explicitMethodTypes.Count == 0)
                {
                    parameters.Add("automatic_payment_methods[enabled]", "true");
                }
                else
                {
                    for (var i = 0; i < explicitMethodTypes.Count; i++)
                    {
                        parameters.Add($"payment_method_types[{i}]", explicitMethodTypes[i]);
                    }
                }

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
                    var customerId = request.CustomerId.Trim();
                    if (customerId.StartsWith("cus_", StringComparison.OrdinalIgnoreCase))
                    {
                        parameters.Add("customer", customerId);
                    }
                    else
                    {
                        parameters.Add("metadata[internal_customer_id]", customerId);
                    }
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
                    var errorObj = error.TryGetProperty("error", out var parsedErrorObj)
                        ? parsedErrorObj
                        : error;
                    var errorMessage = errorObj.TryGetProperty("message", out var messageProperty)
                        ? messageProperty.GetString() ?? "Unknown error"
                        : "Unknown error";
                    var errorCode = errorObj.TryGetProperty("code", out var codeProperty)
                        ? codeProperty.GetString() ?? string.Empty
                        : string.Empty;
                    
                    return new PaymentIntentResult
                    {
                        Success = false,
                        ErrorMessage = errorMessage,
                        ErrorCode = errorCode,
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

        public async Task<StripeSubscriptionCreateResult> CreateRecurringSubscriptionAsync(StripeSubscriptionCreateRequest request)
        {
            try
            {
                if (request.Amount <= 0)
                {
                    return new StripeSubscriptionCreateResult
                    {
                        Success = false,
                        ErrorMessage = "Subscription amount must be greater than zero"
                    };
                }

                var customerId = await EnsureCustomerIdAsync(request);
                if (string.IsNullOrWhiteSpace(customerId))
                {
                    return new StripeSubscriptionCreateResult
                    {
                        Success = false,
                        ErrorMessage = "Unable to resolve Stripe customer"
                    };
                }

                var productContent = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "name", string.IsNullOrWhiteSpace(request.ProductName) ? "Recurring Subscription" : request.ProductName }
                });

                var productResponse = await _httpClient.PostAsync("/v1/products", productContent);
                var productBody = await productResponse.Content.ReadAsStringAsync();
                if (!productResponse.IsSuccessStatusCode)
                {
                    return new StripeSubscriptionCreateResult
                    {
                        Success = false,
                        ErrorMessage = productBody
                    };
                }

                var product = JsonSerializer.Deserialize<JsonElement>(productBody);
                var productId = product.GetProperty("id").GetString() ?? string.Empty;

                var priceParams = new Dictionary<string, string>
                {
                    { "unit_amount", ConvertToSmallestUnit(request.Amount, request.Currency).ToString() },
                    { "currency", request.Currency.ToLowerInvariant() },
                    { "recurring[interval]", string.IsNullOrWhiteSpace(request.Interval) ? "month" : request.Interval.ToLowerInvariant() },
                    { "recurring[interval_count]", Math.Max(1, request.IntervalCount).ToString() },
                    { "product", productId }
                };

                var priceContent = new FormUrlEncodedContent(priceParams);
                var priceResponse = await _httpClient.PostAsync("/v1/prices", priceContent);
                var priceBody = await priceResponse.Content.ReadAsStringAsync();
                if (!priceResponse.IsSuccessStatusCode)
                {
                    return new StripeSubscriptionCreateResult
                    {
                        Success = false,
                        ErrorMessage = priceBody
                    };
                }

                var price = JsonSerializer.Deserialize<JsonElement>(priceBody);
                var priceId = price.GetProperty("id").GetString() ?? string.Empty;

                var subscriptionParams = new Dictionary<string, string>
                {
                    { "customer", customerId },
                    { "items[0][price]", priceId },
                    { "collection_method", "charge_automatically" }
                };

                if (request.TrialEndUtc.HasValue && request.TrialEndUtc.Value > DateTime.UtcNow)
                {
                    subscriptionParams.Add("trial_end", new DateTimeOffset(request.TrialEndUtc.Value).ToUnixTimeSeconds().ToString());
                }

                if (request.Metadata.Count > 0)
                {
                    foreach (var kv in request.Metadata)
                    {
                        subscriptionParams[$"metadata[{kv.Key}]"] = kv.Value;
                    }
                }

                var subscriptionContent = new FormUrlEncodedContent(subscriptionParams);
                var subscriptionResponse = await _httpClient.PostAsync("/v1/subscriptions", subscriptionContent);
                var subscriptionBody = await subscriptionResponse.Content.ReadAsStringAsync();

                if (!subscriptionResponse.IsSuccessStatusCode)
                {
                    return new StripeSubscriptionCreateResult
                    {
                        Success = false,
                        ErrorMessage = subscriptionBody
                    };
                }

                var subscription = JsonSerializer.Deserialize<JsonElement>(subscriptionBody);
                return new StripeSubscriptionCreateResult
                {
                    Success = true,
                    SubscriptionId = subscription.GetProperty("id").GetString() ?? string.Empty,
                    CustomerId = customerId,
                    Status = subscription.TryGetProperty("status", out var status) ? status.GetString() ?? string.Empty : string.Empty
                };
            }
            catch (Exception ex)
            {
                return new StripeSubscriptionCreateResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<string> EnsureCustomerIdAsync(StripeSubscriptionCreateRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.CustomerId) && request.CustomerId.StartsWith("cus_", StringComparison.OrdinalIgnoreCase))
            {
                return request.CustomerId;
            }

            if (!string.IsNullOrWhiteSpace(request.CustomerEmail))
            {
                var encodedEmail = Uri.EscapeDataString(request.CustomerEmail);
                var lookup = await _httpClient.GetAsync($"/v1/customers?email={encodedEmail}&limit=1");
                var lookupBody = await lookup.Content.ReadAsStringAsync();
                if (lookup.IsSuccessStatusCode)
                {
                    var lookupJson = JsonSerializer.Deserialize<JsonElement>(lookupBody);
                    if (lookupJson.TryGetProperty("data", out var customers) && customers.GetArrayLength() > 0)
                    {
                        return customers[0].GetProperty("id").GetString() ?? string.Empty;
                    }
                }
            }

            var createCustomerResult = await CreateCustomerProfileAsync(new CustomerProfileRequest
            {
                Email = request.CustomerEmail,
                Name = string.IsNullOrWhiteSpace(request.CustomerName) ? request.CustomerEmail : request.CustomerName,
                Description = "Auto-created for recurring subscription"
            });

            return createCustomerResult.Success ? createCustomerResult.CustomerId : string.Empty;
        }

        private static IEnumerable<string> ResolveStripePaymentMethodTypes(PaymentIntentRequest request)
        {
            var values = new List<string>();

            if (!string.IsNullOrWhiteSpace(request.PreferredPaymentMethodType))
            {
                values.Add(request.PreferredPaymentMethodType);
            }

            if (request.PaymentMethodTypes is { Count: > 0 })
            {
                values.AddRange(request.PaymentMethodTypes.Where(v => !string.IsNullOrWhiteSpace(v)));
            }

            return values
                .Select(MapToStripePaymentMethodType)
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(v => v.ToLowerInvariant());
        }

        private static string MapToStripePaymentMethodType(string value)
        {
            return value.Trim().ToLowerInvariant() switch
            {
                "creditcard" or "credit card" or "card" => "card",
                "bankaccount" or "bank account" => "us_bank_account",
                "paypal" => "paypal",
                "sepa" or "sepadebit" or "sepa_debit" => "sepa_debit",
                "ideal" => "ideal",
                "sofort" => "sofort",
                _ => value.Trim().ToLowerInvariant()
            };
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
