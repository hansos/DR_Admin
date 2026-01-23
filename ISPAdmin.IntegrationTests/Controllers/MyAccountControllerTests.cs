using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ISPAdmin.DTOs;
using Xunit;
using Xunit.Abstractions;

namespace ISPAdmin.IntegrationTests.Controllers;

[Collection("Sequential")]
public class MyAccountControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private readonly TestWebApplicationFactory _factory;

    public MyAccountControllerTests(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _output = output;
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

        _output.WriteLine($"Registered User ID: {result.UserId}");
        _output.WriteLine($"Email: {result.Email}");
        _output.WriteLine($"Confirmation Token: {result.EmailConfirmationToken}");

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
        _output.WriteLine($"Confirm Email Response: {content}");
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

    #region Login Tests

    [Fact]
    [Trait("Category", "MyAccount")]
    [Trait("Priority", "3")]
    public async Task Login_ValidCredentials_ReturnsTokens()
    {
        // Arrange - Register and confirm a user first
        var registerRequest = new RegisterAccountRequestDto
        {
            Username = "loginuser",
            Email = "loginuser@example.com",
            Password = "Test@1234",
            ConfirmPassword = "Test@1234",
            CustomerName = "Login User",
            CustomerEmail = "loginuser@example.com",
            CustomerPhone = "555-0105",
            CustomerAddress = "456 Login Ave"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/MyAccount/register", registerRequest);
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<RegisterAccountResponseDto>();

        Assert.NotNull(registerResult);
        Assert.NotNull(registerResult.EmailConfirmationToken);

        // Confirm email
        var confirmRequest = new ConfirmEmailRequestDto
        {
            Email = registerResult.Email,
            ConfirmationToken = registerResult.EmailConfirmationToken
        };
        await _client.PostAsJsonAsync("/api/v1/MyAccount/confirm-email", confirmRequest);

        // Act - Login
        var loginRequest = new MyAccountLoginRequestDto
        {
            Email = "loginuser@example.com",
            Password = "Test@1234"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/MyAccount/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<MyAccountLoginResponseDto>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.True(result.AccessTokenExpiresAt > DateTime.UtcNow);
        Assert.True(result.RefreshTokenExpiresAt > DateTime.UtcNow);
        Assert.NotNull(result.User);
        Assert.Equal("loginuser@example.com", result.User.Email);

        _output.WriteLine($"Access Token: {result.AccessToken}");
        _output.WriteLine($"Refresh Token: {result.RefreshToken}");
        _output.WriteLine($"Access Token Expires: {result.AccessTokenExpiresAt}");
        _output.WriteLine($"Refresh Token Expires: {result.RefreshTokenExpiresAt}");

        // Store tokens in TestTokenStorage for other test classes to use
        TestTokenStorage.AccessToken = result.AccessToken;
        TestTokenStorage.RefreshToken = result.RefreshToken;
        TestTokenStorage.AccessTokenExpiresAt = result.AccessTokenExpiresAt;
        TestTokenStorage.RefreshTokenExpiresAt = result.RefreshTokenExpiresAt;
        TestTokenStorage.UserId = result.User.Id;
        TestTokenStorage.UserEmail = result.User.Email;

        _output.WriteLine($"\n? Tokens stored in TestTokenStorage for use by other test classes");
    }

    [Fact]
    [Trait("Category", "MyAccount")]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var request = new MyAccountLoginRequestDto
        {
            Email = "nonexistent@example.com",
            Password = "WrongPassword123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/MyAccount/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "MyAccount")]
    public async Task Login_EmptyEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new MyAccountLoginRequestDto
        {
            Email = "",
            Password = "Test@1234"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/MyAccount/login", request);

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

        _output.WriteLine($"User ID: {result.Id}");
        _output.WriteLine($"Username: {result.Username}");
        _output.WriteLine($"Email: {result.Email}");
        _output.WriteLine($"Customer: {result.Customer.Name}");
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

        // Verify we can login with new password
        var loginRequest = new MyAccountLoginRequestDto
        {
            Email = userEmail,
            Password = "NewTest@5678"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/MyAccount/login", loginRequest);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
    }

    [Fact]
    [Trait("Category", "MyAccount")]
    public async Task ChangePassword_MismatchedPasswords_ReturnsBadRequest()
    {
        // Arrange
        await EnsureAuthenticatedUser();

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
        await EnsureAuthenticatedUser();

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

    #region Refresh Token Tests

    [Fact]
    [Trait("Category", "MyAccount")]
    [Trait("Priority", "5")]
    public async Task RefreshToken_ValidToken_ReturnsNewTokens()
    {
        // Arrange - Get a valid refresh token
        await EnsureAuthenticatedUser();

        var request = new RefreshTokenRequestDto
        {
            RefreshToken = TestTokenStorage.RefreshToken!
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/MyAccount/refresh-token", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<RefreshTokenResponseDto>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);

        _output.WriteLine($"New Access Token: {result.AccessToken}");
        _output.WriteLine($"New Refresh Token: {result.RefreshToken}");

        // Update stored tokens
        TestTokenStorage.AccessToken = result.AccessToken;
        TestTokenStorage.RefreshToken = result.RefreshToken;
    }

    [Fact]
    [Trait("Category", "MyAccount")]
    public async Task RefreshToken_InvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var request = new RefreshTokenRequestDto
        {
            RefreshToken = "invalid-refresh-token"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/MyAccount/refresh-token", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Logout Tests

    [Fact]
    [Trait("Category", "MyAccount")]
    public async Task Logout_ValidToken_ReturnsOk()
    {
        // Arrange - Get a valid refresh token
        await EnsureAuthenticatedUser();

        var request = new RefreshTokenRequestDto
        {
            RefreshToken = TestTokenStorage.RefreshToken!
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/MyAccount/logout", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify the token can't be used to refresh anymore
        var refreshResponse = await _client.PostAsJsonAsync("/api/v1/MyAccount/refresh-token", request);
        Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);
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
            _output.WriteLine("Using existing valid access token");
            return TestTokenStorage.UserEmail!;
        }

        _output.WriteLine("Creating new authenticated user...");

        // Create a new user for testing
        var timestamp = DateTime.UtcNow.Ticks;
        var email = $"authuser{timestamp}@example.com";

        var registerRequest = new RegisterAccountRequestDto
        {
            Username = $"authuser{timestamp}",
            Email = email,
            Password = "Test@1234",
            ConfirmPassword = "Test@1234",
            CustomerName = "Auth Test User",
            CustomerEmail = email,
            CustomerPhone = "555-0199",
            CustomerAddress = "999 Auth St"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/MyAccount/register", registerRequest);
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<RegisterAccountResponseDto>();

        Assert.NotNull(registerResult);
        Assert.NotNull(registerResult.EmailConfirmationToken);

        // Confirm email
        var confirmRequest = new ConfirmEmailRequestDto
        {
            Email = registerResult.Email,
            ConfirmationToken = registerResult.EmailConfirmationToken
        };
        await _client.PostAsJsonAsync("/api/v1/MyAccount/confirm-email", confirmRequest);

        // Login to get tokens
        var loginRequest = new MyAccountLoginRequestDto
        {
            Email = email,
            Password = "Test@1234"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/MyAccount/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<MyAccountLoginResponseDto>();

        Assert.NotNull(loginResult);

        // Store tokens
        TestTokenStorage.AccessToken = loginResult.AccessToken;
        TestTokenStorage.RefreshToken = loginResult.RefreshToken;
        TestTokenStorage.AccessTokenExpiresAt = loginResult.AccessTokenExpiresAt;
        TestTokenStorage.RefreshTokenExpiresAt = loginResult.RefreshTokenExpiresAt;
        TestTokenStorage.UserId = loginResult.User.Id;
        TestTokenStorage.UserEmail = loginResult.User.Email;

        _output.WriteLine($"Created and authenticated user: {email}");

        return email;
    }

    #endregion
}
