namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a user
/// </summary>
public class UserDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the user
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the customer ID associated with this user (optional)
    /// </summary>
    public int? CustomerId { get; set; }
    
    /// <summary>
    /// Gets or sets the user's username
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a value indicating whether the user account is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the user was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the user was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}


/// <summary>
/// Data transfer object for creating a new user
/// </summary>
public class CreateUserDto
{
    /// <summary>
    /// Gets or sets the customer ID associated with this user (optional)
    /// </summary>
    public int? CustomerId { get; set; }
    
    /// <summary>
    /// Gets or sets the user's username
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's password
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a value indicating whether the user account is active (default: true)
    /// </summary>
    public bool IsActive { get; set; } = true;
}


/// <summary>
/// Data transfer object for updating an existing user
/// </summary>
public class UpdateUserDto
{
    public int? CustomerId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
