namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a user role
/// </summary>
public class RoleDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the role
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the role name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the role description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the date and time when the role was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the role was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}


/// <summary>
/// Data transfer object for creating a new role
/// </summary>
public class CreateRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}


/// <summary>
/// Data transfer object for updating an existing role
/// </summary>
public class UpdateRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
