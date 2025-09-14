namespace Set.Auth.Application.DTOs.Role;

/// <summary>
/// Data transfer object for role information
/// </summary>
public class RoleDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the role
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the role
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the role
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the role is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the role was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the role was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of permissions assigned to this role
    /// </summary>
    public ICollection<string> Permissions { get; set; } = [];
    
    /// <summary>
    /// Gets or sets the number of users assigned to this role
    /// </summary>
    public int UserCount { get; set; }
}

/// <summary>
/// Data transfer object for creating a new role
/// </summary>
public class RoleCreateDto
{
    /// <summary>
    /// Gets or sets the name of the role
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the role
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the role is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the collection of permission IDs to assign to this role
    /// </summary>
    public ICollection<Guid> PermissionIds { get; set; } = [];
}

/// <summary>
/// Data transfer object for updating an existing role
/// </summary>
public class RoleUpdateDto
{
    /// <summary>
    /// Gets or sets the name of the role
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the role
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the role is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of permission IDs to assign to this role
    /// </summary>
    public ICollection<Guid> PermissionIds { get; set; } = [];
}

/// <summary>
/// Data transfer object for role assignment operations
/// </summary>
public class RoleAssignmentDto
{
    /// <summary>
    /// Gets or sets the user identifier
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of role IDs to assign to the user
    /// </summary>
    public ICollection<Guid> RoleIds { get; set; } = [];
    
    /// <summary>
    /// Gets or sets the expiration date and time for the role assignment (optional)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// Data transfer object for role assignment response
/// </summary>
public class RoleAssignmentResponseDto
{
    /// <summary>
    /// Gets or sets the user identifier
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Gets or sets the user's full name
    /// </summary>
    public string UserName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the collection of assigned roles
    /// </summary>
    public ICollection<RoleDto> Roles { get; set; } = [];
    
    /// <summary>
    /// Gets or sets the date and time when the assignment was completed
    /// </summary>
    public DateTime AssignedAt { get; set; }
}

/// <summary>
/// Data transfer object for paginated role list
/// </summary>
public class RoleListDto
{
    /// <summary>
    /// Gets or sets the collection of roles
    /// </summary>
    public ICollection<RoleDto> Roles { get; set; } = [];
    
    /// <summary>
    /// Gets or sets the total number of roles
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// Gets or sets the current page number
    /// </summary>
    public int PageNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the page size
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// Gets or sets the total number of pages
    /// </summary>
    public int TotalPages { get; set; }
}
