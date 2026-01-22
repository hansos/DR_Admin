namespace EmailSenderLib.Infrastructure.Settings
{
    public class Postmark
    {
        public string ServerToken { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
    }
}
