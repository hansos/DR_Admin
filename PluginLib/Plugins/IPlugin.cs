namespace PluginLib.Plugins
{
    public interface IPlugin
    {
        PluginType Type { get; }
        string Key { get; }
        string DisplayName { get; }
        string Version { get; }
        IReadOnlyCollection<string> Capabilities { get; }
        bool IsEnabled { get; }
    }
}
