namespace DomainRegistrationLib.Infrastructure.Settings
{
    public class RegistrarSettings
    {
        public string Provider { get; set; } = string.Empty;
        
        // Existing providers
        public NamecheapSettings? Namecheap { get; set; }
        public GoDaddySettings? GoDaddy { get; set; }
        public CloudflareSettings? Cloudflare { get; set; }
        
        // New reseller providers
        public OpenSrsSettings? OpenSrs { get; set; }
        public CentralNicSettings? CentralNic { get; set; }
        public DnSimpleSettings? DnSimple { get; set; }
        public DomainboxSettings? Domainbox { get; set; }
        public OxxaSettings? Oxxa { get; set; }
        public RegtonsSettings? Regtons { get; set; }
        public DomainNameApiSettings? DomainNameApi { get; set; }
        public AwsSettings? Aws { get; set; }
    }
}

