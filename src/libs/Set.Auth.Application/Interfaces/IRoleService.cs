using Set.Auth.Application.DTOs.Role;

namespace Set.Auth.Application.Interfaces;

/// <summary>
/// Service interface for role management operations
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Retrieves a role by its unique identifier
    /// </summary>
    /// <param name="id">The role identifier</param>
    /// <returns>The role DTO if found; otherwise, null</returns>
    Task<RoleDto?> GetRoleByIdAsync(Guid id);
    
    /// <summary>
    /// Retrieves a role by its name
    /// </summary>
    /// <param name="name">The role name</param>
    /// <returns>The role DTO if found; otherwise, null</returns>
    Task<RoleDto?> GetRoleByNameAsync(string name);
    
    /// <summary>
    /// Retrieves all roles
    /// </summary>
    /// <returns>A collection of role DTOs</returns>
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    
    /// <summary>
    /// Retrieves roles with pagination and filtering
    /// </summary>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="searchTerm">Optional search term for filtering</param>
    /// <param name="isActive">Optional active status filter</param>
    /// <param name="sortBy">Sort field</param>
    /// <param name="sortDirection">Sort direction</param>
    /// <returns>A paginated list of role DTOs</returns>
    Task<RoleListDto> GetRolesPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null,
        bool? isActive = null,
        string sortBy = "Name",
        string sortDirection = "asc");
    
    /// <summary>
    /// Retrieves all roles assigned to a specific user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>A collection of role DTOs assigned to the user</returns>
    Task<IEnumerable<RoleDto>> GetRolesByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Creates a new role
    /// </summary>
    /// <param name="createDto">The role creation data</param>
    /// <returns>The created role DTO</returns>
    Task<RoleDto> CreateRoleAsync(RoleCreateDto createDto);
    
    /// <summary>
    /// Updates an existing role
    /// </summary>
    /// <param name="id">The role identifier</param>
    /// <param name="updateDto">The role update data</param>
    /// <returns>The updated role DTO</returns>
    Task<RoleDto> UpdateRoleAsync(Guid id, RoleUpdateDto updateDto);
    
    /// <summary>
    /// Deletes a role by its identifier
    /// </summary>
    /// <param name="id">The role identifier</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task DeleteRoleAsync(Guid id);
    
    /// <summary>
    /// Activates a role
    /// </summary>
    /// <param name="id">The role identifier</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task ActivateRoleAsync(Guid id);
    
    /// <summary>
    /// Deactivates a role
    /// </summary>
    /// <param name="id">The role identifier</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task DeactivateRoleAsync(Guid id);
    
    /// <summary>
    /// Assigns roles to a user
    /// </summary>
    /// <param name="assignmentDto">The role assignment data</param>
    /// <returns>The role assignment response</returns>
    Task<RoleAssignmentResponseDto> AssignRolesToUserAsync(RoleAssignmentDto assignmentDto);
    
    /// <summary>
    /// Removes roles from a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="roleIds">The collection of role identifiers to remove</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task RemoveRolesFromUserAsync(Guid userId, IEnumerable<Guid> roleIds);
    
    /// <summary>
    /// Assigns permissions to a role
    /// </summary>
    /// <param name="roleId">The role identifier</param>
    /// <param name="permissionIds">The collection of permission identifiers</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task AssignPermissionsToRoleAsync(Guid roleId, IEnumerable<Guid> permissionIds);
    
    /// <summary>
    /// Removes permissions from a role
    /// </summary>
    /// <param name="roleId">The role identifier</param>
    /// <param name="permissionIds">The collection of permission identifiers to remove</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task RemovePermissionsFromRoleAsync(Guid roleId, IEnumerable<Guid> permissionIds);
    
    /// <summary>
    /// Checks if a role exists by its identifier
    /// </summary>
    /// <param name="id">The role identifier</param>
    /// <returns>True if the role exists; otherwise, false</returns>
    Task<bool> RoleExistsAsync(Guid id);
    
    /// <summary>
    /// Checks if a role name is already in use
    /// </summary>
    /// <param name="name">The role name to check</param>
    /// <param name="excludeId">Optional role ID to exclude from the check</param>
    /// <returns>True if the role name exists; otherwise, false</returns>
    Task<bool> RoleNameExistsAsync(string name, Guid? excludeId = null);
    
    /// <summary>
    /// Validates if the user has permission to perform role operations
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="operation">The operation being performed</param>
    /// <returns>True if the user has permission; otherwise, false</returns>
    Task<bool> CanPerformRoleOperationAsync(Guid userId, string operation);
}
