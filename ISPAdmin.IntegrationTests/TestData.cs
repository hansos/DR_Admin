namespace ISPAdmin.IntegrationTests;

/// <summary>
/// Constants for test data used across integration tests
/// </summary>
public static class TestData
{
    public static class Users
    {
        public const string DefaultUsername = "testuser";
        public const string DefaultEmail = "testuser@example.com";
        public const string DefaultPassword = "Test@1234";
        
        public const string CustomerName = "Test Customer";
        public const string CustomerEmail = "customer@example.com";
        public const string CustomerPhone = "555-0100";
        public const string CustomerAddress = "123 Test St";
    }
    
    public static class Passwords
    {
        public const string Valid = "Test@1234";
        public const string Invalid = "WrongPassword";
        public const string NewPassword = "NewTest@5678";
    }
}
