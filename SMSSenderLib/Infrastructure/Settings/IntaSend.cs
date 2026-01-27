namespace SMSSenderLib.Infrastructure.Settings
{
    public class IntaSend
    {
        public string ApiKey { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public bool TestMode { get; set; } = false;
    }
}
