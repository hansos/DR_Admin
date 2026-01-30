using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ISPAdmin.DTOs;
using Xunit;


namespace DR_Admin.IntegrationTests.Controllers;

[Collection("Sequential")]
public class MyAccountControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    private readonly TestWebApplicationFactory _factory;

    public MyAccountControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Registration Tests

    [Fact]
    [Trait("Category", "MyAccount")]
    [Trait("Priority", "1")]
    public async Task Register_ValidRequest_ReturnsCreatedWithRegistrationDetails()
    {
        // Arrange
        var request = new RegisterAccountRequestDto
        {
            Username = "testuser",
            Email = "testuser@example.com",
            Password = "Test@1234",
            ConfirmPassword = "Test@1234",
            CustomerName = "Test Customer",
            CustomerEmail = "customer@example.com",
            CustomerPhone = "555-0100",
            CustomerAddress = "123 Test St"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/MyAccount/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<RegisterAccountResponseDto>();
        Assert.NotNull(result);
        Assert.True(result.UserId > 0);
        Assert.Equal(request.Email, result.Email);
        Assert.NotNull(result.EmailConfirmationToken);

        Console.WriteLine($"Registered User ID: {result.UserId}");
        Console.WriteLine($"Email: {result.Email}");
        Console.WriteLine($"Confirmation Token: {result.EmailConfirmationToken}");

        // Store for later tests
        TestTokenStorage.UserId = result.UserId;
        TestTokenStorage.UserEmail = result.Email;
    }

    [Fact]
    [Trait("Category", "MyAccount")]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        // Arrange - First registration
        var request1 = new RegisterAccountRequestDto
        {
            Username = "user1",
            Email = "duplicate@example.com",
            Password = "Test@1234",
            ConfirmPassword = "Test@1234",
            CustomerName = "Customer 1",
            CustomerEmail = "customer1@example.com",
            CustomerPhone = "555-0101",
            CustomerAddress = "123 Test St"
        };

        await _client.PostAsJsonAsync("/api/v1/MyAccount/register", request1);

        // Act - Second registration with same email
        var request2 = new RegisterAccountRequestDto
        {
            Username = "user2",
            Email = "duplicate@example.com",
            Password = "Test@1234",
            ConfirmPassword = "Test@1234",
            CustomerName = "Customer 2",
            CustomerEmail = "customer2@example.com",
            CustomerPhone = "555-0102",
            CustomerAddress = "456 Test Ave"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/MyAccount/register", request2);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "MyAccount")]
    public async Task Register_MismatchedPasswords_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterAccountRequestDto
        {
            Username = "testuser2",
            Email = "testuser2@example.com",
            Password = "Test@1234",
            ConfirmPassword = "DifferentPassword",
            CustomerName = "Test Customer 2",
            CustomerEmail = "customer2@example.com",
            CustomerPhone = "555-0103",
            CustomerAddress = "789 Test Blvd"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/MyAccount/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Email Confirmation Tests

    [Fact]
    [Trait("Category", "MyAccount")]
    [Trait("Priority", "2")]
    public async Task ConfirmEmail_ValidToken_ReturnsOk()
    {
        // Arrange - First register a user
        var registerRequest = new RegisterAccountRequestDto
        {
            Username = "confirmuser",
            Email = "confirmuser@example.com",
            Password = "Test@1234",
            ConfirmPassword = "Test@1234",
            CustomerName = "Confirm User",
            CustomerEmail = "confirm@example.com",
            CustomerPhone = "555-0104",
            CustomerAddress = "321 Confirm St"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/MyAccount/register", registerRequest);
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<RegisterAccountResponseDto>();

        Assert.NotNull(registerResult);
        Assert.NotNull(registerResult.EmailConfirmationToken);

        // Act - Confirm email
        var confirmRequest = new ConfirmEmailRequestDto
        {
            Email = registerResult.Email,
            ConfirmationToken = registerResult.EmailConfirmationToken
        };

        var response = await _client.PostAsJsonAsync("/api/v1/MyAccount/confirm-email", confirmRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Confirm Email Response: {content}");
    }

    [Fact]
    [Trait("Category", "MyAccount")]
    public async Task ConfirmEmail_InvalidToken_ReturnsBadRequest()
    {
        // Arrange
        var request = new ConfirmEmailRequestDto
        {
            Email = "test@example.com",
            ConfirmationToken = "invalid-token-12345"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/MyAccount/confirm-email", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Get My Account Tests

    [Fact]
    [Trait("Category", "MyAccount")]
    [Trait("Priority", "4")]
    public async Task GetMyAccount_Authenticated_ReturnsUserInfo()
    {
        // Arrange - Get a valid token
        await EnsureAuthenticatedUser();

        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", TestTokenStorage.AccessToken);

        // Act
        var response = await _client.GetAsync("/api/v1/MyAccount/me");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<UserAccountDto>();
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.NotEmpty(result.Email);
        Assert.NotEmpty(result.Username);
        Assert.NotNull(result.Customer);

        Console.WriteLine($"User ID: {result.Id}");
        Console.WriteLine($"Username: {result.Username}");
        Console.WriteLine($"Email: {result.Email}");
        Console.WriteLine($"Customer: {result.Customer.Name}");
    }

    [Fact]
    [Trait("Category", "MyAccount")]
    public async Task GetMyAccount_Unauthenticated_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/MyAccount/me");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Change Password Tests

    [Fact]
    [Trait("Category", "MyAccount")]
    public async Task ChangePassword_ValidRequest_ReturnsOk()
    {
        // Arrange - Get a valid token
        var userEmail = await EnsureAuthenticatedUser();

        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", TestTokenStorage.AccessToken);

        var request = new ChangePasswordRequestDto
        {
            CurrentPassword = "Test@1234",
            NewPassword = "NewTest@5678",
            ConfirmPassword = "NewTest@5678"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/MyAccount/change-password", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Console.WriteLine("Password changed successfully");
    }

    [Fact]
    [Trait("Category", "MyAccount")]
    public async Task ChangePassword_MismatchedPasswords_ReturnsBadRequest()
    {
        // Arrange
        var userEmail = await EnsureAuthenticatedUser();

        // Verify token was created
        Assert.NotNull(TestTokenStorage.AccessToken);
        Assert.NotEmpty(TestTokenStorage.AccessToken);

        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", TestTokenStorage.AccessToken);

        var request = new ChangePasswordRequestDto
        {
            CurrentPassword = "Test@1234",
            NewPassword = "NewTest@5678",
            ConfirmPassword = "DifferentPassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/MyAccount/change-password", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "MyAccount")]
    public async Task ChangePassword_WrongCurrentPassword_ReturnsBadRequest()
    {
        // Arrange
        var userEmail = await EnsureAuthenticatedUser();

        // Verify token was created
        Assert.NotNull(TestTokenStorage.AccessToken);
        Assert.NotEmpty(TestTokenStorage.AccessToken);

        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", TestTokenStorage.AccessToken);

        var request = new ChangePasswordRequestDto
        {
            CurrentPassword = "WrongPassword",
            NewPassword = "NewTest@5678",
            ConfirmPassword = "NewTest@5678"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/MyAccount/change-password", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Ensures we have an authenticated user with valid tokens
    /// Returns the user's email address
    /// </summary>
    private async Task<string> EnsureAuthenticatedUser()
    {
        // Check if we already have valid tokens
        if (TestTokenStorage.HasValidAccessToken())
        {
            Console.WriteLine("Using existing valid access token");
            return TestTokenStorage.UserEmail!;
        }

        Console.WriteLine("Creating new authenticated user...");

        // Create a new user for testing
        var timestamp = DateTime.UtcNow.Ticks;
        var username = $"authuser{timestamp}";
        var email = $"authuser{timestamp}@example.com";

        var registerRequest = new RegisterAccountRequestDto
        {
            Username = username,
            Email = email,
            Password = "Test@1234",
            ConfirmPassword = "Test@1234",
            CustomerName = "Auth Test User",
            CustomerEmail = email,
            CustomerPhone = "555-0199",
            CustomerAddress = "999 Auth St"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/MyAccount/register", registerRequest);
        
        if (!registerResponse.IsSuccessStatusCode)
        {
            var errorContent = await registerResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Registration failed: {registerResponse.StatusCode} - {errorContent}");
            throw new Exception($"Registration failed: {registerResponse.StatusCode}");
        }

        var registerResult = await registerResponse.Content.ReadFromJsonAsync<RegisterAccountResponseDto>();
        Assert.NotNull(registerResult);
        Assert.NotNull(registerResult.EmailConfirmationToken);

        // Confirm email
        var confirmRequest = new ConfirmEmailRequestDto
        {
            Email = registerResult.Email,
            ConfirmationToken = registerResult.EmailConfirmationToken
        };
        var confirmResponse = await _client.PostAsJsonAsync("/api/v1/MyAccount/confirm-email", confirmRequest);
        
        if (!confirmResponse.IsSuccessStatusCode)
        {
            var errorContent = await confirmResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Email confirmation failed: {confirmResponse.StatusCode} - {errorContent}");
            throw new Exception($"Email confirmation failed: {confirmResponse.StatusCode}");
        }

        // Login to get tokens using AuthController
        var loginRequest = new LoginRequestDto
        {
            Username = username,  // Use the actual username, not the email
            Password = "Test@1234"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);
        
        if (!loginResponse.IsSuccessStatusCode)
        {
            var errorContent = await loginResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Login failed: {loginResponse.StatusCode} - {errorContent}");
            throw new Exception($"Login failed: {loginResponse.StatusCode}");
        }

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        Assert.NotNull(loginResult);
        Assert.NotEmpty(loginResult.AccessToken);
        Assert.NotEmpty(loginResult.RefreshToken);

        // Store tokens
        TestTokenStorage.AccessToken = loginResult.AccessToken;
        TestTokenStorage.RefreshToken = loginResult.RefreshToken;
        TestTokenStorage.AccessTokenExpiresAt = loginResult.ExpiresAt;
        TestTokenStorage.RefreshTokenExpiresAt = loginResult.ExpiresAt.AddDays(7); // Refresh tokens typically last 7 days
        TestTokenStorage.UserId = registerResult.UserId;
        TestTokenStorage.UserEmail = email;

        Console.WriteLine($"Created and authenticated user: {email}");
        Console.WriteLine($"Access Token: {loginResult.AccessToken[..20]}...");

        return email;
    }

    #endregion
}

