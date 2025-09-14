namespace Set.Auth.Application.DTOs.User;

/// <summary>
/// Data transfer object for creating a new user (admin operation)
/// </summary>
public class UserCreateDto
{
    /// <summary>
    /// Gets or sets the user's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's phone number (optional, must be Vietnamese format)
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the user's password
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the password confirmation
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's avatar URL (optional)
    /// </summary>
    public string? Avatar { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the user's email is verified
    /// </summary>
    public bool IsEmailVerified { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether the user's phone number is verified
    /// </summary>
    public bool IsPhoneVerified { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether the user account is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the collection of role IDs to assign to the user
    /// </summary>
    public ICollection<Guid> RoleIds { get; set; } = [];
}

/// <summary>
/// Data transfer object for updating user information (admin operation)
/// </summary>
public class UserAdminUpdateDto
{
    /// <summary>
    /// Id update
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's phone number (optional)
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the user's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's avatar URL (optional)
    /// </summary>
    public string? Avatar { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the user's email is verified
    /// </summary>
    public bool IsEmailVerified { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the user's phone number is verified
    /// </summary>
    public bool IsPhoneVerified { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the user account is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of role IDs to assign to the user
    /// </summary>
    public ICollection<Guid> RoleIds { get; set; } = new List<Guid>();
}

/// <summary>
/// Data transfer object for user list display
/// </summary>
public class UserListItemDto
{
    /// <summary>
    /// Gets or sets the user's unique identifier
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the user's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's phone number (optional)
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the user's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the user's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets the user's full name by combining first and last names
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    /// <summary>
    /// Gets or sets the user's avatar URL (optional)
    /// </summary>
    public string? Avatar { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the user's email is verified
    /// </summary>
    public bool IsEmailVerified { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the user's phone number is verified
    /// </summary>
    public bool IsPhoneVerified { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the user account is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the user account was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time of the user's last login (optional)
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of role names assigned to the user
    /// </summary>
    public ICollection<string> Roles { get; set; } = [];
}

/// <summary>
/// Data transfer object for paginated user list
/// </summary>
public class UserListDto
{
    /// <summary>
    /// Gets or sets the collection of users
    /// </summary>
    public ICollection<UserListItemDto> Users { get; set; } = [];
    
    /// <summary>
    /// Gets or sets the total number of users
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
/// Data transfer object for user filtering and search
/// </summary>
public class UserFilterDto
{
    /// <summary>
    /// Gets or sets the search term for user name or email
    /// </summary>
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// Gets or sets the role filter
    /// </summary>
    public string? Role { get; set; }
    
    /// <summary>
    /// Gets or sets the active status filter
    /// </summary>
    public bool? IsActive { get; set; }
    
    /// <summary>
    /// Gets or sets the email verification status filter
    /// </summary>
    public bool? IsEmailVerified { get; set; }
    
    /// <summary>
    /// Gets or sets the phone verification status filter
    /// </summary>
    public bool? IsPhoneVerified { get; set; }
    
    /// <summary>
    /// Gets or sets the created date range start
    /// </summary>
    public DateTime? CreatedFrom { get; set; }
    
    /// <summary>
    /// Gets or sets the created date range end
    /// </summary>
    public DateTime? CreatedTo { get; set; }
    
    /// <summary>
    /// Gets or sets the last login date range start
    /// </summary>
    public DateTime? LastLoginFrom { get; set; }
    
    /// <summary>
    /// Gets or sets the last login date range end
    /// </summary>
    public DateTime? LastLoginTo { get; set; }
    
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
    public string SortBy { get; set; } = "CreatedAt";
    
    /// <summary>
    /// Gets or sets the sort direction (asc or desc)
    /// </summary>
    public string SortDirection { get; set; } = "desc";
}

/// <summary>
/// Data transfer object for bulk user operations
/// </summary>
public class BulkUserDto
{
    /// <summary>
    /// Gets or sets the collection of user IDs
    /// </summary>
    public ICollection<Guid> UserIds { get; set; } = new List<Guid>();
    
    /// <summary>
    /// Gets or sets the operation to perform (activate, deactivate, delete)
    /// </summary>
    public string Operation { get; set; } = string.Empty;
}

/// <summary>
/// Data transfer object for user statistics
/// </summary>
public class UserStatsDto
{
    /// <summary>
    /// Gets or sets the total number of users
    /// </summary>
    public int TotalUsers { get; set; }
    
    /// <summary>
    /// Gets or sets the number of active users
    /// </summary>
    public int ActiveUsers { get; set; }
    
    /// <summary>
    /// Gets or sets the number of inactive users
    /// </summary>
    public int InactiveUsers { get; set; }
    
    /// <summary>
    /// Gets or sets the number of users with verified email
    /// </summary>
    public int EmailVerifiedUsers { get; set; }
    
    /// <summary>
    /// Gets or sets the number of users with verified phone
    /// </summary>
    public int PhoneVerifiedUsers { get; set; }
    
    /// <summary>
    /// Gets or sets the number of users registered in the last 30 days
    /// </summary>
    public int NewUsersLast30Days { get; set; }
    
    /// <summary>
    /// Gets or sets the number of users who logged in today
    /// </summary>
    public int ActiveUsersToday { get; set; }
}
