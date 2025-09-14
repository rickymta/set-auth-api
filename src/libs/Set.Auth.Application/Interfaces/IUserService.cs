using Microsoft.AspNetCore.Http;
using Set.Auth.Application.DTOs.Auth;
using Set.Auth.Application.DTOs.User;

namespace Set.Auth.Application.Interfaces;

/// <summary>
/// Service interface for user management operations
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves a user by their unique identifier
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>The user information if found; otherwise, null</returns>
    Task<UserDto?> GetUserByIdAsync(Guid userId);
    
    /// <summary>
    /// Retrieves the current user's information
    /// </summary>
    /// <param name="userId">The current user identifier</param>
    /// <returns>The current user information if found; otherwise, null</returns>
    Task<UserDto?> GetCurrentUserAsync(Guid userId);
    
    /// <summary>
    /// Updates a user's profile information
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="request">The update request containing new user information</param>
    /// <returns>The updated user information</returns>
    Task<UpdateUserResponseDto> UpdateUserAsync(Guid userId, UpdateUserRequestDto request);
    
    /// <summary>
    /// Changes a user's password
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="request">The password change request containing old and new passwords</param>
    /// <returns>True if the password was changed successfully; otherwise, false</returns>
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request);
    
    /// <summary>
    /// Deactivates a user account
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>True if the user was deactivated successfully; otherwise, false</returns>
    Task<bool> DeactivateUserAsync(Guid userId);
    
    /// <summary>
    /// Activates a user account
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>True if the user was activated successfully; otherwise, false</returns>
    Task<bool> ActivateUserAsync(Guid userId);
    
    /// <summary>
    /// Retrieves all users with pagination and filtering
    /// </summary>
    /// <param name="filter">The filter criteria</param>
    /// <returns>A paginated list of users</returns>
    Task<UserListDto> GetUsersPagedAsync(UserFilterDto filter);
    
    /// <summary>
    /// Creates a new user (admin operation)
    /// </summary>
    /// <param name="createDto">The user creation data</param>
    /// <returns>The created user information</returns>
    Task<UserDto> CreateUserAsync(UserCreateDto createDto);
    
    /// <summary>
    /// Updates a user (admin operation)
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="updateDto">The user update data</param>
    /// <returns>The updated user information</returns>
    Task<UserDto> UpdateUserAdminAsync(Guid userId, UserAdminUpdateDto updateDto);
    
    /// <summary>
    /// Deletes a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task DeleteUserAsync(Guid userId);
    
    /// <summary>
    /// Performs bulk operations on users
    /// </summary>
    /// <param name="bulkDto">The bulk operation data</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task BulkUserOperationAsync(BulkUserDto bulkDto);
    
    /// <summary>
    /// Gets user statistics
    /// </summary>
    /// <returns>User statistics</returns>
    Task<UserStatsDto> GetUserStatsAsync();
    
    /// <summary>
    /// Checks if a user exists by their identifier
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>True if the user exists; otherwise, false</returns>
    Task<bool> UserExistsAsync(Guid userId);
    
    /// <summary>
    /// Checks if an email address is already in use
    /// </summary>
    /// <param name="email">The email address to check</param>
    /// <param name="excludeUserId">Optional user ID to exclude from the check</param>
    /// <returns>True if the email exists; otherwise, false</returns>
    Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null);
    
    /// <summary>
    /// Checks if a phone number is already in use
    /// </summary>
    /// <param name="phoneNumber">The phone number to check</param>
    /// <param name="excludeUserId">Optional user ID to exclude from the check</param>
    /// <returns>True if the phone number exists; otherwise, false</returns>
    Task<bool> PhoneNumberExistsAsync(string phoneNumber, Guid? excludeUserId = null);
    
    /// <summary>
    /// Validates if the user has permission to perform user operations
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="operation">The operation being performed</param>
    /// <returns>True if the user has permission; otherwise, false</returns>
    Task<bool> CanPerformUserOperationAsync(Guid userId, string operation);
    
    /// <summary>
    /// Uploads an avatar image for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="avatarFile">The avatar image file</param>
    /// <returns>The URL of the uploaded avatar</returns>
    Task<string> UploadAvatarAsync(Guid userId, IFormFile avatarFile);
    
    /// <summary>
    /// Deletes a user's avatar
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>True if the avatar was deleted successfully; otherwise, false</returns>
    Task<bool> DeleteAvatarAsync(Guid userId);
    
    /// <summary>
    /// Gets an avatar image by filename
    /// </summary>
    /// <param name="filename">The avatar filename</param>
    /// <returns>Stream of the avatar image</returns>
    Task<Stream> GetAvatarAsync(string filename);
}
