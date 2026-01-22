namespace EmailSenderLib.Infrastructure.Settings
{
    public class GraphApi
    {
        public string TenantId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string Scope { get; set; } = "https://graph.microsoft.com/.default";
    }
}
