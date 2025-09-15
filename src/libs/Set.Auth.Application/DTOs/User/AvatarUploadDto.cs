using Microsoft.AspNetCore.Http;

namespace Set.Auth.Application.DTOs.User;

/// <summary>
/// DTO for avatar upload request
/// </summary>
public class AvatarUploadDto
{
    /// <summary>
    /// The avatar image file (max 5MB, formats: JPG, PNG, GIF)
    /// </summary>
    public required IFormFile AvatarFile { get; set; }
}
