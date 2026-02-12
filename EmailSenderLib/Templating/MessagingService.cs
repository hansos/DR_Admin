using EmailSenderLib.Enums;
using Serilog;

namespace EmailSenderLib.Templating;

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
            _log.Information("Rendering message - Type: {MessageType}, Channel: {Channel}", 
                messageType, channel);

            var channelFileName = channel switch
            {
                MessageChannel.EmailHtml => "email.html",
                MessageChannel.EmailText => "email.text",
                MessageChannel.Sms => "sms",
                _ => throw new ArgumentOutOfRangeException(nameof(channel))
            };

            var contentTemplate = _loader.LoadTemplate(messageType, channelFileName);
            var masterTemplate = _loader.LoadMasterTemplate(channel);

            var values = TemplateRenderer.ModelToDictionary(model);

            var result = TemplateRenderer.RenderWithMaster(contentTemplate, masterTemplate, values);
            
            _log.Debug("Message rendered successfully - Type: {MessageType}, Length: {Length}", 
                messageType, result.Length);
            
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
