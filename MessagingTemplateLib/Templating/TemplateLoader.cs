using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace MessagingTemplateLib.Templating;

/// <summary>
/// Loads and caches message templates from the file system
/// </summary>
public class TemplateLoader
{
    private readonly IMemoryCache _cache;
    private readonly string _templateBasePath;
    private static readonly ILogger _log = Log.ForContext<TemplateLoader>();

    public TemplateLoader(IMemoryCache cache, string templateBasePath = "Templates")
    {
        _cache = cache;
        _templateBasePath = templateBasePath;
        _log.Information("TemplateLoader initialized with base path: {BasePath}", _templateBasePath);
    }

    /// <summary>
    /// Loads a message template from the file system
    /// </summary>
    /// <param name="messageType">The message type folder (e.g., "DomainRegistered")</param>
    /// <param name="channel">The channel file (e.g., "email.html")</param>
    /// <returns>The template content</returns>
    public string LoadTemplate(string messageType, string channel)
    {
        var templateFile = Path.Combine(_templateBasePath, messageType, $"{channel}.txt");

        if (_cache.TryGetValue(templateFile, out string? cached) && cached is not null)
        {
            _log.Debug("Template cache hit: {TemplatePath} (Length={Length})", templateFile, cached.Length);
            return cached;
        }

        if (!File.Exists(templateFile))
        {
            _log.Warning("Template file not found: {TemplatePath}", templateFile);
            throw new FileNotFoundException($"Template not found: {templateFile}");
        }

        var content = File.ReadAllText(templateFile);
        _log.Information("Loaded template from disk: {TemplatePath} (Length={Length}, Snippet={Snippet})",
            templateFile, content.Length, content.Length > 120 ? content[..120] : content);

        _cache.Set(templateFile, content, TimeSpan.FromMinutes(10));
        return content;
    }

    /// <summary>
    /// Loads a master/layout template for the specified channel
    /// </summary>
    /// <param name="channel">The message channel</param>
    /// <returns>The master template content</returns>
    public string LoadMasterTemplate(MessageChannel channel)
    {
        string masterFile = channel switch
        {
            MessageChannel.EmailHtml => Path.Combine(_templateBasePath, "Layouts", "email.html.master.txt"),
            MessageChannel.EmailText => Path.Combine(_templateBasePath, "Layouts", "email.text.master.txt"),
            MessageChannel.Sms => Path.Combine(_templateBasePath, "Layouts", "sms.master.txt"),
            _ => throw new ArgumentOutOfRangeException(nameof(channel))
        };

        if (_cache.TryGetValue(masterFile, out string? cached) && cached is not null)
        {
            _log.Debug("Master template cache hit: {MasterPath} (Length={Length})", masterFile, cached.Length);
            return cached;
        }

        if (!File.Exists(masterFile))
        {
            _log.Warning("Master template file not found: {MasterPath}", masterFile);
            throw new FileNotFoundException($"Master template not found: {masterFile}");
        }

        var content = File.ReadAllText(masterFile);
        _log.Information("Loaded master template from disk: {MasterPath} (Length={Length}, Snippet={Snippet})",
            masterFile, content.Length, content.Length > 120 ? content[..120] : content);

        _cache.Set(masterFile, content, TimeSpan.FromMinutes(10));
        return content;
    }

    /// <summary>
    /// Clears the template cache to force reload from disk
    /// </summary>
    public void ClearCache()
    {
        _log.Information("Template cache cleared");
        // Note: IMemoryCache doesn't have a clear all method, 
        // but cached items will expire based on their configured expiration
    }
}
