using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Defines operations for DNS troubleshooting checks.
/// </summary>
public interface IDnsTroubleshootService
{
    /// <summary>
    /// Runs DNS troubleshoot tests for a domain.
    /// </summary>
    /// <param name="domainId">The domain identifier.</param>
    /// <returns>A troubleshooting report for the domain.</returns>
    Task<DnsTroubleshootReportDto?> RunForDomainAsync(int domainId);
}
