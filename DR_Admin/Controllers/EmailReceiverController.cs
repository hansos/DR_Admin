using EmailReceiverLib.Factories;
using EmailReceiverLib.Infrastructure.Settings;
using EmailReceiverLib.Interfaces;
using EmailReceiverLib.Models;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;

namespace ISPAdmin.Controllers;

/// <summary>
/// Provides endpoints for reading inbound emails through configured receiver plugins.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class EmailReceiverController : ControllerBase
{
    private static readonly HttpClient HttpClient = new();
    private readonly EmailReceiverFactory _emailReceiverFactory;
    private readonly EmailReceiverSettings _emailReceiverSettings;
    private readonly ICommunicationIngestionService _communicationIngestionService;
    private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<EmailReceiverController>();

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailReceiverController"/> class.
    /// </summary>
    /// <param name="emailReceiverFactory">Factory used to resolve the configured email receiver plugin.</param>
    /// <param name="emailReceiverSettings">Email receiver settings from configuration.</param>
    /// <param name="communicationIngestionService">Service used to persist inbound mailbox messages into communication threads.</param>
    public EmailReceiverController(
        EmailReceiverFactory emailReceiverFactory,
        EmailReceiverSettings emailReceiverSettings,
        ICommunicationIngestionService communicationIngestionService)
    {
        _emailReceiverFactory = emailReceiverFactory;
        _emailReceiverSettings = emailReceiverSettings;
        _communicationIngestionService = communicationIngestionService;
    }

    /// <summary>
    /// Tests token acquisition against the Office365 OAuth token endpoint using configured client credentials.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for aborting the token request.</param>
    /// <returns>Token endpoint status and token metadata (without returning the full access token).</returns>
    /// <response code="200">Token was acquired successfully.</response>
    /// <response code="400">Returned when required Office365 settings are missing.</response>
    /// <response code="401">Returned when user is not authenticated.</response>
    /// <response code="403">Returned when user has insufficient permissions.</response>
    /// <response code="500">Returned when an unexpected server error occurs.</response>
    [HttpPost("token/test")]
    [Authorize(Policy = "EmailReceiver.Read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> TestOffice365Token(CancellationToken cancellationToken = default)
    {
        try
        {
            var office365 = _emailReceiverSettings.Office365;
            if (office365 is null
                || string.IsNullOrWhiteSpace(office365.TenantId)
                || string.IsNullOrWhiteSpace(office365.ClientId)
                || string.IsNullOrWhiteSpace(office365.ClientSecret))
            {
                return BadRequest("Office365 receiver settings (TenantId, ClientId, ClientSecret) must be configured.");
            }

            var endpoint = $"https://login.microsoftonline.com/{Uri.EscapeDataString(office365.TenantId)}/oauth2/v2.0/token";

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = office365.ClientId,
                    ["client_secret"] = office365.ClientSecret,
                    ["scope"] = "https://graph.microsoft.com/.default",
                    ["grant_type"] = "client_credentials"
                })
            };

            using var response = await HttpClient.SendAsync(request, cancellationToken);
            var payload = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("Office365 token test failed. Status: {StatusCode}. Response: {Response}", (int)response.StatusCode, payload);
                return StatusCode((int)response.StatusCode, new
                {
                    success = false,
                    endpoint,
                    statusCode = (int)response.StatusCode,
                    response = payload
                });
            }

            using var json = JsonDocument.Parse(payload);
            var root = json.RootElement;

            var tokenType = root.TryGetProperty("token_type", out var tokenTypeProp) ? tokenTypeProp.GetString() : null;
            var expiresIn = root.TryGetProperty("expires_in", out var expiresInProp) && expiresInProp.TryGetInt32(out var exp) ? exp : 0;
            var accessTokenPresent = root.TryGetProperty("access_token", out var tokenProp) && !string.IsNullOrWhiteSpace(tokenProp.GetString());

            return Ok(new
            {
                success = true,
                endpoint,
                tokenType,
                expiresIn,
                accessTokenPresent
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error while testing Office365 token endpoint.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while testing Office365 token endpoint.");
        }
    }

    /// <summary>
    /// Reads messages from the configured Office365 mailbox.
    /// </summary>
    /// <param name="folder">Optional folder name, such as Inbox.</param>
    /// <param name="unreadOnly">When true, only unread messages are returned.</param>
    /// <param name="maxItems">Maximum number of messages to return.</param>
    /// <param name="aliasRecipient">Optional recipient alias email address to filter messages by.</param>
    /// <param name="receivedAfterUtc">Optional UTC timestamp to only return newer messages.</param>
    /// <param name="cancellationToken">Cancellation token for aborting the read operation.</param>
    /// <returns>A result containing received messages and optional pagination cursor.</returns>
    /// <response code="200">Returns the mailbox read result.</response>
    /// <response code="400">Returned when plugin configuration is invalid.</response>
    /// <response code="401">Returned when user is not authenticated.</response>
    /// <response code="403">Returned when user has insufficient permissions.</response>
    /// <response code="500">Returned when an unexpected server error occurs.</response>
    [HttpGet("messages")]
    [Authorize(Policy = "EmailReceiver.Read")]
    [ProducesResponseType(typeof(MailReadResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MailReadResult>> ReadMessages(
        [FromQuery] string? folder = null,
        [FromQuery] bool unreadOnly = false,
        [FromQuery] int maxItems = 25,
        [FromQuery] string? aliasRecipient = null,
        [FromQuery] DateTime? receivedAfterUtc = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var receiver = _emailReceiverFactory.CreateEmailReceiver();
            var result = await receiver.ReadMessagesAsync(new MailReadRequest
            {
                Folder = folder,
                UnreadOnly = unreadOnly,
                MaxItems = maxItems,
                AliasRecipient = aliasRecipient,
                ReceivedAfterUtc = receivedAfterUtc
            }, cancellationToken);

            if (result.Success && result.Messages.Count > 0)
            {
                await _communicationIngestionService.PersistInboundMessagesAsync(result.Messages);
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            Log.Warning(ex, "Email receiver configuration issue");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error while reading inbound emails");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while reading inbound emails.");
        }
    }

    /// <summary>
    /// Marks a mailbox message as read by external message identifier.
    /// </summary>
    /// <param name="externalMessageId">Provider-specific external message identifier.</param>
    /// <param name="cancellationToken">Cancellation token for aborting the operation.</param>
    /// <returns>No content when updated successfully.</returns>
    /// <response code="204">Message was marked as read.</response>
    /// <response code="400">External message id is missing or invalid.</response>
    /// <response code="401">Returned when user is not authenticated.</response>
    /// <response code="403">Returned when user has insufficient permissions.</response>
    /// <response code="404">Message was not found or could not be updated.</response>
    /// <response code="500">Returned when an unexpected server error occurs.</response>
    [HttpPatch("messages/{externalMessageId}/read")]
    [Authorize(Policy = "EmailReceiver.Write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> MarkMessageAsRead(string externalMessageId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(externalMessageId))
            {
                return BadRequest("External message id is required.");
            }

            var receiver = _emailReceiverFactory.CreateEmailReceiver();
            var marked = await receiver.MarkAsReadAsync(externalMessageId, cancellationToken);

            if (!marked)
            {
                return NotFound($"Message with external id '{externalMessageId}' was not found or could not be marked as read.");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            Log.Warning(ex, "Email receiver configuration issue while marking message as read");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error while marking message {ExternalMessageId} as read", externalMessageId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while marking the message as read.");
        }
    }

    /// <summary>
    /// Temporary diagnostics endpoint for Office365 receiver configuration and token claims.
    /// </summary>
    /// <param name="folder">Optional folder name, such as Inbox.</param>
    /// <param name="unreadOnly">When true, only unread messages are considered in endpoint generation.</param>
    /// <param name="maxItems">Maximum number of messages used to build diagnostics query.</param>
    /// <param name="aliasRecipient">Optional recipient alias email address filter used in diagnostics query.</param>
    /// <param name="receivedAfterUtc">Optional UTC timestamp used in diagnostics query filter.</param>
    /// <param name="cancellationToken">Cancellation token for aborting diagnostics operation.</param>
    /// <returns>Diagnostics result including mailbox, generated Graph URL, token audience, tenant, app id and roles.</returns>
    /// <response code="200">Returns diagnostics output.</response>
    /// <response code="400">Returned when plugin configuration is invalid or diagnostics unsupported.</response>
    /// <response code="401">Returned when user is not authenticated.</response>
    /// <response code="403">Returned when user has insufficient permissions.</response>
    /// <response code="500">Returned when an unexpected server error occurs.</response>
    [HttpGet("diagnostics")]
    [Authorize(Policy = "EmailReceiver.Read")]
    [ProducesResponseType(typeof(EmailReceiverDiagnosticsResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EmailReceiverDiagnosticsResult>> Diagnostics(
        [FromQuery] string? folder = null,
        [FromQuery] bool unreadOnly = false,
        [FromQuery] int maxItems = 25,
        [FromQuery] string? aliasRecipient = null,
        [FromQuery] DateTime? receivedAfterUtc = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var receiver = _emailReceiverFactory.CreateEmailReceiver();
            if (receiver is not IEmailReceiverDiagnostics diagnosticsReceiver)
            {
                return BadRequest("Current email receiver plugin does not support diagnostics.");
            }

            var request = new MailReadRequest
            {
                Folder = folder,
                UnreadOnly = unreadOnly,
                MaxItems = maxItems,
                AliasRecipient = aliasRecipient,
                ReceivedAfterUtc = receivedAfterUtc
            };

            var diagnostics = await diagnosticsReceiver.GetDiagnosticsAsync(request, cancellationToken);
            return Ok(diagnostics);
        }
        catch (InvalidOperationException ex)
        {
            Log.Warning(ex, "Email receiver diagnostics configuration issue");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error while generating email receiver diagnostics");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while generating diagnostics.");
        }
    }
}
