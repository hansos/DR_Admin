namespace ISPAdmin.Data.Entities;

public class User : EntityBase
{
    public int? CustomerId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? EmailConfirmed { get; set; }
    public bool IsActive { get; set; }

    /// <summary>
    /// Normalized version of Username for case-insensitive searches
    /// </summary>
    public string NormalizedUsername { get; set; } = string.Empty;

    public Customer? Customer { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Token> Tokens { get; set; } = new List<Token>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    public ICollection<LoginHistory> LoginHistories { get; set; } = new List<LoginHistory>();
}
