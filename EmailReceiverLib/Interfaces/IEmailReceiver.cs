using EmailReceiverLib.Models;

namespace EmailReceiverLib.Interfaces;

public interface IEmailReceiver
{
    Task<MailReadResult> ReadMessagesAsync(MailReadRequest request, CancellationToken cancellationToken = default);
    Task<bool> MarkAsReadAsync(string externalMessageId, CancellationToken cancellationToken = default);
}
