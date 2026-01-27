namespace SMSSenderLib.Infrastructure.Settings
{
    public class Gatewayapi
    {
        public string ApiToken { get; set; } = string.Empty;
        public string Sender { get; set; } = string.Empty;
        public string? DestinationAddress { get; set; }
    }
}
