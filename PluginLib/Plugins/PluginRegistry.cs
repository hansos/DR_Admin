namespace PluginLib.Plugins
{
    public sealed class PluginRegistry
    {
        private readonly Dictionary<PluginType, Dictionary<string, IPlugin>> _pluginsByType;

        public PluginRegistry(IEnumerable<IPlugin> plugins)
        {
            ArgumentNullException.ThrowIfNull(plugins);

            _pluginsByType = plugins
                .Where(p => p.IsEnabled)
                .GroupBy(p => p.Type)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(p => p.Key, StringComparer.OrdinalIgnoreCase));
        }

        public IReadOnlyCollection<IPlugin> GetPlugins(PluginType pluginType)
        {
            if (_pluginsByType.TryGetValue(pluginType, out var plugins))
            {
                return plugins.Values.ToArray();
            }

            return Array.Empty<IPlugin>();
        }

        public IPlugin? GetPlugin(PluginType pluginType, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            if (_pluginsByType.TryGetValue(pluginType, out var plugins)
                && plugins.TryGetValue(key, out var plugin))
            {
                return plugin;
            }

            return null;
        }
    }
}
