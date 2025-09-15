using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Set.Auth.Application.DTOs.Auth;
using Set.Auth.Application.DTOs.User;
using Set.Auth.Application.DTOs.Role;
using Set.Auth.Application.Interfaces;
using Set.Auth.Api.Controllers.Base;

namespace Set.Auth.Api.Controllers;

/// <summary>
/// Controller for user management operations
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UserController"/> class
/// </remarks>
/// <param name="userService">The user service</param>
/// <param name="permissionService">The permission service</param>
/// <param name="roleService">The role service</param>
/// <param name="configuration">The configuration service</param>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(IUserService userService, IPermissionService permissionService, IRoleService roleService, IConfiguration configuration) : BaseController
{
	private readonly IConfiguration _configuration = configuration;
	/// <summary>
	/// Retrieves current user's profile
	/// </summary>
	/// <returns>Current user profile</returns>
	/// <response code="200">Profile retrieved successfully</response>
	/// <response code="404">User not found</response>
	[HttpGet("profile")]
	public async Task<ActionResult<UserDto>> GetCurrentUser()
	{
		try
		{
			var userId = GetCurrentUserId();
			var user = await userService.GetCurrentUserAsync(userId);
			if (user == null)
			{
				return Ok(ErrorMessage("User not found", 404));
			}

			return Ok(SuccessData(user));
		}
		catch (Exception ex)
		{
			return Ok(ErrorMessage(ex.Message, 400));
		}
	}

	/// <summary>
	/// Retrieves all users with pagination and filtering (Admin only)
	/// </summary>
	/// <param name="filter">Filter criteria</param>
	/// <returns>Paginated list of users</returns>
	/// <response code="200">Users retrieved successfully</response>
	/// <response code="400">Invalid request parameters</response>
	/// <response code="403">Insufficient permissions</response>
	[HttpGet]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<UserListDto>> GetUsers([FromQuery] UserFilterDto filter)
	{
		try
		{
			var currentUserId = GetCurrentUserId();
			if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "read"))
			{
				return Ok(ErrorMessage("Insufficient permissions to view users", 403));
			}

			var result = await userService.GetUsersPagedAsync(filter);
            return Ok(SuccessData(result));
		}
		catch (Exception ex)
		{
			return Ok(ErrorMessage(ex.Message, 400));
		}
	}

	/// <summary>
	/// Retrieves user statistics (Admin only)
	/// </summary>
	/// <returns>User statistics</returns>
	/// <response code="200">Statistics retrieved successfully</response>
	/// <response code="403">Insufficient permissions</response>
	[HttpGet("statistics")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<UserStatsDto>> GetUserStatistics()
	{
		try
		{
			var currentUserId = GetCurrentUserId();
			if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "read"))
			{
				return Ok(ErrorMessage("Insufficient permissions to view user statistics", 403));
			}

			var stats = await userService.GetUserStatsAsync();
			return Ok(SuccessData(stats));
		}
		catch (Exception ex)
		{
			return Ok(ErrorMessage(ex.Message, 400));
		}
	}

	/// <summary>
	/// Retrieves a user by ID (Admin only)
	/// </summary>
	/// <param name="id">User identifier</param>
	/// <returns>User information</returns>
	/// <response code="200">User retrieved successfully</response>
	/// <response code="404">User not found</response>
	/// <response code="403">Insufficient permissions</response>
	[HttpGet("{id}")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<UserDto>> GetUserById(Guid id)
	{
		try
		{
			var currentUserId = GetCurrentUserId();
			if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "read"))
			{
				return Ok(ErrorMessage("Insufficient permissions to view users", 403));
			}

			var user = await userService.GetUserByIdAsync(id);
			if (user == null)
			{
				return Ok(ErrorMessage("User not found", 404));
			}

			return Ok(SuccessData(user));
		}
		catch (Exception ex)
		{
			return Ok(ErrorMessage(ex.Message, 400));
		}
	}

	/// <summary>
	/// Creates a new user (Admin only)
	/// </summary>
	/// <param name="createDto">User creation data</param>
	/// <returns>Created user information</returns>
	/// <response code="201">User created successfully</response>
	/// <response code="400">Invalid request data</response>
	/// <response code="403">Insufficient permissions</response>
	/// <response code="409">Email already exists</response>
	[HttpPost]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserCreateDto createDto)
	{
		try
		{
			var currentUserId = GetCurrentUserId();
			if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "create"))
			{
				return Ok(ErrorMessage("Insufficient permissions to create users", 403));
			}

			var result = await userService.CreateUserAsync(createDto);
			return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
		}
		catch (Exception ex)
		{
			return Ok(ErrorMessage(ex.Message, 400));
		}
	}

	/// <summary>
	/// Updates current user's profile
	/// </summary>
	/// <param name="request">Profile update data</param>
	/// <returns>Update confirmation</returns>
	/// <response code="200">Profile updated successfully</response>
	/// <response code="400">Invalid request data</response>
	[HttpPut("profile")]
	public async Task<ActionResult<UpdateUserResponseDto>> UpdateProfile([FromBody] UpdateUserRequestDto request)
	{
		try
		{
			var userId = GetCurrentUserId();
			var result = await userService.UpdateUserAsync(userId, request);
			return Ok(SuccessData(result));
		}
		catch (Exception ex)
		{
			return Ok(ErrorMessage(ex.Message, 400));
		}
	}

	/// <summary>
	/// Updates a user (Admin only)
	/// </summary>
	/// <param name="updateDto">User update data</param>
	/// <returns>Updated user information</returns>
	/// <response code="200">User updated successfully</response>
	/// <response code="400">Invalid request data</response>
	/// <response code="404">User not found</response>
	/// <response code="403">Insufficient permissions</response>
	[HttpPut("update")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<UserDto>> UpdateUser([FromBody] UserAdminUpdateDto updateDto)
	{
		try
		{
			var currentUserId = GetCurrentUserId();
			if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "update"))
			{
				return Ok(ErrorMessage("Insufficient permissions to update users", 403));
			}

			var result = await userService.UpdateUserAdminAsync(updateDto.Id, updateDto);
			return Ok(SuccessData(result));
		}
		catch (Exception ex)
		{
			return Ok(ErrorMessage(ex.Message, 400));
		}
	}

	/// <summary>
	/// Deletes a user (Admin only)
	/// </summary>
	/// <param name="id">User identifier</param>
	/// <returns>Deletion confirmation</returns>
	/// <response code="200">User deleted successfully</response>
	/// <response code="404">User not found</response>
	/// <response code="403">Insufficient permissions</response>
	/// <response code="400">User cannot be deleted</response>
	[HttpDelete("{id}")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult> DeleteUser(Guid id)
	{
		try
		{
			var currentUserId = GetCurrentUserId();
			if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "delete"))
			{
				return Ok(ErrorMessage("Insufficient permissions to delete users", 403));
			}

			// Prevent self-deletion
			if (id == currentUserId)
			{
				return Ok(ErrorMessage("Cannot delete your own account", 400));
			}

			await userService.DeleteUserAsync(id);
			return Ok(new { message = "User deleted successfully" });
		}
		catch (Exception ex)
		{
			return Ok(ErrorMessage(ex.Message, 400));
		}
	}

	/// <summary>
	/// Changes current user's password
	/// </summary>
	/// <param name="request">Password change data</param>
	/// <returns>Change confirmation</returns>
	/// <response code="200">Password changed successfully</response>
	/// <response code="400">Invalid request data or current password incorrect</response>
	[HttpPost("change-password")]
	public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
	{
		try
		{
			var userId = GetCurrentUserId();
			var result = await userService.ChangePasswordAsync(userId, request);

			if (result)
			{
				return Ok(SuccessMessage("Password changed successfully"));
			}

			return Ok(ErrorMessage("Failed to change password", 400));
		}
		catch (Exception ex)
		{
			return Ok(ErrorMessage(ex.Message, 400));
		}
	}

	/// <summary>
	/// Deactivates a user (Admin only)
	/// </summary>
	/// <param name="id">User identifier</param>
	/// <returns>Deactivation confirmation</returns>
	/// <response code="200">User deactivated successfully</response>
	/// <response code="404">User not found</response>
	/// <response code="403">Insufficient permissions</response>
	/// <response code="400">Cannot deactivate own account</response>
	[HttpPost("{id}/deactivate")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult> DeactivateUser(Guid id)
	{
		try
		{
			var currentUserId = GetCurrentUserId();
			if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "update"))
			{
				return Ok(ErrorMessage("Insufficient permissions to deactivate users", 403));
			}

			// Prevent self-deactivation
			if (id == currentUserId)
			{
				return Ok(ErrorMessage("Cannot deactivate your own account", 400));
			}

			var result = await userService.DeactivateUserAsync(id);

			if (result)
			{
				return Ok(SuccessMessage("User deactivated successfully"));
			}

			return Ok(ErrorMessage("Failed to deactivate user", 400));
		}
		catch (Exception ex)
		{
			return Ok(ErrorMessage(ex.Message, 400));
		}
	}

	/// <summary>
	/// Activates a user (Admin only)
	/// </summary>
	/// <param name="id">User identifier</param>
	/// <returns>Activation confirmation</returns>
	/// <response code="200">User activated successfully</response>
	/// <response code="404">User not found</response>
	/// <response code="403">Insufficient permissions</response>
	[HttpPost("{id}/activate")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult> ActivateUser(Guid id)
	{
		try
		{
			var currentUserId = GetCurrentUserId();
			if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "update"))
			{
				return Ok(ErrorMessage("Insufficient permissions to activate users", 403));
			}

			var result = await userService.ActivateUserAsync(id);
			if (result)
			{
				return Ok(SuccessMessage("User activated successfully"));
			}

			return Ok(ErrorMessage("Failed to activate user", 400));
		}
		catch (Exception ex)
		{
			return Ok(ErrorMessage(ex.Message, 400));
		}
	}

	/// <summary>
	/// Assigns roles to a user (Admin only)
	/// </summary>
	/// <param name="id">User identifier</param>
	/// <param name="roleIds">Collection of role identifiers</param>
	/// <returns>Assignment confirmation</returns>
	/// <response code="200">Roles assigned successfully</response>
	/// <response code="400">Invalid request data</response>
	/// <response code="404">User or role not found</response>
	/// <response code="403">Insufficient permissions</response>
	[HttpPost("{id}/roles")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult> AssignRolesToUser(Guid id, [FromBody] IEnumerable<Guid> roleIds)
	{
		try
		{
			var currentUserId = GetCurrentUserId();
			if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "assign"))
			{
				return Ok(ErrorMessage("Insufficient permissions to assign roles", 403));
			}

			var assignmentDto = new RoleAssignmentDto
			{
				UserId = id,
				RoleIds = [.. roleIds]
			};
			await roleService.AssignRolesToUserAsync(assignmentDto);
			return Ok(SuccessMessage("Roles assigned successfully"));
		}
		catch (Exception ex)
		{
			return Ok(ErrorMessage(ex.Message, 400));
		}
	}

	/// <summary>
	/// Removes roles from a user (Admin only)
	/// </summary>
	/// <param name="id">User identifier</param>
	/// <param name="roleIds">Collection of role identifiers</param>
	/// <returns>Removal confirmation</returns>
	/// <response code="200">Roles removed successfully</response>
	/// <response code="400">Invalid request data</response>
	/// <response code="404">User or role not found</response>
	/// <response code="403">Insufficient permissions</response>
	[HttpDelete("{id}/roles")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult> RemoveRolesFromUser(Guid id, [FromBody] IEnumerable<Guid> roleIds)
	{
		try
		{
			var currentUserId = GetCurrentUserId();
			if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "assign"))
			{
				return Ok(ErrorMessage("Insufficient permissions to remove roles", 403));
			}

			await roleService.RemoveRolesFromUserAsync(id, roleIds);
			return Ok(SuccessMessage("Roles removed successfully"));
		}
		catch (Exception ex)
		{
			return Ok(ErrorMessage(ex.Message, 400));
		}
	}

	/// <summary>
	/// Retrieves roles assigned to a user (Admin only)
	/// </summary>
	/// <param name="id">User identifier</param>
	/// <returns>List of user roles</returns>
	/// <response code="200">User roles retrieved successfully</response>
	/// <response code="404">User not found</response>
	/// <response code="403">Insufficient permissions</response>
	[HttpGet("{id}/roles")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult<IEnumerable<RoleDto>>> GetUserRoles(Guid id)
	{
		try
		{
			var currentUserId = GetCurrentUserId();
			if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "read"))
			{
				return Ok(ErrorMessage("Insufficient permissions to view user roles", 403));
			}

			var roles = await roleService.GetRolesByUserIdAsync(id);
			return Ok(SuccessData(roles));
		}
		catch (Exception ex)
		{
			return Ok(ErrorMessage(ex.Message, 400));
		}
	}

	/// <summary>
	/// Performs bulk operations on users (Admin only)
	/// </summary>
	/// <param name="bulkDto">Bulk operation data</param>
	/// <returns>Operation confirmation</returns>
	/// <response code="200">Bulk operation completed successfully</response>
	/// <response code="400">Invalid request data</response>
	/// <response code="403">Insufficient permissions</response>
	[HttpPost("bulk")]
	[Authorize(Roles = "Admin")]
	public async Task<ActionResult> BulkUserOperation([FromBody] BulkUserDto bulkDto)
	{
		try
		{
			var currentUserId = GetCurrentUserId();
			if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "update"))
			{
				return Ok(ErrorMessage("Insufficient permissions to perform bulk operations", 403));
			}

			// Remove current user from bulk operations to prevent self-modification
			bulkDto.UserIds = [.. bulkDto.UserIds.Where(id => id != currentUserId)];
			await userService.BulkUserOperationAsync(bulkDto);
			return Ok(SuccessMessage($"Bulk {bulkDto.Operation} operation completed successfully"));
		}
		catch (Exception ex)
		{
			return Ok(ErrorMessage(ex.Message, 400));
		}
	}

	/// <summary>
	/// Gets an avatar image by filename
	/// </summary>
	/// <param name="filename">The avatar filename</param>
	/// <returns>Avatar image file</returns>
	/// <response code="200">Avatar image retrieved successfully</response>
	/// <response code="404">Avatar not found</response>
	[HttpGet("avatar/{filename}")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(FileResult), 200)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> GetAvatar(string filename)
	{
		try
		{
			var stream = await userService.GetAvatarAsync(filename);
			var contentType = GetContentType(filename);
			
			return File(stream, contentType);
		}
		catch (FileNotFoundException)
		{
            return Ok(ErrorMessage("Avatar not found", 400));
		}
		catch (Exception ex)
		{
            return Ok(ErrorMessage(ex.Message, 400));
        }
	}

    /// <summary>
    /// Uploads an avatar image for the current user
    /// </summary>
    /// <param name="request">Avatar upload request containing the image file</param>
    /// <returns>Avatar upload result</returns>
    /// <response code="200">Avatar uploaded successfully</response>
    /// <response code="400">Invalid file or validation error</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found</response>
    [HttpPost("avatar")]
	[ProducesResponseType(typeof(object), 200)]
	[ProducesResponseType(typeof(object), 400)]
	[ProducesResponseType(401)]
	[ProducesResponseType(404)]
	[Consumes("multipart/form-data")]
	public async Task<IActionResult> UploadAvatar([FromForm] AvatarUploadDto request)
	{
		try
		{
			var userId = GetCurrentUserId();
			var avatarFilename = await userService.UploadAvatarAsync(userId, request.AvatarFile);

			// Create full URL that client can use to access the image
			var avatarUrl = CreateAvatarUrl(avatarFilename);

			return Ok(new
			{
				success = true,
				message = "Avatar uploaded successfully",
				data = new { avatarUrl, filename = avatarFilename }
			});
		}
		catch (Exception ex)
		{
            return Ok(ErrorMessage(ex.Message, 400));
        }
	}

	/// <summary>
	/// Deletes the current user's avatar
	/// </summary>
	/// <returns>Avatar deletion result</returns>
	/// <response code="200">Avatar deleted successfully</response>
	/// <response code="401">Unauthorized</response>
	/// <response code="404">User not found</response>
	[HttpDelete("avatar")]
	[ProducesResponseType(typeof(object), 200)]
	[ProducesResponseType(401)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> DeleteAvatar()
	{
		try
		{
			var userId = GetCurrentUserId();
			var deleted = await userService.DeleteAvatarAsync(userId);

			if (deleted)
			{
				return Ok(SuccessMessage("Avatar deleted successfully"));
            }

			return Ok(ErrorMessage("No avatar to delete", 400));
        }
		catch (Exception ex)
		{
            return Ok(ErrorMessage(ex.Message, 400));
        }
	}

	private Guid GetCurrentUserId()
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
		{
			throw new UnauthorizedAccessException("Invalid user ID in token");
		}

		return userId;
	}

	private static string GetContentType(string filename)
	{
		var extension = Path.GetExtension(filename).ToLowerInvariant();
		return extension switch
		{
			".jpg" or ".jpeg" => "image/jpeg",
			".png" => "image/png",
			".gif" => "image/gif",
			".webp" => "image/webp",
			_ => "application/octet-stream"
		};
	}

	private string CreateAvatarUrl(string filename)
	{
		var baseUrl = _configuration["BaseUrl"] ?? "https://localhost:8080";
		return $"{baseUrl}/api/user/avatar/{filename}";
	}
}
