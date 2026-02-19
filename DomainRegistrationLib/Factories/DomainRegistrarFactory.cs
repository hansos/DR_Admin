using DomainRegistrationLib.Implementations;
using DomainRegistrationLib.Infrastructure.Settings;
using DomainRegistrationLib.Interfaces;
using Serilog;

namespace DomainRegistrationLib.Factories
{
    public class DomainRegistrarFactory
    {
        private readonly RegistrarSettings _registrarSettings;
        private readonly SandboxSettings? _sandboxSettings;
        private static readonly ILogger _logger = Log.ForContext<DomainRegistrarFactory>();

        public DomainRegistrarFactory(RegistrarSettings registrarSettings, SandboxSettings? sandboxSettings = null)
        {
            _registrarSettings = registrarSettings ?? throw new ArgumentNullException(nameof(registrarSettings));
            _sandboxSettings = sandboxSettings;
        }

        /// <summary>
        /// Returns true when sandbox mode is enabled and the DomainRegistrationLib filter is active
        /// </summary>
        private bool IsSandboxActive => _sandboxSettings is { Enabled: true, Filters.DomainRegistrationLib: true };

        public IDomainRegistrar CreateRegistrar()
        {
            if (IsSandboxActive)
            {
                _logger.Information("[SANDBOX] Sandbox mode is active — returning SandboxRegistrar instead of provider '{Provider}'", _registrarSettings.Provider);
                return new SandboxRegistrar();
            }

            return _registrarSettings.Provider.ToLower() switch
            {
                "namecheap" => _registrarSettings.Namecheap is not null
                    ? new NamecheapRegistrar(
                        _registrarSettings.Namecheap.ApiUser,
                        _registrarSettings.Namecheap.ApiKey,
                        _registrarSettings.Namecheap.Username,
                        _registrarSettings.Namecheap.ClientIp,
                        _registrarSettings.Namecheap.UseSandbox
                    )
                    : throw new InvalidOperationException("Namecheap settings are not configured"),

                "godaddy" => _registrarSettings.GoDaddy is not null
                    ? new GoDaddyRegistrar(
                        _registrarSettings.GoDaddy.ApiKey,
                        _registrarSettings.GoDaddy.ApiSecret,
                        _registrarSettings.GoDaddy.UseProduction
                    )
                    : throw new InvalidOperationException("GoDaddy settings are not configured"),

                "cloudflare" => _registrarSettings.Cloudflare is not null
                    ? new CloudflareRegistrar(
                        _registrarSettings.Cloudflare.ApiToken,
                        _registrarSettings.Cloudflare.AccountId
                    )
                    : throw new InvalidOperationException("Cloudflare settings are not configured"),

                "opensrs" => _registrarSettings.OpenSrs is not null
                    ? new OpenSrsRegistrar(
                        _registrarSettings.OpenSrs.Username,
                        _registrarSettings.OpenSrs.ApiKey,
                        _registrarSettings.OpenSrs.Domain,
                        _registrarSettings.OpenSrs.UseLiveEnvironment
                    )
                    : throw new InvalidOperationException("OpenSRS settings are not configured"),

                "centralnic" => _registrarSettings.CentralNic is not null
                    ? new CentralNicRegistrar(
                        _registrarSettings.CentralNic.Username,
                        _registrarSettings.CentralNic.Password,
                        _registrarSettings.CentralNic.UseLiveEnvironment
                    )
                    : throw new InvalidOperationException("CentralNic settings are not configured"),

                "dnsimple" => _registrarSettings.DnSimple is not null
                    ? new DnSimpleRegistrar(
                        _registrarSettings.DnSimple.AccountId,
                        _registrarSettings.DnSimple.ApiToken,
                        _registrarSettings.DnSimple.UseLiveEnvironment
                    )
                    : throw new InvalidOperationException("DNSimple settings are not configured"),

                "domainbox" => _registrarSettings.Domainbox is not null
                    ? new DomainboxRegistrar(
                        _registrarSettings.Domainbox.ApiKey,
                        _registrarSettings.Domainbox.ApiSecret,
                        _registrarSettings.Domainbox.UseLiveEnvironment
                    )
                    : throw new InvalidOperationException("DomainBox settings are not configured"),

                "oxxa" => _registrarSettings.Oxxa is not null
                    ? new OxxaRegistrar(
                        _registrarSettings.Oxxa.Username,
                        _registrarSettings.Oxxa.Password,
                        _registrarSettings.Oxxa.UseLiveEnvironment
                    )
                    : throw new InvalidOperationException("OXXA settings are not configured"),

                "regtons" => _registrarSettings.Regtons is not null
                    ? new RegtonsRegistrar(
                        _registrarSettings.Regtons.ApiKey,
                        _registrarSettings.Regtons.ApiSecret,
                        _registrarSettings.Regtons.Username,
                        _registrarSettings.Regtons.UseLiveEnvironment
                    )
                    : throw new InvalidOperationException("Regtons settings are not configured"),

                "domainnameapi" => _registrarSettings.DomainNameApi is not null
                    ? new DomainNameApiRegistrar(
                        _registrarSettings.DomainNameApi.Username,
                        _registrarSettings.DomainNameApi.Password,
                        _registrarSettings.DomainNameApi.UseLiveEnvironment
                    )
                    : throw new InvalidOperationException("Domain Name API settings are not configured"),

                "aws" => _registrarSettings.Aws is not null
                    ? new AwsRegistrar(
                        _registrarSettings.Aws.AccessKeyId,
                        _registrarSettings.Aws.SecretAccessKey,
                        _registrarSettings.Aws.Region
                    )
                    : throw new InvalidOperationException("AWS settings are not configured"),

                _ => throw new NotSupportedException($"Registrar provider '{_registrarSettings.Provider}' is not supported")
            };
        }

        public IDomainRegistrar CreateRegistrar(string providerCode)
        {
            if (IsSandboxActive)
            {
                _logger.Information("[SANDBOX] Sandbox mode is active — ignoring provider code '{ProviderCode}', returning SandboxRegistrar", providerCode);
                return new SandboxRegistrar();
            }

            var originalProvider = _registrarSettings.Provider;
            try
            {
                _registrarSettings.Provider = providerCode;
                return CreateRegistrar();
            }
            finally
            {
                _registrarSettings.Provider = originalProvider;
            }
        }
    }
}
