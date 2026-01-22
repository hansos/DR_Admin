namespace EmailSenderLib.Infrastructure.Settings
{
    public class Mailgun
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string Region { get; set; } = "US";
    }
}
