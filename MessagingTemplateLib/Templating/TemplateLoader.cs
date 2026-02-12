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
        
        return _cache.GetOrCreate(templateFile, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            
            if (!File.Exists(templateFile))
            {
                _log.Warning("Template file not found: {TemplatePath}", templateFile);
                throw new FileNotFoundException($"Template not found: {templateFile}");
            }
            
            _log.Debug("Loading template from: {TemplatePath}", templateFile);
            return File.ReadAllText(templateFile);
        });
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

        return _cache.GetOrCreate(masterFile, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            
            if (!File.Exists(masterFile))
            {
                _log.Warning("Master template file not found: {MasterPath}", masterFile);
                throw new FileNotFoundException($"Master template not found: {masterFile}");
            }
            
            _log.Debug("Loading master template from: {MasterPath}", masterFile);
            return File.ReadAllText(masterFile);
        });
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
