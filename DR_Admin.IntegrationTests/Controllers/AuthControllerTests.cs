using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ISPAdmin.DTOs;
using Xunit;
using Xunit.Abstractions;

namespace DR_Admin.IntegrationTests.Controllers;

[Collection("Sequential")]
public class AuthControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private readonly TestWebApplicationFactory _factory;

    public AuthControllerTests(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _output = output;
    }

    #region Login Tests

    [Fact]
    [Trait("Category", "Auth")]
    [Trait("Priority", "1")]
    public async Task Login_ValidCredentials_ReturnsTokens()
    {
        // Arrange - First register a user
        var registerRequest = new RegisterAccountRequestDto
        {
            Username = "authuser",
            Email = "authuser@example.com",
            Password = "Auth@1234",
            ConfirmPassword = "Auth@1234",
            CustomerName = "Auth Test Customer",
            CustomerEmail = "authcustomer@example.com",
            CustomerPhone = "555-0200",
            CustomerAddress = "456 Auth St"
        };

        await _client.PostAsJsonAsync("/api/v1/MyAccount/register", registerRequest);

        var loginRequest = new LoginRequestDto
        {
            Username = "authuser",
            Password = "Auth@1234"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.Equal("authuser", result.Username);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
        Assert.NotNull(result.Roles);

        _output.WriteLine($"Access Token: {result.AccessToken[..20]}...");
        _output.WriteLine($"Refresh Token: {result.RefreshToken[..20]}...");
        _output.WriteLine($"Username: {result.Username}");
        _output.WriteLine($"Expires At: {result.ExpiresAt}");
        _output.WriteLine($"Roles: {string.Join(", ", result.Roles)}");

        // Store for other tests
        TestTokenStorage.AccessToken = result.AccessToken;
        TestTokenStorage.RefreshToken = result.RefreshToken;
        TestTokenStorage.AccessTokenExpiresAt = result.ExpiresAt;
        TestTokenStorage.UserEmail = "authuser@example.com";
    }

    [Fact]
    [Trait("Category", "Auth")]
    public async Task Login_InvalidUsername_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "nonexistentuser",
            Password = "SomePassword123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid username or password", content);

        _output.WriteLine($"Response: {content}");
    }

    [Fact]
    [Trait("Category", "Auth")]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange - First register a user
        var registerRequest = new RegisterAccountRequestDto
        {
            Username = "passwordtest",
            Email = "passwordtest@example.com",
            Password = "Correct@1234",
            ConfirmPassword = "Correct@1234",
            CustomerName = "Password Test Customer",
            CustomerEmail = "passwordcustomer@example.com",
            CustomerPhone = "555-0201",
            CustomerAddress = "789 Password St"
        };

        await _client.PostAsJsonAsync("/api/v1/MyAccount/register", registerRequest);

        var loginRequest = new LoginRequestDto
        {
            Username = "passwordtest",
            Password = "WrongPassword123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid username or password", content);

        _output.WriteLine($"Response: {content}");
    }

    [Fact]
    [Trait("Category", "Auth")]
    public async Task Login_EmptyUsername_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "",
            Password = "SomePassword123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Username and password are required", content);

        _output.WriteLine($"Response: {content}");
    }

    [Fact]
    [Trait("Category", "Auth")]
    public async Task Login_EmptyPassword_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "someuser",
            Password = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Username and password are required", content);

        _output.WriteLine($"Response: {content}");
    }

    #endregion

    #region Refresh Token Tests

    [Fact]
    [Trait("Category", "Auth")]
    public async Task RefreshToken_ValidToken_ReturnsNewTokens()
    {
        // Arrange - First register and login to get tokens
        var registerRequest = new RegisterAccountRequestDto
        {
            Username = "refreshuser",
            Email = "refreshuser@example.com",
            Password = "Refresh@1234",
            ConfirmPassword = "Refresh@1234",
            CustomerName = "Refresh Test Customer",
            CustomerEmail = "refreshcustomer@example.com",
            CustomerPhone = "555-0202",
            CustomerAddress = "321 Refresh St"
        };

        await _client.PostAsJsonAsync("/api/v1/MyAccount/register", registerRequest);

        var loginRequest = new LoginRequestDto
        {
            Username = "refreshuser",
            Password = "Refresh@1234"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        // Small delay to ensure tokens are different
        await Task.Delay(1000);

        var refreshRequest = new RefreshTokenRequestDto
        {
            RefreshToken = loginResult!.RefreshToken
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/refresh", refreshRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.Equal("refreshuser", result.Username);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);

        // New tokens should be different from old tokens
        Assert.NotEqual(loginResult.AccessToken, result.AccessToken);

        _output.WriteLine($"Old Access Token: {loginResult.AccessToken[..20]}...");
        _output.WriteLine($"New Access Token: {result.AccessToken[..20]}...");
        _output.WriteLine($"New Refresh Token: {result.RefreshToken[..20]}...");
    }

    [Fact]
    [Trait("Category", "Auth")]
    public async Task RefreshToken_InvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var refreshRequest = new RefreshTokenRequestDto
        {
            RefreshToken = "invalid-refresh-token-12345"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/refresh", refreshRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid or expired refresh token", content);

        _output.WriteLine($"Response: {content}");
    }

    [Fact]
    [Trait("Category", "Auth")]
    public async Task RefreshToken_EmptyToken_ReturnsBadRequest()
    {
        // Arrange
        var refreshRequest = new RefreshTokenRequestDto
        {
            RefreshToken = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/refresh", refreshRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Refresh token is required", content);

        _output.WriteLine($"Response: {content}");
    }

    [Fact]
    [Trait("Category", "Auth")]
    public async Task RefreshToken_RevokedToken_ReturnsUnauthorized()
    {
        // Arrange - Register, login, then logout (revoke token)
        var registerRequest = new RegisterAccountRequestDto
        {
            Username = "revokeuser",
            Email = "revokeuser@example.com",
            Password = "Revoke@1234",
            ConfirmPassword = "Revoke@1234",
            CustomerName = "Revoke Test Customer",
            CustomerEmail = "revokecustomer@example.com",
            CustomerPhone = "555-0203",
            CustomerAddress = "654 Revoke St"
        };

        await _client.PostAsJsonAsync("/api/v1/MyAccount/register", registerRequest);

        var loginRequest = new LoginRequestDto
        {
            Username = "revokeuser",
            Password = "Revoke@1234"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        // Logout to revoke the refresh token
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.AccessToken);
        var logoutRequest = new RefreshTokenRequestDto
        {
            RefreshToken = loginResult.RefreshToken
        };
        await _client.PostAsJsonAsync("/api/v1/Auth/logout", logoutRequest);

        // Reset authorization header
        _client.DefaultRequestHeaders.Authorization = null;

        var refreshRequest = new RefreshTokenRequestDto
        {
            RefreshToken = loginResult.RefreshToken
        };

        // Act - Try to refresh with revoked token
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/refresh", refreshRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid or expired refresh token", content);

        _output.WriteLine($"Response: {content}");
    }

    #endregion

    #region Logout Tests

    [Fact]
    [Trait("Category", "Auth")]
    public async Task Logout_ValidToken_ReturnsOk()
    {
        // Arrange - Register and login
        var registerRequest = new RegisterAccountRequestDto
        {
            Username = "logoutuser",
            Email = "logoutuser@example.com",
            Password = "Logout@1234",
            ConfirmPassword = "Logout@1234",
            CustomerName = "Logout Test Customer",
            CustomerEmail = "logoutcustomer@example.com",
            CustomerPhone = "555-0204",
            CustomerAddress = "987 Logout St"
        };

        await _client.PostAsJsonAsync("/api/v1/MyAccount/register", registerRequest);

        var loginRequest = new LoginRequestDto
        {
            Username = "logoutuser",
            Password = "Logout@1234"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        // Set authorization header
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.AccessToken);

        var logoutRequest = new RefreshTokenRequestDto
        {
            RefreshToken = loginResult.RefreshToken
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/logout", logoutRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Logged out successfully", content);

        _output.WriteLine($"Response: {content}");

        // Reset authorization header
        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    [Trait("Category", "Auth")]
    public async Task Logout_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange - No authentication header
        var logoutRequest = new RefreshTokenRequestDto
        {
            RefreshToken = "some-refresh-token"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/logout", logoutRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        _output.WriteLine($"Status: {response.StatusCode}");
    }

    [Fact]
    [Trait("Category", "Auth")]
    public async Task Logout_EmptyRefreshToken_ReturnsBadRequest()
    {
        // Arrange - Register and login first
        var registerRequest = new RegisterAccountRequestDto
        {
            Username = "emptylogoutuser",
            Email = "emptylogoutuser@example.com",
            Password = "EmptyLogout@1234",
            ConfirmPassword = "EmptyLogout@1234",
            CustomerName = "Empty Logout Test Customer",
            CustomerEmail = "emptylogoutcustomer@example.com",
            CustomerPhone = "555-0205",
            CustomerAddress = "111 Empty Logout St"
        };

        await _client.PostAsJsonAsync("/api/v1/MyAccount/register", registerRequest);

        var loginRequest = new LoginRequestDto
        {
            Username = "emptylogoutuser",
            Password = "EmptyLogout@1234"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        // Set authorization header
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.AccessToken);

        var logoutRequest = new RefreshTokenRequestDto
        {
            RefreshToken = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/Auth/logout", logoutRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Refresh token is required", content);

        _output.WriteLine($"Response: {content}");

        // Reset authorization header
        _client.DefaultRequestHeaders.Authorization = null;
    }

    #endregion

    #region Verify Token Tests

    [Fact]
    [Trait("Category", "Auth")]
    public async Task Verify_ValidToken_ReturnsUserInfo()
    {
        // Arrange - Register and login
        var registerRequest = new RegisterAccountRequestDto
        {
            Username = "verifyuser",
            Email = "verifyuser@example.com",
            Password = "Verify@1234",
            ConfirmPassword = "Verify@1234",
            CustomerName = "Verify Test Customer",
            CustomerEmail = "verifycustomer@example.com",
            CustomerPhone = "555-0206",
            CustomerAddress = "222 Verify St"
        };

        await _client.PostAsJsonAsync("/api/v1/MyAccount/register", registerRequest);

        var loginRequest = new LoginRequestDto
        {
            Username = "verifyuser",
            Password = "Verify@1234"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        // Set authorization header
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.AccessToken);

        // Act
        var response = await _client.GetAsync("/api/v1/Auth/verify");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("verifyuser", content);
        Assert.Contains("isAuthenticated", content);

        _output.WriteLine($"Response: {content}");

        // Reset authorization header
        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    [Trait("Category", "Auth")]
    public async Task Verify_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange - No authentication header

        // Act
        var response = await _client.GetAsync("/api/v1/Auth/verify");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        _output.WriteLine($"Status: {response.StatusCode}");
    }

    [Fact]
    [Trait("Category", "Auth")]
    public async Task Verify_InvalidToken_ReturnsUnauthorized()
    {
        // Arrange - Set invalid token
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid.jwt.token");

        // Act
        var response = await _client.GetAsync("/api/v1/Auth/verify");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        _output.WriteLine($"Status: {response.StatusCode}");

        // Reset authorization header
        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    [Trait("Category", "Auth")]
    public async Task Verify_ExpiredToken_ReturnsUnauthorized()
    {
        // Arrange - Use a known expired token format (this will be treated as invalid)
        var expiredToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJleHAiOjE1MTYyMzkwMjJ9.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", expiredToken);

        // Act
        var response = await _client.GetAsync("/api/v1/Auth/verify");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        _output.WriteLine($"Status: {response.StatusCode}");

        // Reset authorization header
        _client.DefaultRequestHeaders.Authorization = null;
    }

    #endregion

    #region Integration Tests

    [Fact]
    [Trait("Category", "Auth")]
    [Trait("Priority", "2")]
    public async Task FullAuthFlow_RegisterLoginRefreshVerifyLogout_Success()
    {
        // 1. Register
        var registerRequest = new RegisterAccountRequestDto
        {
            Username = "fullflowuser",
            Email = "fullflowuser@example.com",
            Password = "FullFlow@1234",
            ConfirmPassword = "FullFlow@1234",
            CustomerName = "Full Flow Test Customer",
            CustomerEmail = "fullflowcustomer@example.com",
            CustomerPhone = "555-0207",
            CustomerAddress = "333 FullFlow St"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/MyAccount/register", registerRequest);
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
        _output.WriteLine("? Registration successful");

        // 2. Login
        var loginRequest = new LoginRequestDto
        {
            Username = "fullflowuser",
            Password = "FullFlow@1234"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/Auth/login", loginRequest);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        Assert.NotNull(loginResult);
        _output.WriteLine("? Login successful");

        // 3. Verify token
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);
        var verifyResponse = await _client.GetAsync("/api/v1/Auth/verify");
        Assert.Equal(HttpStatusCode.OK, verifyResponse.StatusCode);
        _output.WriteLine("? Token verification successful");

        // 4. Refresh token
        await Task.Delay(1000); // Ensure new tokens
        var refreshRequest = new RefreshTokenRequestDto
        {
            RefreshToken = loginResult.RefreshToken
        };

        var refreshResponse = await _client.PostAsJsonAsync("/api/v1/Auth/refresh", refreshRequest);
        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);
        var refreshResult = await refreshResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        Assert.NotNull(refreshResult);
        _output.WriteLine("? Token refresh successful");

        // 5. Logout
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", refreshResult.AccessToken);
        var logoutRequest = new RefreshTokenRequestDto
        {
            RefreshToken = refreshResult.RefreshToken
        };

        var logoutResponse = await _client.PostAsJsonAsync("/api/v1/Auth/logout", logoutRequest);
        Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);
        _output.WriteLine("? Logout successful");

        // 6. Verify token is revoked - try to refresh again
        _client.DefaultRequestHeaders.Authorization = null;
        var refreshAfterLogout = await _client.PostAsJsonAsync("/api/v1/Auth/refresh", new RefreshTokenRequestDto
        {
            RefreshToken = refreshResult.RefreshToken
        });
        Assert.Equal(HttpStatusCode.Unauthorized, refreshAfterLogout.StatusCode);
        _output.WriteLine("? Token properly revoked after logout");

        _output.WriteLine("\n? Full authentication flow completed successfully!");
    }

    #endregion
}
