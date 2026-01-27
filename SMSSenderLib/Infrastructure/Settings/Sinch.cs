namespace SMSSenderLib.Infrastructure.Settings
{
    public class Sinch
    {
        public string ServicePlanId { get; set; } = string.Empty;
        public string ApiToken { get; set; } = string.Empty;
        public string FromNumber { get; set; } = string.Empty;
        public string? Region { get; set; }
    }
}
