# ISPAdmin Integration Tests

This project contains integration tests for the ISPAdmin API endpoints.

## Overview

The integration tests use:
- **xUnit** - Testing framework
- **Microsoft.AspNetCore.Mvc.Testing** - For creating test servers
- **SQLite In-Memory Database** - Each test run uses a fresh in-memory database for complete isolation

## Important: No Pre-existing Credentials Needed!

**You don't need to provide any login credentials or passwords.** The tests automatically:
1. Create a fresh in-memory SQLite database for each test run
2. Register test users dynamically during test execution
3. Use those test users for authentication tests

The test data is defined in:
- `TestData.cs` - Contains constants for test usernames, emails, and passwords
- `appsettings.Test.json` - Optional configuration (not currently used)

## Test Structure

### TestWebApplicationFactory
- Custom web application factory for integration testing
- Configures a SQLite in-memory database that persists only for the duration of the test run
- Uses the same database provider (SQLite) as production but in memory mode
- Skips database migrations (uses `EnsureCreated()` instead)
- Sets environment to "Testing" to skip role synchronization and migrations

### TestTokenStorage
- Static class for sharing authentication tokens across test classes
- Stores access tokens, refresh tokens, and user information
- Allows other test classes to reuse authentication without re-login
- **Note:** Tokens are shared across test runs but may not work if tests run in parallel or use separate database instances

## MyAccountController Tests

The `MyAccountControllerTests` class contains comprehensive tests for all MyAccount endpoints:

### Registration Tests
- ? Register with valid data
- ? Duplicate email validation
- ? Password mismatch validation

### Email Confirmation Tests
- ? Confirm email with valid token
- ? Invalid token handling

### Get My Account Tests
- ? Get authenticated user information
- ? Unauthorized access handling

### Change Password Tests
- ? Change password with valid data
- ? Mismatched passwords validation
- ? Wrong current password handling

**Note:** Login, logout, and refresh token endpoints have been moved to `AuthController` to avoid duplication. See `AuthControllerTests` for these tests.

## AuthController Tests

The `AuthControllerTests` class contains comprehensive tests for all authentication endpoints:

### Login Tests
- ? Login with valid credentials returns JWT tokens
- ? Login with invalid username returns Unauthorized
- ? Login with invalid password returns Unauthorized
- ? Login with empty username returns BadRequest
- ? Login with empty password returns BadRequest
- **? Stores tokens in TestTokenStorage for other test classes**

### Refresh Token Tests
- ? Refresh token with valid token returns new tokens
- ? Refresh token with invalid token returns Unauthorized
- ? Refresh token with empty token returns BadRequest
- ? Refresh token with revoked token returns Unauthorized

### Logout Tests
- ? Logout with valid token revokes refresh token
- ? Logout without authentication returns Unauthorized
- ? Logout with empty refresh token returns BadRequest

### Verify Token Tests
- ? Verify with valid token returns user information
- ? Verify without authentication returns Unauthorized
- ? Verify with invalid token returns Unauthorized
- ? Verify with expired token returns Unauthorized

### Integration Tests
- ? Full authentication flow: Register ? Login ? Verify ? Refresh ? Logout

## BillingCyclesController Tests

The `BillingCyclesControllerTests` class contains comprehensive tests for all BillingCycles endpoints with role-based access control:

### Get All Billing Cycles Tests
- ? Admin role can retrieve all billing cycles
- ? Support role can retrieve all billing cycles
- ? Sales role can retrieve all billing cycles
- ? Customer role receives Forbidden
- ? Unauthenticated access returns Unauthorized

### Get Billing Cycle By Id Tests
- ? Get by valid ID returns OK
- ? Get by invalid ID returns NotFound
- ? Unauthenticated access returns Unauthorized

### Create Billing Cycle Tests
- ? Admin role can create billing cycles
- ? Support role receives Forbidden
- ? Unauthenticated access returns Unauthorized

### Update Billing Cycle Tests
- ? Admin role can update billing cycles
- ? Update with invalid ID returns NotFound
- ? Support role receives Forbidden
- ? Unauthenticated access returns Unauthorized

### Delete Billing Cycle Tests
- ? Admin role can delete billing cycles
- ? Delete with invalid ID returns NotFound
- ? Support role receives Forbidden
- ? Unauthenticated access returns Unauthorized
- ? Deletion is verified

### Integration Tests
- ? Full CRUD flow: Create ? Read ? Update ? Delete

**Note:** These tests demonstrate role-based authorization, dynamically creating users with Admin, Support, Sales, and Customer roles.

## Running the Tests

### Run all tests
```bash
dotnet test
```

### Run tests with detailed output
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run tests by category
```bash
dotnet test --filter "Category=MyAccount"
dotnet test --filter "Category=Auth"
dotnet test --filter "Category=BillingCycles"
```

### Run tests by priority
```bash
dotnet test --filter "Priority=1"
```

## Using TestTokenStorage in Other Test Classes

The `AuthControllerTests` class automatically stores authentication tokens after successful login. Other test classes can use these tokens:

```csharp
public class OtherControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public OtherControllerTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task SomeProtectedEndpoint_WithValidToken_ReturnsData()
    {
        // Check if we have a valid token
        if (TestTokenStorage.HasValidAccessToken())
        {
            // Use the stored token
            _client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", TestTokenStorage.AccessToken);
        }
        else
        {
            // No valid token available, need to login first
            // Run AuthControllerTests.Login_ValidCredentials_ReturnsTokens first
            throw new InvalidOperationException("No valid access token. Run login tests first.");
        }

        var response = await _client.GetAsync("/api/v1/SomeProtectedEndpoint");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

## Test Execution Order

Tests marked with `[Trait("Priority", "N")]` indicate the recommended execution order:
1. Registration tests
2. Email confirmation tests
3. Login tests (stores tokens)
4. Protected endpoint tests
5. Token refresh tests

## Adding New Tests

When adding tests for new controllers:

1. Create a new test class in the `Controllers` folder
2. Inherit from `IClassFixture<TestWebApplicationFactory>`
3. Use `TestTokenStorage` to access shared authentication tokens
4. Follow the existing test patterns for consistency

Example:
```csharp
[Collection("Sequential")]
public class NewControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public NewControllerTests(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    [Fact]
    [Trait("Category", "NewController")]
    public async Task TestMethod_Scenario_ExpectedResult()
    {
        // Arrange
        if (TestTokenStorage.HasValidAccessToken())
        {
            _client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", TestTokenStorage.AccessToken);
        }

        // Act
        var response = await _client.GetAsync("/api/v1/endpoint");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

## Notes

- Tests use a clean in-memory database for each test run
- The `[Collection("Sequential")]` attribute ensures tests run sequentially to avoid database conflicts
- Tokens are stored in a static class and persist across test classes within the same test run
- Always check `TestTokenStorage.HasValidAccessToken()` before using stored tokens
- The test database is automatically seeded with any required initial data through the application's initialization services
