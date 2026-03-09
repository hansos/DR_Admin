using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Represents a registered domain.
/// </summary>
public class RegisteredDomainDto
{
    /// <summary>
    /// Gets or sets the domain ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the customer ID.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the related service ID.
    /// </summary>
    public int? ServiceId { get; set; }

    /// <summary>
    /// Gets or sets the domain name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the registrar/provider ID.
    /// </summary>
    public int ProviderId { get; set; }

    /// <summary>
    /// Gets or sets the overall domain lifecycle status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the asynchronous registration workflow status.
    /// </summary>
    public DomainRegistrationStatus RegistrationStatus { get; set; }

    /// <summary>
    /// Gets or sets the successful registration timestamp.
    /// </summary>
    public DateTime? RegistrationDate { get; set; }

    /// <summary>
    /// Gets or sets the expiration date.
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets the number of registration attempts.
    /// </summary>
    public int RegistrationAttemptCount { get; set; }

    /// <summary>
    /// Gets or sets the last registration attempt timestamp.
    /// </summary>
    public DateTime? LastRegistrationAttemptUtc { get; set; }

    /// <summary>
    /// Gets or sets the next planned registration attempt timestamp.
    /// </summary>
    public DateTime? NextRegistrationAttemptUtc { get; set; }

    /// <summary>
    /// Gets or sets the latest registration error.
    /// </summary>
    public string? RegistrationError { get; set; }

    /// <summary>
    /// Gets or sets the created timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the updated timestamp.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets customer details.
    /// </summary>
    public CustomerDto? Customer { get; set; }

    /// <summary>
    /// Gets or sets registrar details.
    /// </summary>
    public RegistrarDto? Registrar { get; set; }
}

/// <summary>
/// Represents the payload for creating a registered domain.
/// </summary>
public class CreateRegisteredDomainDto
{
    /// <summary>
    /// Gets or sets the customer ID.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the related service ID.
    /// </summary>
    public int? ServiceId { get; set; }

    /// <summary>
    /// Gets or sets the domain name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the registrar/provider ID.
    /// </summary>
    public int ProviderId { get; set; }

    /// <summary>
    /// Gets or sets the overall domain lifecycle status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the asynchronous registration workflow status.
    /// </summary>
    public DomainRegistrationStatus RegistrationStatus { get; set; } = DomainRegistrationStatus.PendingPayment;

    /// <summary>
    /// Gets or sets the successful registration timestamp.
    /// </summary>
    public DateTime? RegistrationDate { get; set; }

    /// <summary>
    /// Gets or sets the number of registration attempts.
    /// </summary>
    public int RegistrationAttemptCount { get; set; }

    /// <summary>
    /// Gets or sets the last registration attempt timestamp.
    /// </summary>
    public DateTime? LastRegistrationAttemptUtc { get; set; }

    /// <summary>
    /// Gets or sets the next planned registration attempt timestamp.
    /// </summary>
    public DateTime? NextRegistrationAttemptUtc { get; set; }

    /// <summary>
    /// Gets or sets the latest registration error.
    /// </summary>
    public string? RegistrationError { get; set; }

    /// <summary>
    /// Gets or sets the expiration date.
    /// </summary>
    public DateTime? ExpirationDate { get; set; }
}

/// <summary>
/// Represents the payload for updating a registered domain.
/// </summary>
public class UpdateRegisteredDomainDto
{
    /// <summary>
    /// Gets or sets the customer ID.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the related service ID.
    /// </summary>
    public int? ServiceId { get; set; }

    /// <summary>
    /// Gets or sets the domain name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the registrar/provider ID.
    /// </summary>
    public int ProviderId { get; set; }

    /// <summary>
    /// Gets or sets the overall domain lifecycle status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the asynchronous registration workflow status.
    /// </summary>
    public DomainRegistrationStatus RegistrationStatus { get; set; } = DomainRegistrationStatus.PendingPayment;

    /// <summary>
    /// Gets or sets the successful registration timestamp.
    /// </summary>
    public DateTime? RegistrationDate { get; set; }

    /// <summary>
    /// Gets or sets the number of registration attempts.
    /// </summary>
    public int RegistrationAttemptCount { get; set; }

    /// <summary>
    /// Gets or sets the last registration attempt timestamp.
    /// </summary>
    public DateTime? LastRegistrationAttemptUtc { get; set; }

    /// <summary>
    /// Gets or sets the next planned registration attempt timestamp.
    /// </summary>
    public DateTime? NextRegistrationAttemptUtc { get; set; }

    /// <summary>
    /// Gets or sets the latest registration error.
    /// </summary>
    public string? RegistrationError { get; set; }

    /// <summary>
    /// Gets or sets the expiration date.
    /// </summary>
    public DateTime? ExpirationDate { get; set; }
}
