namespace DomainRegistrationLib.Infrastructure.Settings
{
    public class GenericSettings
    {
        public string Name { get; set; } = string.Empty;
        public string ApiUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Dictionary<string, string> AdditionalSettings { get; set; } = new();
    }
}
