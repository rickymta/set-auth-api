using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Set.Auth.Api.Controllers.Base;
using Set.Auth.Application.DTOs.Permission;
using Set.Auth.Application.Interfaces;
using Set.Auth.Domain.Entities;
using System.Security.Claims;

namespace Set.Auth.Api.Controllers;

/// <summary>
/// Controller for permission management operations
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PermissionController"/> class
/// </remarks>
/// <param name="permissionService">The permission service</param>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionController(IPermissionService permissionService) : BaseController
{
    /// <summary>
    /// Retrieves all permissions with pagination and filtering
    /// </summary>
    /// <param name="filter">Filter criteria</param>
    /// <returns>Paginated list of permissions</returns>
    /// <response code="200">Permissions retrieved successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PermissionListDto>> GetPermissions([FromQuery] PermissionFilterDto filter)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "read"))
            {
                return Ok(ErrorMessage("Insufficient permissions to view permissions"));
            }

            var result = await permissionService.GetPermissionsPagedAsync(filter);
            return Ok(SuccessData(result));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Retrieves all permissions (simple list)
    /// </summary>
    /// <returns>List of all permissions</returns>
    /// <response code="200">Permissions retrieved successfully</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetAllPermissions()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "read"))
            {
                return Ok(ErrorMessage("Insufficient permissions to view permissions"));
            }

            var result = await permissionService.GetAllPermissionsAsync();
            return Ok(SuccessData(result));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Retrieves permissions grouped by resource
    /// </summary>
    /// <returns>Permissions grouped by resource</returns>
    /// <response code="200">Permissions retrieved successfully</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpGet("grouped")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Dictionary<string, IEnumerable<PermissionDto>>>> GetPermissionsGrouped()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "read"))
            {
                return Ok(ErrorMessage("Insufficient permissions to view permissions"));
            }

            var result = await permissionService.GetPermissionsGroupedByResourceAsync();
            return Ok(SuccessData(result));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Retrieves a permission by its identifier
    /// </summary>
    /// <param name="id">Permission identifier</param>
    /// <returns>Permission information</returns>
    /// <response code="200">Permission retrieved successfully</response>
    /// <response code="404">Permission not found</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PermissionDto>> GetPermissionById(Guid id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "read"))
            {
                return Ok(ErrorMessage("Insufficient permissions to view permissions"));
            }

            var permission = await permissionService.GetPermissionByIdAsync(id);
            if (permission == null)
            {
                return Ok(ErrorMessage("Permission not found"));
            }

            return Ok(SuccessData(permission));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Retrieves permissions assigned to a specific role
    /// </summary>
    /// <param name="roleId">Role identifier</param>
    /// <returns>List of permissions assigned to the role</returns>
    /// <response code="200">Role permissions retrieved successfully</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpGet("role/{roleId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetRolePermissions(Guid roleId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "read"))
            {
                return Ok(ErrorMessage("Insufficient permissions to view role permissions"));
            }

            var permissions = await permissionService.GetPermissionsByRoleIdAsync(roleId);
            return Ok(SuccessData(permissions));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Retrieves permissions available to a specific user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>List of permissions available to the user</returns>
    /// <response code="200">User permissions retrieved successfully</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetUserPermissions(Guid userId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "read"))
            {
                return Ok(ErrorMessage("Insufficient permissions to view user permissions"));
            }

            var permissions = await permissionService.GetPermissionsByUserIdAsync(userId);
            return Ok(SuccessData(permissions));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Creates a new permission
    /// </summary>
    /// <param name="createDto">Permission creation data</param>
    /// <returns>Created permission information</returns>
    /// <response code="201">Permission created successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="409">Permission name or resource+action combination already exists</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PermissionDto>> CreatePermission([FromBody] PermissionCreateDto createDto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "create"))
            {
                return Ok(ErrorMessage("Insufficient permissions to create permissions"));
            }

            var result = await permissionService.CreatePermissionAsync(createDto);
            return Ok(SuccessData(result));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Updates an existing permission
    /// </summary>
    /// <param name="id">Permission identifier</param>
    /// <param name="updateDto">Permission update data</param>
    /// <returns>Updated permission information</returns>
    /// <response code="200">Permission updated successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="404">Permission not found</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PermissionDto>> UpdatePermission(Guid id, [FromBody] PermissionUpdateDto updateDto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "update"))
            {
                return Ok(ErrorMessage("Insufficient permissions to update permissions"));
            }

            var result = await permissionService.UpdatePermissionAsync(id, updateDto);
            return Ok(SuccessData(result));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Deletes a permission
    /// </summary>
    /// <param name="id">Permission identifier</param>
    /// <returns>Deletion confirmation</returns>
    /// <response code="200">Permission deleted successfully</response>
    /// <response code="404">Permission not found</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="400">Permission cannot be deleted (assigned to roles)</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeletePermission(Guid id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "delete"))
            {
                return Ok(ErrorMessage("Insufficient permissions to delete permissions"));
            }

            await permissionService.DeletePermissionAsync(id);
            return Ok(SuccessMessage("Permission deleted successfully"));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Activates a permission
    /// </summary>
    /// <param name="id">Permission identifier</param>
    /// <returns>Activation confirmation</returns>
    /// <response code="200">Permission activated successfully</response>
    /// <response code="404">Permission not found</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ActivatePermission(Guid id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "update"))
            {
                return Ok(ErrorMessage("Insufficient permissions to activate permissions"));
            }

            await permissionService.ActivatePermissionAsync(id);
            return Ok(SuccessMessage("Permission activated successfully"));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Deactivates a permission
    /// </summary>
    /// <param name="id">Permission identifier</param>
    /// <returns>Deactivation confirmation</returns>
    /// <response code="200">Permission deactivated successfully</response>
    /// <response code="404">Permission not found</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeactivatePermission(Guid id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "update"))
            {
                return Ok(ErrorMessage("Insufficient permissions to deactivate permissions"));
            }

            await permissionService.DeactivatePermissionAsync(id);
            return Ok(SuccessMessage("Permission deactivated successfully"));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Performs bulk operations on permissions
    /// </summary>
    /// <param name="bulkDto">Bulk operation data</param>
    /// <returns>Operation confirmation</returns>
    /// <response code="200">Bulk operation completed successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpPost("bulk")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> BulkPermissionOperation([FromBody] BulkPermissionDto bulkDto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "update"))
            {
                return Ok(ErrorMessage("Insufficient permissions to perform bulk operations"));
            }

            await permissionService.BulkPermissionOperationAsync(bulkDto);
            return Ok(SuccessMessage($"Bulk {bulkDto.Operation} operation completed successfully"));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Assigns permissions to a role
    /// </summary>
    /// <param name="assignmentDto">Permission assignment data</param>
    /// <returns>Permission assignment response</returns>
    /// <response code="200">Permissions assigned successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="404">Role or permission not found</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpPost("assign")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PermissionAssignmentResponseDto>> AssignPermissionsToRole([FromBody] PermissionAssignmentDto assignmentDto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "assign"))
            {
                return Ok(ErrorMessage("Insufficient permissions to assign permissions"));
            }

            var result = await permissionService.AssignPermissionsToRoleAsync(assignmentDto);
            return Ok(SuccessData(result));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Checks if a user has a specific permission
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="resource">Resource name</param>
    /// <param name="action">Action name</param>
    /// <returns>Permission check result</returns>
    /// <response code="200">Permission check completed</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpGet("check/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> CheckUserPermission(Guid userId, [FromQuery] string resource, [FromQuery] string action)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "read"))
            {
                return Ok(ErrorMessage("Insufficient permissions to check user permissions"));
            }

            var hasPermission = await permissionService.UserHasPermissionAsync(userId, resource, action);
            return Ok(SuccessData<object>(hasPermission));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Checks if a user has any of the specified permissions
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="permissions">Collection of permission names</param>
    /// <returns>Permission check result</returns>
    /// <response code="200">Permission check completed</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpPost("check/{userId}/any")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> CheckUserHasAnyPermission(Guid userId, [FromBody] IEnumerable<string> permissions)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "read"))
            {
                return Ok(ErrorMessage("Insufficient permissions to check user permissions"));
            }

            var hasAnyPermission = await permissionService.UserHasAnyPermissionAsync(userId, permissions);
            return Ok(SuccessData<object>(hasAnyPermission));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Checks if a user has all of the specified permissions
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="permissions">Collection of permission names</param>
    /// <returns>Permission check result</returns>
    /// <response code="200">Permission check completed</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpPost("check/{userId}/all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> CheckUserHasAllPermissions(Guid userId, [FromBody] IEnumerable<string> permissions)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await permissionService.CanPerformPermissionOperationAsync(currentUserId, "read"))
            {
                return Ok(ErrorMessage("Insufficient permissions to check user permissions"));
            }

            var hasAllPermissions = await permissionService.UserHasAllPermissionsAsync(userId, permissions);
            return Ok(SuccessData<object>(hasAllPermissions));
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
            throw new UnauthorizedAccessException("Invalid user ID in token");
        
        return userId;
    }
}
