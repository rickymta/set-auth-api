namespace Set.Auth.Application.DTOs.Permission;

/// <summary>
/// Data transfer object for permission information
/// </summary>
public class PermissionDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the permission
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the permission
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the permission
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the resource that this permission applies to
    /// </summary>
    public string Resource { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the action allowed by this permission
    /// </summary>
    public string Action { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a value indicating whether the permission is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the permission was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the permission was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the number of roles that have this permission
    /// </summary>
    public int RoleCount { get; set; }
}

/// <summary>
/// Data transfer object for creating a new permission
/// </summary>
public class PermissionCreateDto
{
    /// <summary>
    /// Gets or sets the name of the permission
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the permission
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the resource that this permission applies to
    /// </summary>
    public string Resource { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the action allowed by this permission
    /// </summary>
    public string Action { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a value indicating whether the permission is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Data transfer object for updating an existing permission
/// </summary>
public class PermissionUpdateDto
{
    /// <summary>
    /// Gets or sets the name of the permission
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the permission
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the resource that this permission applies to
    /// </summary>
    public string Resource { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the action allowed by this permission
    /// </summary>
    public string Action { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a value indicating whether the permission is active
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// Data transfer object for bulk permission operations
/// </summary>
public class BulkPermissionDto
{
    /// <summary>
    /// Gets or sets the collection of permission IDs
    /// </summary>
    public ICollection<Guid> PermissionIds { get; set; } = [];
    
    /// <summary>
    /// Gets or sets the operation to perform (activate, deactivate, delete)
    /// </summary>
    public string Operation { get; set; } = string.Empty;
}

/// <summary>
/// Data transfer object for permission assignment to roles
/// </summary>
public class PermissionAssignmentDto
{
    /// <summary>
    /// Gets or sets the role identifier
    /// </summary>
    public Guid RoleId { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of permission IDs to assign to the role
    /// </summary>
    public ICollection<Guid> PermissionIds { get; set; } = [];
}

/// <summary>
/// Data transfer object for permission assignment response
/// </summary>
public class PermissionAssignmentResponseDto
{
    /// <summary>
    /// Gets or sets the role identifier
    /// </summary>
    public Guid RoleId { get; set; }
    
    /// <summary>
    /// Gets or sets the role name
    /// </summary>
    public string RoleName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the collection of assigned permissions
    /// </summary>
    public ICollection<PermissionDto> Permissions { get; set; } = [];
    
    /// <summary>
    /// Gets or sets the date and time when the assignment was completed
    /// </summary>
    public DateTime AssignedAt { get; set; }
}

/// <summary>
/// Data transfer object for paginated permission list
/// </summary>
public class PermissionListDto
{
    /// <summary>
    /// Gets or sets the collection of permissions
    /// </summary>
    public ICollection<PermissionDto> Permissions { get; set; } = [];
    
    /// <summary>
    /// Gets or sets the total number of permissions
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

/// <summary>
/// Data transfer object for permission filtering and search
/// </summary>
public class PermissionFilterDto
{
    /// <summary>
    /// Gets or sets the search term for permission name or description
    /// </summary>
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// Gets or sets the resource filter
    /// </summary>
    public string? Resource { get; set; }
    
    /// <summary>
    /// Gets or sets the action filter
    /// </summary>
    public string? Action { get; set; }
    
    /// <summary>
    /// Gets or sets the active status filter
    /// </summary>
    public bool? IsActive { get; set; }
    
    /// <summary>
    /// Gets or sets the page number for pagination
    /// </summary>
    public int PageNumber { get; set; } = 1;
    
    /// <summary>
    /// Gets or sets the page size for pagination
    /// </summary>
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// Gets or sets the sort field
    /// </summary>
    public string SortBy { get; set; } = "Name";
    
    /// <summary>
    /// Gets or sets the sort direction (asc or desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";
}
