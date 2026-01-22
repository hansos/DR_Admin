using DomainRegistrationLib.Implementations;
using DomainRegistrationLib.Infrastructure.Settings;
using DomainRegistrationLib.Interfaces;

namespace DomainRegistrationLib.Factories
{
    public class DomainRegistrarFactory
    {
        private readonly RegistrarSettings _registrarSettings;

        public DomainRegistrarFactory(RegistrarSettings registrarSettings)
        {
            _registrarSettings = registrarSettings ?? throw new ArgumentNullException(nameof(registrarSettings));
        }

        public IDomainRegistrar CreateRegistrar()
        {
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

                "generic" => _registrarSettings.Generic is not null
                    ? new GenericRegistrar(
                        _registrarSettings.Generic.ApiUrl,
                        _registrarSettings.Generic.ApiKey,
                        _registrarSettings.Generic.ApiSecret,
                        _registrarSettings.Generic.Username,
                        _registrarSettings.Generic.Password
                    )
                    : throw new InvalidOperationException("Generic registrar settings are not configured"),

                _ => throw new NotSupportedException($"Registrar provider '{_registrarSettings.Provider}' is not supported")
            };
        }

        public IDomainRegistrar CreateRegistrar(string providerCode)
        {
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
