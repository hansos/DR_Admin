namespace SMSSenderLib.Infrastructure.Settings
{
    public class Twilio
    {
        public string AccountSid { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;
        public string FromNumber { get; set; } = string.Empty;
        public string? MessagingServiceSid { get; set; }
    }
}
