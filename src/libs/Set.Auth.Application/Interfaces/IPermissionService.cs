using Set.Auth.Application.DTOs.Permission;

namespace Set.Auth.Application.Interfaces;

/// <summary>
/// Service interface for permission management operations
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Retrieves a permission by its unique identifier
    /// </summary>
    /// <param name="id">The permission identifier</param>
    /// <returns>The permission DTO if found; otherwise, null</returns>
    Task<PermissionDto?> GetPermissionByIdAsync(Guid id);
    
    /// <summary>
    /// Retrieves a permission by its name
    /// </summary>
    /// <param name="name">The permission name</param>
    /// <returns>The permission DTO if found; otherwise, null</returns>
    Task<PermissionDto?> GetPermissionByNameAsync(string name);
    
    /// <summary>
    /// Retrieves a permission by resource and action
    /// </summary>
    /// <param name="resource">The resource name</param>
    /// <param name="action">The action name</param>
    /// <returns>The permission DTO if found; otherwise, null</returns>
    Task<PermissionDto?> GetPermissionByResourceAndActionAsync(string resource, string action);
    
    /// <summary>
    /// Retrieves all permissions
    /// </summary>
    /// <returns>A collection of permission DTOs</returns>
    Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync();
    
    /// <summary>
    /// Retrieves permissions with pagination and filtering
    /// </summary>
    /// <param name="filter">The filter criteria</param>
    /// <returns>A paginated list of permission DTOs</returns>
    Task<PermissionListDto> GetPermissionsPagedAsync(PermissionFilterDto filter);
    
    /// <summary>
    /// Retrieves all permissions assigned to a specific role
    /// </summary>
    /// <param name="roleId">The role identifier</param>
    /// <returns>A collection of permission DTOs assigned to the role</returns>
    Task<IEnumerable<PermissionDto>> GetPermissionsByRoleIdAsync(Guid roleId);
    
    /// <summary>
    /// Retrieves all permissions for a specific user through their roles
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>A collection of permission DTOs available to the user</returns>
    Task<IEnumerable<PermissionDto>> GetPermissionsByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Retrieves all permissions grouped by resource
    /// </summary>
    /// <returns>A dictionary with resource names as keys and permissions as values</returns>
    Task<Dictionary<string, IEnumerable<PermissionDto>>> GetPermissionsGroupedByResourceAsync();
    
    /// <summary>
    /// Creates a new permission
    /// </summary>
    /// <param name="createDto">The permission creation data</param>
    /// <returns>The created permission DTO</returns>
    Task<PermissionDto> CreatePermissionAsync(PermissionCreateDto createDto);
    
    /// <summary>
    /// Updates an existing permission
    /// </summary>
    /// <param name="id">The permission identifier</param>
    /// <param name="updateDto">The permission update data</param>
    /// <returns>The updated permission DTO</returns>
    Task<PermissionDto> UpdatePermissionAsync(Guid id, PermissionUpdateDto updateDto);
    
    /// <summary>
    /// Deletes a permission by its identifier
    /// </summary>
    /// <param name="id">The permission identifier</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task DeletePermissionAsync(Guid id);
    
    /// <summary>
    /// Deletes multiple permissions
    /// </summary>
    /// <param name="ids">The collection of permission identifiers</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task DeletePermissionsAsync(IEnumerable<Guid> ids);
    
    /// <summary>
    /// Activates a permission
    /// </summary>
    /// <param name="id">The permission identifier</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task ActivatePermissionAsync(Guid id);
    
    /// <summary>
    /// Deactivates a permission
    /// </summary>
    /// <param name="id">The permission identifier</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task DeactivatePermissionAsync(Guid id);
    
    /// <summary>
    /// Performs bulk operations on permissions
    /// </summary>
    /// <param name="bulkDto">The bulk operation data</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task BulkPermissionOperationAsync(BulkPermissionDto bulkDto);
    
    /// <summary>
    /// Assigns permissions to a role
    /// </summary>
    /// <param name="assignmentDto">The permission assignment data</param>
    /// <returns>The permission assignment response</returns>
    Task<PermissionAssignmentResponseDto> AssignPermissionsToRoleAsync(PermissionAssignmentDto assignmentDto);
    
    /// <summary>
    /// Checks if a permission exists by its identifier
    /// </summary>
    /// <param name="id">The permission identifier</param>
    /// <returns>True if the permission exists; otherwise, false</returns>
    Task<bool> PermissionExistsAsync(Guid id);
    
    /// <summary>
    /// Checks if a permission name is already in use
    /// </summary>
    /// <param name="name">The permission name to check</param>
    /// <param name="excludeId">Optional permission ID to exclude from the check</param>
    /// <returns>True if the permission name exists; otherwise, false</returns>
    Task<bool> PermissionNameExistsAsync(string name, Guid? excludeId = null);
    
    /// <summary>
    /// Checks if a permission with the same resource and action already exists
    /// </summary>
    /// <param name="resource">The resource name</param>
    /// <param name="action">The action name</param>
    /// <param name="excludeId">Optional permission ID to exclude from the check</param>
    /// <returns>True if the combination exists; otherwise, false</returns>
    Task<bool> ResourceActionExistsAsync(string resource, string action, Guid? excludeId = null);
    
    /// <summary>
    /// Validates if the user has permission to perform permission operations
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="operation">The operation being performed</param>
    /// <returns>True if the user has permission; otherwise, false</returns>
    Task<bool> CanPerformPermissionOperationAsync(Guid userId, string operation);
    
    /// <summary>
    /// Checks if a user has a specific permission
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="resource">The resource name</param>
    /// <param name="action">The action name</param>
    /// <returns>True if the user has the permission; otherwise, false</returns>
    Task<bool> UserHasPermissionAsync(Guid userId, string resource, string action);
    
    /// <summary>
    /// Checks if a user has any of the specified permissions
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="permissions">The collection of permission names</param>
    /// <returns>True if the user has any of the permissions; otherwise, false</returns>
    Task<bool> UserHasAnyPermissionAsync(Guid userId, IEnumerable<string> permissions);
    
    /// <summary>
    /// Checks if a user has all of the specified permissions
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="permissions">The collection of permission names</param>
    /// <returns>True if the user has all of the permissions; otherwise, false</returns>
    Task<bool> UserHasAllPermissionsAsync(Guid userId, IEnumerable<string> permissions);
}
