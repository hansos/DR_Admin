namespace PluginLib.Plugins
{
    public interface IPluginSelector
    {
        IPlugin Select(PluginType pluginType, string? preferredKey = null, IEnumerable<string>? fallbackKeys = null);
    }
}
