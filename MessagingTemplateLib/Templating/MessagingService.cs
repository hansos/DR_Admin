using Serilog;

namespace MessagingTemplateLib.Templating;

/// <summary>
/// Service for rendering messages using templates with master layouts
/// </summary>
public class MessagingService
{
    private readonly TemplateLoader _loader;
    private static readonly ILogger _log = Log.ForContext<MessagingService>();

    public MessagingService(TemplateLoader loader)
    {
        _loader = loader;
    }

    /// <summary>
    /// Renders a message using templates for the specified channel
    /// </summary>
    /// <typeparam name="TModel">The type of the data model</typeparam>
    /// <param name="messageType">The message type (folder name)</param>
    /// <param name="channel">The message channel</param>
    /// <param name="model">The data model for the message</param>
    /// <returns>The fully rendered message</returns>
    public string RenderMessage<TModel>(string messageType, MessageChannel channel, TModel model) where TModel : class
    {
        try
        {
            _log.Information("Rendering message - Type: {MessageType}, Channel: {Channel}, ModelType: {ModelType}", 
                messageType, channel, model.GetType().Name);

            var channelFileName = channel switch
            {
                MessageChannel.EmailHtml => "email.html",
                MessageChannel.EmailText => "email.text",
                MessageChannel.Sms => "sms",
                _ => throw new ArgumentOutOfRangeException(nameof(channel))
            };

            var contentTemplate = _loader.LoadTemplate(messageType, channelFileName);
            var masterTemplate = _loader.LoadMasterTemplate(channel);

            _log.Information("Templates loaded - Content length: {ContentLen}, Master length: {MasterLen}, ContentIsNull: {ContentNull}, MasterIsNull: {MasterNull}",
                contentTemplate?.Length ?? -1, masterTemplate?.Length ?? -1,
                contentTemplate is null, masterTemplate is null);

            var values = TemplateRenderer.ModelToDictionary(model);

            _log.Information("Model values ({Count}): [{Values}]",
                values.Count,
                string.Join(", ", values.Select(kv => $"{kv.Key}='{kv.Value}'")));

            var result = TemplateRenderer.RenderWithMaster(contentTemplate, masterTemplate, values);

            _log.Information("Message rendered - Type: {MessageType}, ResultLength: {Length}, HasUnreplacedPlaceholders: {HasUnreplaced}, ResultSnippet: {Snippet}", 
                messageType, result.Length,
                result.Contains("{{"),
                result.Length > 300 ? result[..300] : result);

            return result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error rendering message - Type: {MessageType}, Channel: {Channel}", 
                messageType, channel);
            throw;
        }
    }

    /// <summary>
    /// Renders a message without using a master template
    /// </summary>
    /// <typeparam name="TModel">The type of the data model</typeparam>
    /// <param name="messageType">The message type (folder name)</param>
    /// <param name="channel">The message channel</param>
    /// <param name="model">The data model for the message</param>
    /// <returns>The rendered message content only</returns>
    public string RenderMessageWithoutMaster<TModel>(string messageType, MessageChannel channel, TModel model) where TModel : class
    {
        try
        {
            _log.Information("Rendering message without master - Type: {MessageType}, Channel: {Channel}", 
                messageType, channel);

            var channelFileName = channel switch
            {
                MessageChannel.EmailHtml => "email.html",
                MessageChannel.EmailText => "email.text",
                MessageChannel.Sms => "sms",
                _ => throw new ArgumentOutOfRangeException(nameof(channel))
            };

            var contentTemplate = _loader.LoadTemplate(messageType, channelFileName);
            var values = TemplateRenderer.ModelToDictionary(model);

            return TemplateRenderer.Render(contentTemplate, values);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error rendering message - Type: {MessageType}, Channel: {Channel}", 
                messageType, channel);
            throw;
        }
    }
}
