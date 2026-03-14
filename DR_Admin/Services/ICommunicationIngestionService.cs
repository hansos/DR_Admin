using EmailReceiverLib.Models;

namespace ISPAdmin.Services;

/// <summary>
/// Defines ingestion operations for inbound communication messages.
/// </summary>
public interface ICommunicationIngestionService
{
    /// <summary>
    /// Persists inbound email messages to communication threads and messages.
    /// </summary>
    /// <param name="messages">Inbound messages to persist.</param>
    /// <returns>The number of newly persisted inbound messages.</returns>
    Task<int> PersistInboundMessagesAsync(IEnumerable<InboundEmailMessage> messages);
}
