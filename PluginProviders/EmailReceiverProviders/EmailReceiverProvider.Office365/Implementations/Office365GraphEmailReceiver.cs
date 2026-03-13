using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EmailReceiverLib.Infrastructure.Settings;
using EmailReceiverLib.Interfaces;
using EmailReceiverLib.Models;
using Serilog;

namespace EmailReceiverProviders.Office365.Implementations;

public class Office365GraphEmailReceiver : IEmailReceiver, IEmailReceiverDiagnostics
{
    private static readonly HttpClient HttpClient = new();
    private readonly Office365ReceiverSettings _settings;
    private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<Office365GraphEmailReceiver>();

    public Office365GraphEmailReceiver(Office365ReceiverSettings settings)
    {
        _settings = settings;
    }

    public async Task<MailReadResult> ReadMessagesAsync(MailReadRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await GetAccessTokenAsync(cancellationToken);
            var endpoint = BuildMessagesEndpoint(request);
            var aliasRecipient = GetEffectiveAliasRecipient(request);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
            var payload = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("Office365 read failed. Mailbox: {Mailbox}. Endpoint: {Endpoint}. Status: {StatusCode}. Response: {Response}",
                    _settings.MailboxAddress,
                    endpoint,
                    (int)response.StatusCode,
                    payload);

                return new MailReadResult
                {
                    Success = false,
                    Message = "Failed to read messages from Office365 Graph API.",
                    Errors = [$"HTTP {(int)response.StatusCode}: {payload}"]
                };
            }

            using var json = JsonDocument.Parse(payload);
            var root = json.RootElement;

            var result = new MailReadResult
            {
                Success = true,
                Message = "Messages retrieved successfully",
                NextCursor = root.TryGetProperty("@odata.nextLink", out var next) ? next.GetString() : null
            };

            if (!root.TryGetProperty("value", out var messages) || messages.ValueKind != JsonValueKind.Array)
            {
                return result;
            }

            foreach (var message in messages.EnumerateArray())
            {
                result.Messages.Add(MapMessage(message));
            }

            if (!string.IsNullOrWhiteSpace(aliasRecipient))
            {
                result.Messages = result.Messages
                    .Where(m => m.ToAddresses.Any(to => to.Equals(aliasRecipient, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error reading emails from Office365 for mailbox {Mailbox}", _settings.MailboxAddress);
            return new MailReadResult
            {
                Success = false,
                Message = "Exception occurred while reading messages from Office365.",
                Errors = [ex.Message]
            };
        }
    }

    public async Task<EmailReceiverDiagnosticsResult> GetDiagnosticsAsync(MailReadRequest request, CancellationToken cancellationToken = default)
    {
        var endpoint = BuildMessagesEndpoint(request);

        try
        {
            var token = await GetAccessTokenAsync(cancellationToken);
            var tokenInfo = ParseJwtPayload(token);

            Log.Information(
                "Office365 diagnostics. Mailbox: {Mailbox}. AliasRecipient: {AliasRecipient}. Endpoint: {Endpoint}. TokenAud: {Audience}. TokenTid: {TenantId}. TokenAppId: {ClientId}. TokenRoles: {Roles}",
                _settings.MailboxAddress,
                string.IsNullOrWhiteSpace(request.AliasRecipient) ? _settings.AliasRecipient : request.AliasRecipient,
                endpoint,
                tokenInfo.Audience,
                tokenInfo.TenantId,
                tokenInfo.ClientId,
                tokenInfo.Roles);

            return new EmailReceiverDiagnosticsResult
            {
                Success = true,
                Message = "Diagnostics generated successfully.",
                MailboxAddress = _settings.MailboxAddress,
                AliasRecipient = string.IsNullOrWhiteSpace(request.AliasRecipient) ? _settings.AliasRecipient : request.AliasRecipient,
                GraphMessagesEndpoint = endpoint,
                TokenTenantId = tokenInfo.TenantId,
                TokenClientId = tokenInfo.ClientId,
                TokenAudience = tokenInfo.Audience,
                TokenExpiresAtUtc = tokenInfo.ExpiresAtUtc,
                TokenRoles = tokenInfo.Roles
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Office365 diagnostics failed for mailbox {Mailbox}. Endpoint: {Endpoint}", _settings.MailboxAddress, endpoint);

            return new EmailReceiverDiagnosticsResult
            {
                Success = false,
                Message = "Diagnostics failed.",
                MailboxAddress = _settings.MailboxAddress,
                AliasRecipient = string.IsNullOrWhiteSpace(request.AliasRecipient) ? _settings.AliasRecipient : request.AliasRecipient,
                GraphMessagesEndpoint = endpoint,
                Errors = [ex.Message]
            };
        }
    }

    public async Task<bool> MarkAsReadAsync(string externalMessageId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(externalMessageId))
        {
            return false;
        }

        try
        {
            var token = await GetAccessTokenAsync(cancellationToken);
            var endpoint = $"https://graph.microsoft.com/v1.0/users/{Uri.EscapeDataString(_settings.MailboxAddress)}/messages/{Uri.EscapeDataString(externalMessageId)}";

            using var request = new HttpRequestMessage(HttpMethod.Patch, endpoint)
            {
                Content = new StringContent("{\"isRead\":true}", Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await HttpClient.SendAsync(request, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error marking message {ExternalMessageId} as read", externalMessageId);
            return false;
        }
    }

    private string BuildMessagesEndpoint(MailReadRequest request)
    {
        var folder = string.IsNullOrWhiteSpace(request.Folder)
            ? _settings.DefaultFolder
            : request.Folder!;

        var top = request.MaxItems <= 0
            ? _settings.DefaultMaxItems
            : request.MaxItems;

        var filters = new List<string>();
        if (request.UnreadOnly)
        {
            filters.Add("isRead eq false");
        }

        if (request.ReceivedAfterUtc.HasValue)
        {
            filters.Add($"receivedDateTime ge {request.ReceivedAfterUtc.Value.ToString("o", CultureInfo.InvariantCulture)}");
        }

        var sb = new StringBuilder();
        sb.Append("https://graph.microsoft.com/v1.0/users/");
        sb.Append(Uri.EscapeDataString(_settings.MailboxAddress));
        sb.Append("/mailFolders/");
        sb.Append(Uri.EscapeDataString(folder));
        sb.Append("/messages?");
        sb.Append("$top=");
        sb.Append(top);
        sb.Append("&$orderby=receivedDateTime desc");
        sb.Append("&$select=id,internetMessageId,subject,from,toRecipients,bodyPreview,receivedDateTime,isRead,hasAttachments");

        if (filters.Count > 0)
        {
            sb.Append("&$filter=");
            sb.Append(Uri.EscapeDataString(string.Join(" and ", filters)));
        }

        return sb.ToString();
    }

    private string? GetEffectiveAliasRecipient(MailReadRequest request)
    {
        var aliasRecipient = string.IsNullOrWhiteSpace(request.AliasRecipient)
            ? _settings.AliasRecipient
            : request.AliasRecipient;

        return string.IsNullOrWhiteSpace(aliasRecipient)
            ? null
            : aliasRecipient.Trim();
    }

    private static (string TenantId, string ClientId, string? Audience, DateTime? ExpiresAtUtc, List<string> Roles) ParseJwtPayload(string jwt)
    {
        var parts = jwt.Split('.');
        if (parts.Length < 2)
        {
            throw new InvalidOperationException("Invalid JWT format.");
        }

        var payloadJson = DecodeBase64Url(parts[1]);
        using var payload = JsonDocument.Parse(payloadJson);
        var root = payload.RootElement;

        var tenantId = root.TryGetProperty("tid", out var tid) ? tid.GetString() ?? string.Empty : string.Empty;
        var clientId = root.TryGetProperty("appid", out var appid) ? appid.GetString() ?? string.Empty : string.Empty;
        var audience = root.TryGetProperty("aud", out var aud) ? aud.GetString() : null;

        DateTime? expiresAtUtc = null;
        if (root.TryGetProperty("exp", out var exp) && exp.TryGetInt64(out var expUnix))
        {
            expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
        }

        var roles = new List<string>();
        if (root.TryGetProperty("roles", out var rolesElement) && rolesElement.ValueKind == JsonValueKind.Array)
        {
            roles.AddRange(rolesElement
                .EnumerateArray()
                .Where(r => r.ValueKind == JsonValueKind.String)
                .Select(r => r.GetString() ?? string.Empty)
                .Where(r => !string.IsNullOrWhiteSpace(r)));
        }

        return (tenantId, clientId, audience, expiresAtUtc, roles);
    }

    private static string DecodeBase64Url(string value)
    {
        var padded = value.Replace('-', '+').Replace('_', '/');
        while (padded.Length % 4 != 0)
        {
            padded += "=";
        }

        var bytes = Convert.FromBase64String(padded);
        return Encoding.UTF8.GetString(bytes);
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        var endpoint = $"https://login.microsoftonline.com/{Uri.EscapeDataString(_settings.TenantId)}/oauth2/v2.0/token";
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _settings.ClientId,
                ["client_secret"] = _settings.ClientSecret,
                ["scope"] = "https://graph.microsoft.com/.default",
                ["grant_type"] = "client_credentials"
            })
        };

        using var response = await HttpClient.SendAsync(request, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        response.EnsureSuccessStatusCode();

        using var json = JsonDocument.Parse(payload);
        if (!json.RootElement.TryGetProperty("access_token", out var tokenElement))
        {
            throw new InvalidOperationException("Office365 token response does not include access_token.");
        }

        return tokenElement.GetString() ?? throw new InvalidOperationException("Office365 access token is empty.");
    }

    private static InboundEmailMessage MapMessage(JsonElement message)
    {
        var toAddresses = new List<string>();
        if (message.TryGetProperty("toRecipients", out var recipients) && recipients.ValueKind == JsonValueKind.Array)
        {
            foreach (var recipient in recipients.EnumerateArray())
            {
                if (recipient.TryGetProperty("emailAddress", out var addressObj)
                    && addressObj.TryGetProperty("address", out var address)
                    && !string.IsNullOrWhiteSpace(address.GetString()))
                {
                    toAddresses.Add(address.GetString()!);
                }
            }
        }

        string fromAddress = string.Empty;
        string fromName = string.Empty;
        if (message.TryGetProperty("from", out var from)
            && from.TryGetProperty("emailAddress", out var emailAddress))
        {
            fromAddress = emailAddress.TryGetProperty("address", out var addr) ? addr.GetString() ?? string.Empty : string.Empty;
            fromName = emailAddress.TryGetProperty("name", out var name) ? name.GetString() ?? string.Empty : string.Empty;
        }

        DateTime? received = null;
        if (message.TryGetProperty("receivedDateTime", out var receivedDate)
            && DateTime.TryParse(receivedDate.GetString(), out var parsed))
        {
            received = parsed;
        }

        return new InboundEmailMessage
        {
            ExternalMessageId = message.TryGetProperty("id", out var id) ? id.GetString() ?? string.Empty : string.Empty,
            InternetMessageId = message.TryGetProperty("internetMessageId", out var internetId) ? internetId.GetString() : null,
            Subject = message.TryGetProperty("subject", out var subject) ? subject.GetString() ?? string.Empty : string.Empty,
            FromAddress = fromAddress,
            FromName = fromName,
            ToAddresses = toAddresses,
            BodyPreview = message.TryGetProperty("bodyPreview", out var bodyPreview) ? bodyPreview.GetString() ?? string.Empty : string.Empty,
            ReceivedAtUtc = received,
            IsRead = message.TryGetProperty("isRead", out var isRead) && isRead.GetBoolean(),
            HasAttachments = message.TryGetProperty("hasAttachments", out var hasAttachments) && hasAttachments.GetBoolean()
        };
    }
}
