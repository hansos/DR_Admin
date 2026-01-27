namespace SMSSenderLib.Infrastructure.Settings
{
    public class MessageBird
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Originator { get; set; } = string.Empty;
        public string? Reference { get; set; }
    }
}
