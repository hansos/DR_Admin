namespace HostingPanelLib.Infrastructure.Settings
{
    public class HostingPanelSettings
    {
        public string Provider { get; set; } = string.Empty;
        
        public CpanelSettings? Cpanel { get; set; }
        public PleskSettings? Plesk { get; set; }
        public DirectAdminSettings? DirectAdmin { get; set; }
        public ISPConfigSettings? ISPConfig { get; set; }
        public VirtualminSettings? Virtualmin { get; set; }
        public CyberPanelSettings? CyberPanel { get; set; }
        public CloudPanelSettings? CloudPanel { get; set; }
    }
}
