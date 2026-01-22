namespace EmailSenderLib.Infrastructure.Settings
{
    public class Exchange
    {
        public string ServerUrl { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string Version { get; set; } = "Exchange2013_SP1";
    }
}
