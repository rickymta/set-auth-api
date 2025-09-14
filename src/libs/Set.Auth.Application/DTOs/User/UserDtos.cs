namespace Set.Auth.Application.DTOs.User;

/// <summary>
/// User data transfer object for updating user information
/// </summary>
public class UpdateUserRequestDto
{
    /// <summary>
    /// First name of the user
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name of the user
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Phone number of the user (optional, must be Vietnamese format if provided)
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Avatar URL of the user (optional)
    /// </summary>
    public string? Avatar { get; set; }
}

/// <summary>
/// Update user response data transfer object
/// </summary>
public class UpdateUserResponseDto
{
    /// <summary>
    /// ID of the user
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Email of the user
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Phone number of the user
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// First name of the user
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name of the user
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the user (computed property)
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Avatar URL of the user
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// Indicates whether the user's email is verified
    /// </summary>
    public bool IsEmailVerified { get; set; }

    /// <summary>
    /// Indicates whether the user's phone number is verified
    /// </summary>
    public bool IsPhoneVerified { get; set; }

    /// <summary>
    /// Update timestamp of the user
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
