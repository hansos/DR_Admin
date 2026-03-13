using EmailReceiverLib.Models;

namespace EmailReceiverLib.Interfaces;

public interface IEmailReceiverDiagnostics
{
    Task<EmailReceiverDiagnosticsResult> GetDiagnosticsAsync(MailReadRequest request, CancellationToken cancellationToken = default);
}
