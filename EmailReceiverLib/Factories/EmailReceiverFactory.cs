using EmailReceiverLib.Infrastructure.Settings;
using EmailReceiverLib.Interfaces;
using EmailReceiverLib.Plugins.Email;
using PluginLib.Plugins;

namespace EmailReceiverLib.Factories;

public class EmailReceiverFactory
{
    private readonly EmailReceiverSettings _settings;
    private readonly IReadOnlyCollection<IEmailReceiverPlugin> _plugins;
    private readonly IPluginSelector _pluginSelector;

    public EmailReceiverFactory(EmailReceiverSettings settings, IEnumerable<IEmailReceiverPlugin>? plugins = null)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        var enabled = new HashSet<string>(
            _settings.Selection.EnabledPluginKeys ?? [],
            StringComparer.OrdinalIgnoreCase);
        var disabled = new HashSet<string>(
            _settings.Selection.DisabledPluginKeys ?? [],
            StringComparer.OrdinalIgnoreCase);

        _plugins = (plugins ?? [])
            .Where(p => (enabled.Count == 0 || enabled.Contains(p.Key))
                        && !disabled.Contains(p.Key))
            .ToArray();

        _pluginSelector = new PluginSelector(new PluginRegistry(_plugins));
    }

    public IEmailReceiver CreateEmailReceiver(string? pluginKey = null, IEnumerable<string>? fallbackPluginKeys = null)
    {
        var preferredPluginKey = FirstNotEmpty(
            pluginKey,
            _settings.Provider,
            _settings.Selection.DefaultPluginKey);

        var fallbackKeys = (fallbackPluginKeys ?? _settings.Selection.FallbackPluginKeys ?? [])
            .Where(k => !string.IsNullOrWhiteSpace(k));

        var selectedPlugin = _pluginSelector.Select(
            PluginType.EmailReceiver,
            preferredPluginKey,
            fallbackKeys);

        if (selectedPlugin is not IEmailReceiverPlugin receiverPlugin)
        {
            throw new InvalidOperationException($"Selected plugin '{selectedPlugin.Key}' is not a valid email receiver plugin.");
        }

        if (!receiverPlugin.CanCreate(_settings))
        {
            foreach (var fallback in fallbackKeys)
            {
                var plugin = _plugins.FirstOrDefault(
                    p => p.Key.Equals(fallback, StringComparison.OrdinalIgnoreCase)
                         && p.CanCreate(_settings)
                         && p.IsEnabled);

                if (plugin is not null)
                {
                    return plugin.Create(_settings);
                }
            }

            throw new InvalidOperationException($"Email receiver plugin '{receiverPlugin.Key}' is selected but not configured.");
        }

        return receiverPlugin.Create(_settings);
    }

    private static string? FirstNotEmpty(params string?[] candidates)
    {
        return candidates.FirstOrDefault(static c => !string.IsNullOrWhiteSpace(c));
    }
}
