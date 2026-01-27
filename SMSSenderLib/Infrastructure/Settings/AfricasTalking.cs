namespace SMSSenderLib.Infrastructure.Settings
{
    public class AfricasTalking
    {
        public string Username { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string? Environment { get; set; } // sandbox or production
    }
}
