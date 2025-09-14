using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Set.Auth.Api.Controllers.Base;
using Set.Auth.Application.DTOs.Role;
using Set.Auth.Application.Interfaces;
using System.Security.Claims;

namespace Set.Auth.Api.Controllers;

/// <summary>
/// Controller for role management operations
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RoleController"/> class
/// </remarks>
/// <param name="roleService">The role service</param>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoleController(IRoleService roleService) : BaseController
{
    /// <summary>
    /// Retrieves all roles with pagination and filtering
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term for filtering</param>
    /// <param name="isActive">Active status filter</param>
    /// <param name="sortBy">Sort field</param>
    /// <param name="sortDirection">Sort direction</param>
    /// <returns>Paginated list of roles</returns>
    /// <response code="200">Roles retrieved successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoleListDto>> GetRoles(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string sortBy = "Name",
        [FromQuery] string sortDirection = "asc")
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await roleService.CanPerformRoleOperationAsync(currentUserId, "read"))
            {
                return Ok(ErrorMessage("Insufficient permissions to view roles", 403));
            }

            var result = await roleService.GetRolesPagedAsync(pageNumber, pageSize, searchTerm, isActive, sortBy, sortDirection);
            return Ok(SuccessData(result));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Retrieves all roles (simple list)
    /// </summary>
    /// <returns>List of all roles</returns>
    /// <response code="200">Roles retrieved successfully</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await roleService.CanPerformRoleOperationAsync(currentUserId, "read"))
            {
                return Ok(ErrorMessage("Insufficient permissions to view roles", 403));
            }

            var result = await roleService.GetAllRolesAsync();
            return Ok(SuccessData(result));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Retrieves a role by its identifier
    /// </summary>
    /// <param name="id">Role identifier</param>
    /// <returns>Role information</returns>
    /// <response code="200">Role retrieved successfully</response>
    /// <response code="404">Role not found</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoleDto>> GetRoleById(Guid id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await roleService.CanPerformRoleOperationAsync(currentUserId, "read"))
            {
                return Ok(ErrorMessage("Insufficient permissions to view roles", 403));
            }

            var role = await roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return Ok(ErrorMessage("Role not found", 404));
            }

            return Ok(SuccessData(role));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Retrieves roles assigned to a specific user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>List of roles assigned to the user</returns>
    /// <response code="200">User roles retrieved successfully</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetUserRoles(Guid userId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await roleService.CanPerformRoleOperationAsync(currentUserId, "read"))
            {
                return Ok(ErrorMessage("Insufficient permissions to view user roles", 403));
            }

            var roles = await roleService.GetRolesByUserIdAsync(userId);
            return Ok(SuccessData(roles));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Creates a new role
    /// </summary>
    /// <param name="createDto">Role creation data</param>
    /// <returns>Created role information</returns>
    /// <response code="201">Role created successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="409">Role name already exists</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] RoleCreateDto createDto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await roleService.CanPerformRoleOperationAsync(currentUserId, "create"))
            {
                return Ok(ErrorMessage("Insufficient permissions to create roles", 403));
            }

            var result = await roleService.CreateRoleAsync(createDto);
            return Ok(SuccessData(result));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Updates an existing role
    /// </summary>
    /// <param name="id">Role identifier</param>
    /// <param name="updateDto">Role update data</param>
    /// <returns>Updated role information</returns>
    /// <response code="200">Role updated successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="404">Role not found</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoleDto>> UpdateRole(Guid id, [FromBody] RoleUpdateDto updateDto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await roleService.CanPerformRoleOperationAsync(currentUserId, "update"))
            {
                return Ok(ErrorMessage("Insufficient permissions to update roles", 403));
            }

            var result = await roleService.UpdateRoleAsync(id, updateDto);
            return Ok(SuccessData(result));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Deletes a role
    /// </summary>
    /// <param name="id">Role identifier</param>
    /// <returns>Deletion confirmation</returns>
    /// <response code="200">Role deleted successfully</response>
    /// <response code="404">Role not found</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="400">Role cannot be deleted (assigned to users)</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteRole(Guid id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await roleService.CanPerformRoleOperationAsync(currentUserId, "delete"))
            {
                return Ok(ErrorMessage("Insufficient permissions to delete roles", 403));
            }

            await roleService.DeleteRoleAsync(id);
            return Ok(SuccessMessage("Role deleted successfully"));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Activates a role
    /// </summary>
    /// <param name="id">Role identifier</param>
    /// <returns>Activation confirmation</returns>
    /// <response code="200">Role activated successfully</response>
    /// <response code="404">Role not found</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ActivateRole(Guid id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await roleService.CanPerformRoleOperationAsync(currentUserId, "update"))
            {
                return Ok(ErrorMessage("Insufficient permissions to activate roles", 403));
            }

            await roleService.ActivateRoleAsync(id);
            return Ok(SuccessMessage("Role activated successfully"));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Deactivates a role
    /// </summary>
    /// <param name="id">Role identifier</param>
    /// <returns>Deactivation confirmation</returns>
    /// <response code="200">Role deactivated successfully</response>
    /// <response code="404">Role not found</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeactivateRole(Guid id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await roleService.CanPerformRoleOperationAsync(currentUserId, "update"))
            {
                return Ok(ErrorMessage("Insufficient permissions to deactivate roles", 403));
            }

            await roleService.DeactivateRoleAsync(id);
            return Ok(SuccessMessage("Role deactivated successfully"));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Assigns roles to a user
    /// </summary>
    /// <param name="assignmentDto">Role assignment data</param>
    /// <returns>Role assignment response</returns>
    /// <response code="200">Roles assigned successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="404">User or role not found</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpPost("assign")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoleAssignmentResponseDto>> AssignRolesToUser([FromBody] RoleAssignmentDto assignmentDto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await roleService.CanPerformRoleOperationAsync(currentUserId, "assign"))
            {
                return Ok(ErrorMessage("Insufficient permissions to assign roles", 403));
            }

            var result = await roleService.AssignRolesToUserAsync(assignmentDto);
            return Ok(SuccessData(result));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Removes roles from a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="roleIds">Collection of role identifiers to remove</param>
    /// <returns>Removal confirmation</returns>
    /// <response code="200">Roles removed successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="404">User not found</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpDelete("user/{userId}/roles")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RemoveRolesFromUser(Guid userId, [FromBody] IEnumerable<Guid> roleIds)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await roleService.CanPerformRoleOperationAsync(currentUserId, "assign"))
            {
                return Ok(ErrorMessage("Insufficient permissions to remove roles", 403));
            }

            await roleService.RemoveRolesFromUserAsync(userId, roleIds);
            return Ok(SuccessMessage("Roles removed from user successfully"));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Assigns permissions to a role
    /// </summary>
    /// <param name="roleId">Role identifier</param>
    /// <param name="permissionIds">Collection of permission identifiers</param>
    /// <returns>Assignment confirmation</returns>
    /// <response code="200">Permissions assigned successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="404">Role or permission not found</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpPost("{roleId}/permissions")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> AssignPermissionsToRole(Guid roleId, [FromBody] IEnumerable<Guid> permissionIds)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await roleService.CanPerformRoleOperationAsync(currentUserId, "assign"))
            {
                return Ok(ErrorMessage("Insufficient permissions to assign permissions to roles", 403));
            }

            await roleService.AssignPermissionsToRoleAsync(roleId, permissionIds);
            return Ok(SuccessMessage("Permissions assigned to role successfully"));
        }
        catch (Exception ex)
        {
            return Ok(ErrorMessage(ex.Message, 400));
        }
    }

    /// <summary>
    /// Removes permissions from a role
    /// </summary>
    /// <param name="roleId">Role identifier</param>
    /// <param name="permissionIds">Collection of permission identifiers to remove</param>
    /// <returns>Removal confirmation</returns>
    /// <response code="200">Permissions removed successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="404">Role not found</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpDelete("{roleId}/permissions")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RemovePermissionsFromRole(Guid roleId, [FromBody] IEnumerable<Guid> permissionIds)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (!await roleService.CanPerformRoleOperationAsync(currentUserId, "assign"))
            {
                return Ok(ErrorMessage("Insufficient permissions to remove permissions from roles", 403));
            }

            await roleService.RemovePermissionsFromRoleAsync(roleId, permissionIds);
            return Ok(SuccessMessage("Permissions removed from role successfully"));
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
}
