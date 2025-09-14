namespace Set.Auth.Application.DTOs.User;

/// <summary>
/// Change password request data transfer object
/// </summary>
public class ChangePasswordRequestDto
{
    /// <summary>
    /// Current password of the user
    /// </summary>
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// New password to set
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Confirmation of the new password
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;
}
