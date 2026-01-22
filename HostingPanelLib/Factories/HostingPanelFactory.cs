using HostingPanelLib.Implementations;
using HostingPanelLib.Infrastructure.Settings;
using HostingPanelLib.Interfaces;

namespace HostingPanelLib.Factories
{
    public class HostingPanelFactory
    {
        private readonly HostingPanelSettings _settings;

        public HostingPanelFactory(HostingPanelSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public IHostingPanel CreatePanel()
        {
            return _settings.Provider.ToLower() switch
            {
                "cpanel" => _settings.Cpanel is not null
                    ? new CpanelProvider(
                        _settings.Cpanel.ApiUrl,
                        _settings.Cpanel.ApiToken,
                        _settings.Cpanel.Username,
                        _settings.Cpanel.Port,
                        _settings.Cpanel.UseHttps
                    )
                    : throw new InvalidOperationException("cPanel settings are not configured"),

                "plesk" => _settings.Plesk is not null
                    ? new PleskProvider(
                        _settings.Plesk.ApiUrl,
                        _settings.Plesk.ApiKey,
                        _settings.Plesk.Username,
                        _settings.Plesk.Password,
                        _settings.Plesk.Port,
                        _settings.Plesk.UseHttps
                    )
                    : throw new InvalidOperationException("Plesk settings are not configured"),

                "directadmin" => _settings.DirectAdmin is not null
                    ? new DirectAdminProvider(
                        _settings.DirectAdmin.ApiUrl,
                        _settings.DirectAdmin.Username,
                        _settings.DirectAdmin.Password,
                        _settings.DirectAdmin.Port,
                        _settings.DirectAdmin.UseHttps
                    )
                    : throw new InvalidOperationException("DirectAdmin settings are not configured"),

                "ispconfig" => _settings.ISPConfig is not null
                    ? new ISPConfigProvider(
                        _settings.ISPConfig.ApiUrl,
                        _settings.ISPConfig.Username,
                        _settings.ISPConfig.Password,
                        _settings.ISPConfig.Port,
                        _settings.ISPConfig.UseHttps,
                        _settings.ISPConfig.RemoteApiUrl
                    )
                    : throw new InvalidOperationException("ISPConfig settings are not configured"),

                "virtualmin" => _settings.Virtualmin is not null
                    ? new VirtualminProvider(
                        _settings.Virtualmin.ApiUrl,
                        _settings.Virtualmin.Username,
                        _settings.Virtualmin.Password,
                        _settings.Virtualmin.Port,
                        _settings.Virtualmin.UseHttps
                    )
                    : throw new InvalidOperationException("Virtualmin settings are not configured"),

                "cyberpanel" => _settings.CyberPanel is not null
                    ? new CyberPanelProvider(
                        _settings.CyberPanel.ApiUrl,
                        _settings.CyberPanel.ApiKey,
                        _settings.CyberPanel.AdminUsername,
                        _settings.CyberPanel.AdminPassword,
                        _settings.CyberPanel.Port,
                        _settings.CyberPanel.UseHttps
                    )
                    : throw new InvalidOperationException("CyberPanel settings are not configured"),

                "cloudpanel" => _settings.CloudPanel is not null
                    ? new CloudPanelProvider(
                        _settings.CloudPanel.ApiUrl,
                        _settings.CloudPanel.ApiKey,
                        _settings.CloudPanel.Port,
                        _settings.CloudPanel.UseHttps
                    )
                    : throw new InvalidOperationException("CloudPanel settings are not configured"),

                _ => throw new NotSupportedException($"Hosting panel provider '{_settings.Provider}' is not supported")
            };
        }

        public IHostingPanel CreatePanel(string providerCode)
        {
            var originalProvider = _settings.Provider;
            try
            {
                _settings.Provider = providerCode;
                return CreatePanel();
            }
            finally
            {
                _settings.Provider = originalProvider;
            }
        }
    }
}
