using Set.Auth.Domain.Entities;

namespace Set.Auth.Domain.Interfaces;

/// <summary>
/// Repository interface for permission entity operations
/// </summary>
public interface IPermissionRepository
{
    /// <summary>
    /// Retrieves a permission by its unique identifier
    /// </summary>
    /// <param name="id">The permission identifier</param>
    /// <returns>The permission if found; otherwise, null</returns>
    Task<Permission?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Retrieves a permission by its name
    /// </summary>
    /// <param name="name">The permission name</param>
    /// <returns>The permission if found; otherwise, null</returns>
    Task<Permission?> GetByNameAsync(string name);
    
    /// <summary>
    /// Retrieves a permission by resource and action
    /// </summary>
    /// <param name="resource">The resource name</param>
    /// <param name="action">The action name</param>
    /// <returns>The permission if found; otherwise, null</returns>
    Task<Permission?> GetByResourceAndActionAsync(string resource, string action);
    
    /// <summary>
    /// Retrieves all permissions
    /// </summary>
    /// <returns>A collection of all permissions</returns>
    Task<IEnumerable<Permission>> GetAllAsync();
    
    /// <summary>
    /// Retrieves permissions with pagination and filtering
    /// </summary>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The page size</param>
    /// <param name="searchTerm">Optional search term for filtering</param>
    /// <param name="resource">Optional resource filter</param>
    /// <param name="action">Optional action filter</param>
    /// <param name="isActive">Optional active status filter</param>
    /// <param name="sortBy">Sort field</param>
    /// <param name="sortDirection">Sort direction</param>
    /// <returns>A tuple containing the permissions and total count</returns>
    Task<(IEnumerable<Permission> Permissions, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null,
        string? resource = null,
        string? action = null,
        bool? isActive = null,
        string sortBy = "Name",
        string sortDirection = "asc");
    
    /// <summary>
    /// Retrieves all permissions assigned to a specific role
    /// </summary>
    /// <param name="roleId">The role identifier</param>
    /// <returns>A collection of permissions assigned to the role</returns>
    Task<IEnumerable<Permission>> GetByRoleIdAsync(Guid roleId);
    
    /// <summary>
    /// Retrieves all permissions for a specific user through their roles
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>A collection of permissions available to the user</returns>
    Task<IEnumerable<Permission>> GetByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Retrieves all permissions grouped by resource
    /// </summary>
    /// <returns>A dictionary with resource names as keys and permissions as values</returns>
    Task<Dictionary<string, IEnumerable<Permission>>> GetGroupedByResourceAsync();
    
    /// <summary>
    /// Creates a new permission
    /// </summary>
    /// <param name="permission">The permission to create</param>
    /// <returns>The created permission</returns>
    Task<Permission> CreateAsync(Permission permission);
    
    /// <summary>
    /// Updates an existing permission
    /// </summary>
    /// <param name="permission">The permission to update</param>
    /// <returns>The updated permission</returns>
    Task<Permission> UpdateAsync(Permission permission);
    
    /// <summary>
    /// Deletes a permission by its identifier
    /// </summary>
    /// <param name="id">The permission identifier</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task DeleteAsync(Guid id);
    
    /// <summary>
    /// Deletes multiple permissions by their identifiers
    /// </summary>
    /// <param name="ids">The collection of permission identifiers</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task DeleteManyAsync(IEnumerable<Guid> ids);
    
    /// <summary>
    /// Checks if a permission exists by its identifier
    /// </summary>
    /// <param name="id">The permission identifier</param>
    /// <returns>True if the permission exists; otherwise, false</returns>
    Task<bool> ExistsAsync(Guid id);
    
    /// <summary>
    /// Checks if a permission name is already in use
    /// </summary>
    /// <param name="name">The permission name to check</param>
    /// <returns>True if the permission name exists; otherwise, false</returns>
    Task<bool> NameExistsAsync(string name);
    
    /// <summary>
    /// Checks if a permission with the same resource and action already exists
    /// </summary>
    /// <param name="resource">The resource name</param>
    /// <param name="action">The action name</param>
    /// <param name="excludeId">Optional permission ID to exclude from the check</param>
    /// <returns>True if the combination exists; otherwise, false</returns>
    Task<bool> ResourceActionExistsAsync(string resource, string action, Guid? excludeId = null);
    
    /// <summary>
    /// Gets the count of roles that have a specific permission
    /// </summary>
    /// <param name="permissionId">The permission identifier</param>
    /// <returns>The number of roles that have this permission</returns>
    Task<int> GetRoleCountAsync(Guid permissionId);
    
    /// <summary>
    /// Activates or deactivates multiple permissions
    /// </summary>
    /// <param name="ids">The collection of permission identifiers</param>
    /// <param name="isActive">The active status to set</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task UpdateActiveStatusAsync(IEnumerable<Guid> ids, bool isActive);
}
