namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the asynchronous registration state for a paid domain.
/// </summary>
public enum DomainRegistrationStatus
{
    /// <summary>
    /// Domain has not been paid yet.
    /// </summary>
    PendingPayment = 0,

    /// <summary>
    /// Payment is completed and registration is pending processing.
    /// </summary>
    PaidPendingRegistration = 1,

    /// <summary>
    /// Registration is currently being processed.
    /// </summary>
    RegistrationInProgress = 2,

    /// <summary>
    /// Domain has been successfully registered.
    /// </summary>
    Registered = 3,

    /// <summary>
    /// Registration failed and may be retried.
    /// </summary>
    RegistrationFailed = 4
}
