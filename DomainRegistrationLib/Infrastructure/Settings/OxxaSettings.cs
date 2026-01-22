namespace DomainRegistrationLib.Infrastructure.Settings
{
    public class OxxaSettings
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool UseLiveEnvironment { get; set; } = false;
        public string LiveApiUrl { get; set; } = "https://api.oxxa.com/command.php";
        public string TestApiUrl { get; set; } = "https://api-ote.oxxa.com/command.php";
    }
}
