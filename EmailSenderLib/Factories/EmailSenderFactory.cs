using EmailSenderLib.Infrastructure.Settings;
using EmailSenderLib.Interfaces;
using EmailSenderLib.Plugins.Email;
using PluginLib.Plugins;

namespace EmailSenderLib.Factories
{
    public class EmailSenderFactory
    {
        private readonly EmailSettings _mailSettings;
        private readonly IPluginSelector _pluginSelector;
        private readonly IReadOnlyCollection<IEmailSenderPlugin> _emailPlugins;

        public EmailSenderFactory(EmailSettings emailSettings, IEnumerable<IEmailSenderPlugin>? emailPlugins = null)
        {
            _mailSettings = emailSettings ?? throw new ArgumentNullException(nameof(emailSettings));
            var enabled = new HashSet<string>(
                _mailSettings.Selection.EnabledPluginKeys ?? [],
                StringComparer.OrdinalIgnoreCase);
            var disabled = new HashSet<string>(
                _mailSettings.Selection.DisabledPluginKeys ?? [],
                StringComparer.OrdinalIgnoreCase);

            _emailPlugins = (emailPlugins ?? [])
                .Where(p => (enabled.Count == 0 || enabled.Contains(p.Key))
                            && !disabled.Contains(p.Key))
                .ToArray();

            _pluginSelector = new PluginSelector(new PluginRegistry(_emailPlugins));
        }

        public IEmailSender CreateEmailSender()
        {
            return CreateEmailSender(null);
        }

        public IEmailSender CreateEmailSender(string? pluginKey, IEnumerable<string>? fallbackPluginKeys = null)
        {
            var preferredPluginKey = FirstNotEmpty(
                pluginKey,
                _mailSettings.Provider,
                _mailSettings.Selection.DefaultPluginKey);

            var configuredFallbacks = _mailSettings.Selection.FallbackPluginKeys;
            var fallbackKeys = (fallbackPluginKeys ?? configuredFallbacks ?? [])
                .Where(k => !string.IsNullOrWhiteSpace(k));

            var selectedPlugin = _pluginSelector.Select(
                PluginType.Email,
                preferredPluginKey,
                fallbackKeys);

            if (selectedPlugin is not IEmailSenderPlugin emailPlugin)
            {
                throw new InvalidOperationException($"Selected plugin '{selectedPlugin.Key}' is not a valid email plugin.");
            }

            if (!emailPlugin.CanCreate(_mailSettings))
            {
                foreach (var fallback in fallbackKeys)
                {
                    var plugin = _emailPlugins.FirstOrDefault(
                        p => p.Key.Equals(fallback, StringComparison.OrdinalIgnoreCase)
                             && p.CanCreate(_mailSettings)
                             && p.IsEnabled);

                    if (plugin is not null)
                    {
                        return plugin.Create(_mailSettings);
                    }
                }

                throw new InvalidOperationException($"Email plugin '{emailPlugin.Key}' is selected but not configured.");
            }

            return emailPlugin.Create(_mailSettings);
        }

        private static string? FirstNotEmpty(params string?[] candidates)
        {
            return candidates.FirstOrDefault(static c => !string.IsNullOrWhiteSpace(c));
        }
    }
}
