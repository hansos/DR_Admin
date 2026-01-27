namespace SMSSenderLib.Infrastructure.Settings
{
    public class Safaricom
    {
        public string ConsumerKey { get; set; } = string.Empty;
        public string ConsumerSecret { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public string InitiatorName { get; set; } = string.Empty;
        public string SecurityCredential { get; set; } = string.Empty;
        public string Environment { get; set; } = "sandbox"; // sandbox or production
        public string? CallbackUrl { get; set; }
    }
}
