namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the type of action recorded in registered domain history.
/// </summary>
public enum RegisteredDomainHistoryActionType
{
    /// <summary>
    /// Domain registration lifecycle action.
    /// </summary>
    Registration = 0,

    /// <summary>
    /// Payment-related action for a domain.
    /// </summary>
    Payment = 1,

    /// <summary>
    /// Outbound message action for a domain.
    /// </summary>
    MessageSent = 2,

    /// <summary>
    /// DNS change action for a domain.
    /// </summary>
    DnsChange = 3,

    /// <summary>
    /// Contact person or assignment change action for a domain.
    /// </summary>
    ContactPersonChange = 4,

    /// <summary>
    /// Generic domain change action.
    /// </summary>
    DomainChange = 5
}
