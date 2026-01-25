using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using PaymentGatewayLib.Implementations;
using PaymentGatewayLib.Models;
using Xunit;

namespace PaymentGatewayLib.Tests
{
    /// <summary>
    /// Unit tests for IPayAfricaPaymentGateway
    /// Demonstrates the class is ready for comprehensive testing
    /// </summary>
    public class IPayAfricaPaymentGatewayTests
    {
        private const string TestVendorId = "demo";
        private const string TestHashKey = "demoCHANGED";
        
        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Arrange & Act
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey, useTestMode: true);

            // Assert
            Assert.NotNull(gateway);
            Assert.Equal(TestVendorId, gateway.VendorId);
            Assert.True(gateway.UseTestMode);
            Assert.Equal(IPayAfricaPaymentGateway.DemoUrl, gateway.PaymentUrl);
        }

        [Fact]
        public void Constructor_WithLiveMode_SetsLiveUrl()
        {
            // Arrange & Act
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey, useTestMode: false);

            // Assert
            Assert.False(gateway.UseTestMode);
            Assert.Equal(IPayAfricaPaymentGateway.LiveUrl, gateway.PaymentUrl);
        }

        [Fact]
        public void Constructor_WithNullVendorId_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new IPayAfricaPaymentGateway(null, TestHashKey));
        }

        [Fact]
        public void Constructor_WithNullHashKey_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new IPayAfricaPaymentGateway(TestVendorId, null));
        }

        #endregion

        #region ProcessPaymentAsync Tests

        [Fact]
        public async Task ProcessPaymentAsync_WithValidRequest_ReturnsSuccess()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var request = CreateValidPaymentRequest();

            // Act
            var result = await gateway.ProcessPaymentAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("pending", result.Status);
            Assert.Equal(request.Amount, result.Amount);
            Assert.Equal(request.Currency, result.Currency);
            Assert.NotNull(result.TransactionId);
            Assert.NotEmpty(result.Metadata);
            Assert.True(result.Metadata.ContainsKey("payment_url"));
            Assert.True(result.Metadata.ContainsKey("hash"));
            Assert.True(result.Metadata.ContainsKey("vendor_id"));
        }

        [Fact]
        public async Task ProcessPaymentAsync_WithoutEmail_ReturnsError()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var request = CreateValidPaymentRequest();
            request.CustomerEmail = "";

            // Act
            var result = await gateway.ProcessPaymentAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("email", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ProcessPaymentAsync_WithInvalidCurrency_ThrowsArgumentException()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var request = CreateValidPaymentRequest();
            request.Currency = "XXX"; // Invalid currency

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                async () => await gateway.ProcessPaymentAsync(request));
            Assert.Contains("not supported", exception.Message);
        }

        [Fact]
        public async Task ProcessPaymentAsync_WithPhoneInMetadata_IncludesPhone()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var request = CreateValidPaymentRequest();
            request.Metadata["phone"] = "+254712345678";

            // Act
            var result = await gateway.ProcessPaymentAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("+254712345678", result.Metadata["phone"]);
        }

        [Fact]
        public async Task ProcessPaymentAsync_GeneratesValidHash()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var request = CreateValidPaymentRequest();

            // Act
            var result = await gateway.ProcessPaymentAsync(request);

            // Assert
            Assert.NotNull(result.Metadata["hash"]);
            Assert.Matches("^[a-f0-9]{40}$", result.Metadata["hash"]); // SHA1 is 40 hex chars
        }

        #endregion

        #region Validation Tests

        [Theory]
        [InlineData("KES")]
        [InlineData("USD")]
        [InlineData("GBP")]
        [InlineData("EUR")]
        [InlineData("ZAR")]
        public async Task ProcessPaymentAsync_WithSupportedCurrency_Succeeds(string currency)
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var request = CreateValidPaymentRequest();
            request.Currency = currency;

            // Act
            var result = await gateway.ProcessPaymentAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(currency, result.Currency);
        }

        [Theory]
        [InlineData("test@example.com", true)]
        [InlineData("user.name@domain.co.ke", true)]
        [InlineData("invalid-email", false)]
        [InlineData("@domain.com", false)]
        [InlineData("user@", false)]
        [InlineData("", false)]
        public void IsValidEmail_WithVariousInputs_ReturnsExpected(string email, bool expected)
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);

            // Act
            var result = gateway.IsValidEmail(email);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region GetTransactionStatusAsync Tests

        [Fact]
        public async Task GetTransactionStatusAsync_WithValidId_ReturnsStatus()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var transactionId = "TXN123456";

            // Act
            var result = await gateway.GetTransactionStatusAsync(transactionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(transactionId, result.TransactionId);
            Assert.Equal("pending", result.Status);
        }

        [Fact]
        public async Task GetTransactionStatusAsync_WithNullId_ThrowsArgumentException()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                async () => await gateway.GetTransactionStatusAsync(null));
        }

        [Fact]
        public async Task GetTransactionStatusAsync_WithEmptyId_ThrowsArgumentException()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                async () => await gateway.GetTransactionStatusAsync(""));
        }

        #endregion

        #region RefundPaymentAsync Tests

        [Fact]
        public async Task RefundPaymentAsync_WithValidRequest_ReturnsSuccess()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var request = new RefundRequest
            {
                TransactionId = "TXN123456",
                Amount = 100.00m,
                Reason = "Customer request"
            };

            // Act
            var result = await gateway.RefundPaymentAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(request.TransactionId, result.OriginalTransactionId);
            Assert.Equal("refunded", result.Status);
        }

        [Fact]
        public async Task RefundPaymentAsync_WithNullRequest_ReturnsError()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);

            // Act
            var result = await gateway.RefundPaymentAsync(null);

            // Assert
            Assert.False(result.Success);
            Assert.NotNull(result.ErrorMessage);
        }

        #endregion

        #region VerifyCallback Tests

        [Fact]
        public void VerifyCallback_WithValidData_ReturnsTrue()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var callbackData = new Dictionary<string, string>
            {
                ["status"] = IPayAfricaPaymentGateway.StatusSuccess,
                ["txncd"] = "TXN123456"
            };

            // Act
            var result = gateway.VerifyCallback(callbackData);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyCallback_WithoutStatus_ReturnsFalse()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var callbackData = new Dictionary<string, string>
            {
                ["txncd"] = "TXN123456"
            };

            // Act
            var result = gateway.VerifyCallback(callbackData);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyCallback_WithoutTxncd_ReturnsFalse()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var callbackData = new Dictionary<string, string>
            {
                ["status"] = IPayAfricaPaymentGateway.StatusSuccess
            };

            // Act
            var result = gateway.VerifyCallback(callbackData);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyCallback_WithNullData_ThrowsArgumentNullException()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => gateway.VerifyCallback(null));
        }

        #endregion

        #region ParseIPayStatus Tests

        [Theory]
        [InlineData("aei7p7yrx4ae34", "success")]
        [InlineData("dtfi4p7yty45wq", "failed")]
        [InlineData("bdi6p2yy76etrs", "pending")]
        [InlineData("unknown_code", "unknown")]
        public void ParseIPayStatus_WithVariousStatusCodes_ReturnsExpected(string statusCode, string expected)
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);

            // Act
            var result = gateway.ParseIPayStatus(statusCode);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region CapturePaymentAsync Tests

        [Fact]
        public async Task CapturePaymentAsync_WithValidAuthId_ReturnsSuccess()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var authId = "AUTH123456";

            // Act
            var result = await gateway.CapturePaymentAsync(authId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(authId, result.TransactionId);
        }

        [Fact]
        public async Task CapturePaymentAsync_WithNullAuthId_ThrowsArgumentException()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                async () => await gateway.CapturePaymentAsync(null));
        }

        #endregion

        #region VoidPaymentAsync Tests

        [Fact]
        public async Task VoidPaymentAsync_WithValidAuthId_ReturnsSuccess()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var authId = "AUTH123456";

            // Act
            var result = await gateway.VoidPaymentAsync(authId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(authId, result.AuthorizationId);
            Assert.Equal("voided", result.Status);
        }

        [Fact]
        public async Task VoidPaymentAsync_WithNullAuthId_ThrowsArgumentException()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                async () => await gateway.VoidPaymentAsync(null));
        }

        #endregion

        #region CreateCustomerProfileAsync Tests

        [Fact]
        public async Task CreateCustomerProfileAsync_WithValidRequest_ReturnsSuccess()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var request = new CustomerProfileRequest
            {
                Email = "customer@example.com",
                Name = "John Doe"
            };

            // Act
            var result = await gateway.CreateCustomerProfileAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(request.Email, result.Email);
            Assert.Equal(request.Name, result.Name);
            Assert.NotNull(result.CustomerId);
        }

        #endregion

        #region GetTransactionsAsync Tests

        [Fact]
        public async Task GetTransactionsAsync_WithValidDates_ReturnsEmpty()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow;

            // Act
            var result = await gateway.GetTransactionsAsync(startDate, endDate);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTransactionsAsync_WithInvalidDates_ThrowsArgumentException()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var startDate = DateTime.UtcNow;
            var endDate = DateTime.UtcNow.AddDays(-30);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                async () => await gateway.GetTransactionsAsync(startDate, endDate));
        }

        #endregion

        #region Helper Methods

        private PaymentRequest CreateValidPaymentRequest()
        {
            return new PaymentRequest
            {
                Amount = 1000.00m,
                Currency = "KES",
                CustomerEmail = "customer@example.com",
                CustomerName = "John Doe",
                Description = "Test Payment",
                PaymentMethodToken = "test-token",
                ReferenceId = "REF" + Guid.NewGuid().ToString().Substring(0, 8),
                Metadata = new Dictionary<string, string>
                {
                    ["callback_url"] = "https://example.com/callback"
                }
            };
        }

        #endregion

        #region BuildPaymentParameters Tests

        [Fact]
        public void BuildPaymentParameters_IncludesAllRequiredFields()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey, useTestMode: true);
            var request = CreateValidPaymentRequest();
            var phone = "+254712345678";

            // Act
            var parameters = gateway.BuildPaymentParameters(request, phone);

            // Assert
            Assert.NotNull(parameters);
            Assert.Equal("0", parameters["live"]); // Test mode
            Assert.Equal(TestVendorId, parameters["vid"]);
            Assert.Equal(request.Amount.ToString("F2"), parameters["ttl"]);
            Assert.Equal(phone, parameters["tel"]);
            Assert.Equal(request.CustomerEmail, parameters["eml"]);
            Assert.Equal(request.Currency, parameters["curr"]);
        }

        [Fact]
        public void BuildPaymentParameters_WithLiveMode_SetsLiveFlag()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey, useTestMode: false);
            var request = CreateValidPaymentRequest();

            // Act
            var parameters = gateway.BuildPaymentParameters(request, "");

            // Assert
            Assert.Equal("1", parameters["live"]);
        }

        #endregion

        #region GenerateHash Tests

        [Fact]
        public void GenerateHash_ProducesConsistentOutput()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var parameters = new Dictionary<string, string>
            {
                ["live"] = "0",
                ["oid"] = "ORD123",
                ["inv"] = "INV123",
                ["ttl"] = "1000.00",
                ["tel"] = "+254712345678",
                ["eml"] = "test@example.com",
                ["vid"] = TestVendorId,
                ["curr"] = "KES",
                ["p1"] = "Test",
                ["p2"] = "",
                ["p3"] = "",
                ["p4"] = "",
                ["cbk"] = "https://example.com/callback"
            };

            // Act
            var hash1 = gateway.GenerateHash(parameters);
            var hash2 = gateway.GenerateHash(parameters);

            // Assert
            Assert.Equal(hash1, hash2);
            Assert.Matches("^[a-f0-9]{40}$", hash1); // SHA1 format
        }

        [Fact]
        public void GenerateHash_DifferentInputs_ProducesDifferentHashes()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var parameters1 = CreateHashParameters("1000.00");
            var parameters2 = CreateHashParameters("2000.00");

            // Act
            var hash1 = gateway.GenerateHash(parameters1);
            var hash2 = gateway.GenerateHash(parameters2);

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        private Dictionary<string, string> CreateHashParameters(string amount)
        {
            return new Dictionary<string, string>
            {
                ["live"] = "0",
                ["oid"] = "ORD123",
                ["inv"] = "INV123",
                ["ttl"] = amount,
                ["tel"] = "+254712345678",
                ["eml"] = "test@example.com",
                ["vid"] = TestVendorId,
                ["curr"] = "KES",
                ["p1"] = "",
                ["p2"] = "",
                ["p3"] = "",
                ["p4"] = "",
                ["cbk"] = ""
            };
        }

        #endregion

        #region GetSupportedCurrencies Tests

        [Fact]
        public void GetSupportedCurrencies_ReturnsExpectedCurrencies()
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);

            // Act
            var currencies = gateway.GetSupportedCurrencies();

            // Assert
            Assert.NotNull(currencies);
            Assert.Contains("KES", currencies);
            Assert.Contains("USD", currencies);
            Assert.Contains("GBP", currencies);
            Assert.Contains("EUR", currencies);
            Assert.Contains("ZAR", currencies);
        }

        #endregion

        #region GetTransactionStatusAsync with API Tests

        [Fact]
        public async Task GetTransactionStatusAsync_WithMockedHttpClient_ReturnsSuccess()
        {
            // Arrange
            var mockHttpClient = CreateMockHttpClient(new
            {
                status = "success",
                transaction_id = "TXN123456",
                amount = 1000.00,
                currency = "KES",
                customer_phone = "+254712345678"
            });

            var gateway = new IPayAfricaPaymentGateway(
                TestVendorId, 
                TestHashKey, 
                useTestMode: true,
                apiBaseUrl: "https://apis.ipayafrica.com",
                statusEndpoint: "/payments/v2/transact/mobilemoney/status",
                httpClient: mockHttpClient
            );

            // Act
            var result = await gateway.GetTransactionStatusAsync("TXN123456");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TXN123456", result.TransactionId);
            Assert.Equal("success", result.Status);
            Assert.True(result.Metadata.ContainsKey("amount"));
            Assert.Equal("1000", result.Metadata["amount"]);
        }

        [Fact]
        public async Task GetTransactionStatusAsync_WithPendingStatus_ReturnsPending()
        {
            // Arrange
            var mockHttpClient = CreateMockHttpClient(new
            {
                status = "pending",
                transaction_id = "TXN789"
            });

            var gateway = new IPayAfricaPaymentGateway(
                TestVendorId, 
                TestHashKey, 
                useTestMode: true,
                apiBaseUrl: "https://test.api.com",
                statusEndpoint: "/status",
                httpClient: mockHttpClient
            );

            // Act
            var result = await gateway.GetTransactionStatusAsync("TXN789");

            // Assert
            Assert.Equal("pending", result.Status);
        }

        [Fact]
        public async Task GetTransactionStatusAsync_WithFailedResponse_ReturnsUnknown()
        {
            // Arrange
            var mockHttpClient = CreateMockHttpClientWithError(System.Net.HttpStatusCode.InternalServerError);

            var gateway = new IPayAfricaPaymentGateway(
                TestVendorId, 
                TestHashKey, 
                useTestMode: true,
                apiBaseUrl: "https://test.api.com",
                statusEndpoint: "/status",
                httpClient: mockHttpClient
            );

            // Act
            var result = await gateway.GetTransactionStatusAsync("TXN999");

            // Assert
            Assert.Equal("unknown", result.Status);
            Assert.True(result.Metadata.ContainsKey("error"));
        }

        #endregion

        #region ParseApiStatus Tests

        [Theory]
        [InlineData("{\"status\":\"success\"}", "success")]
        [InlineData("{\"status\":\"completed\"}", "success")]
        [InlineData("{\"status\":\"pending\"}", "pending")]
        [InlineData("{\"status\":\"failed\"}", "failed")]
        [InlineData("{\"status\":\"cancelled\"}", "failed")]
        [InlineData("{\"transaction_status\":\"processing\"}", "processing")]
        public void ParseApiStatus_WithVariousFormats_ReturnsExpected(string json, string expected)
        {
            // Arrange
            var gateway = new IPayAfricaPaymentGateway(TestVendorId, TestHashKey);
            var jsonDoc = System.Text.Json.JsonDocument.Parse(json);

            // Act
            var result = gateway.ParseApiStatus(jsonDoc.RootElement);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region Factory Method Tests

        [Fact]
        public void FromSettings_WithValidSettings_CreatesInstance()
        {
            // Arrange
            var settings = new PaymentGatewayLib.Infrastructure.Settings.IPayAfricaSettings
            {
                VendorId = TestVendorId,
                HashKey = TestHashKey,
                UseTestMode = true,
                ApiBaseUrl = "https://apis.ipayafrica.com",
                StatusEndpoint = "/payments/v2/transact/mobilemoney/status"
            };

            // Act
            var gateway = IPayAfricaPaymentGateway.FromSettings(settings);

            // Assert
            Assert.NotNull(gateway);
            Assert.Equal(TestVendorId, gateway.VendorId);
            Assert.True(gateway.UseTestMode);
        }

        [Fact]
        public void FromSettings_WithNullSettings_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                IPayAfricaPaymentGateway.FromSettings(null));
        }

        #endregion

        #region Helper Methods for HTTP Mocking

        private HttpClient CreateMockHttpClient(object responseObject)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(responseObject);
            var mockHandler = new MockHttpMessageHandler(json, System.Net.HttpStatusCode.OK);
            return new HttpClient(mockHandler)
            {
                BaseAddress = new Uri("https://test.api.com")
            };
        }

        private HttpClient CreateMockHttpClientWithError(System.Net.HttpStatusCode statusCode)
        {
            var mockHandler = new MockHttpMessageHandler("{\"error\":\"Internal error\"}", statusCode);
            return new HttpClient(mockHandler)
            {
                BaseAddress = new Uri("https://test.api.com")
            };
        }

        // Simple mock HTTP handler for testing
        private class MockHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _response;
            private readonly System.Net.HttpStatusCode _statusCode;

            public MockHttpMessageHandler(string response, System.Net.HttpStatusCode statusCode)
            {
                _response = response;
                _statusCode = statusCode;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, 
                System.Threading.CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = _statusCode,
                    Content = new StringContent(_response, System.Text.Encoding.UTF8, "application/json")
                });
            }
        }

        #endregion
    }
}
