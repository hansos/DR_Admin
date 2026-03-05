namespace PluginLib.Plugins
{
    public sealed class PluginSelector(PluginRegistry registry) : IPluginSelector
    {
        private readonly PluginRegistry _registry = registry ?? throw new ArgumentNullException(nameof(registry));

        public IPlugin Select(PluginType pluginType, string? preferredKey = null, IEnumerable<string>? fallbackKeys = null)
        {
            var keysToTry = new List<string>();

            if (!string.IsNullOrWhiteSpace(preferredKey))
            {
                keysToTry.Add(preferredKey);
            }

            if (fallbackKeys is not null)
            {
                keysToTry.AddRange(fallbackKeys.Where(k => !string.IsNullOrWhiteSpace(k)));
            }

            foreach (var key in keysToTry.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var plugin = _registry.GetPlugin(pluginType, key);
                if (plugin is not null)
                {
                    return plugin;
                }
            }

            var available = _registry.GetPlugins(pluginType).ToArray();
            if (available.Length > 0)
            {
                return available[0];
            }

            throw new InvalidOperationException($"No enabled plugins registered for plugin type '{pluginType}'.");
        }
    }
}
