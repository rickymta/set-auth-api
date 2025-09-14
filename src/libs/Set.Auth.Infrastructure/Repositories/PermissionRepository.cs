using Microsoft.EntityFrameworkCore;
using Set.Auth.Domain.Entities;
using Set.Auth.Domain.Interfaces;
using Set.Auth.Infrastructure.Data;

namespace Set.Auth.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Permission entity operations
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PermissionRepository"/> class
/// </remarks>
/// <param name="context">The database context</param>
public class PermissionRepository(AuthDbContext context) : IPermissionRepository
{
    /// <summary>
    /// Retrieves all permissions
    /// </summary>
    /// <returns>A collection of all permissions</returns>
    public async Task<IEnumerable<Permission>> GetAllAsync()
    {
        return await context.Permissions
            .Where(p => p.IsActive)
            .OrderBy(p => p.Resource)
            .ThenBy(p => p.Action)
            .ToListAsync();
    }

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
    public async Task<(IEnumerable<Permission> Permissions, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null,
        string? resource = null,
        string? action = null,
        bool? isActive = null,
        string sortBy = "Name",
        string sortDirection = "asc")
    {
        var query = context.Permissions.AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(resource))
        {
            query = query.Where(p => p.Resource.Contains(resource));
        }

        if (!string.IsNullOrEmpty(action))
        {
            query = query.Where(p => p.Action.Contains(action));
        }

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) || 
                                   (p.Description != null && p.Description.Contains(searchTerm)));
        }

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        // Apply sorting
        query = sortBy.ToLower() switch
        {
            "resource" => sortDirection.Equals("desc", StringComparison.CurrentCultureIgnoreCase)
                ? query.OrderByDescending(p => p.Resource)
                : query.OrderBy(p => p.Resource),
            "action" => sortDirection.Equals("desc", StringComparison.CurrentCultureIgnoreCase)
                ? query.OrderByDescending(p => p.Action)
                : query.OrderBy(p => p.Action),
            "createdat" => sortDirection.Equals("desc", StringComparison.CurrentCultureIgnoreCase) 
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt),
            _ => sortDirection.Equals("desc", StringComparison.CurrentCultureIgnoreCase) 
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name)
        };

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    /// <summary>
    /// Retrieves a permission by its identifier
    /// </summary>
    /// <param name="id">Permission identifier</param>
    /// <returns>The permission if found; otherwise, null</returns>
    public async Task<Permission?> GetByIdAsync(Guid id)
    {
        return await context.Permissions
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Retrieves a permission by name
    /// </summary>
    /// <param name="name">Permission name</param>
    /// <returns>The permission if found; otherwise, null</returns>
    public async Task<Permission?> GetByNameAsync(string name)
    {
        return await context.Permissions
            .FirstOrDefaultAsync(p => p.Name == name);
    }

    /// <summary>
    /// Retrieves a permission by resource and action
    /// </summary>
    /// <param name="resource">Resource name</param>
    /// <param name="action">Action name</param>
    /// <returns>The permission if found; otherwise, null</returns>
    public async Task<Permission?> GetByResourceAndActionAsync(string resource, string action)
    {
        return await context.Permissions
            .FirstOrDefaultAsync(p => p.Resource == resource && p.Action == action);
    }

    /// <summary>
    /// Retrieves permissions by resource
    /// </summary>
    /// <param name="resource">Resource name</param>
    /// <returns>A collection of permissions for the resource</returns>
    public async Task<IEnumerable<Permission>> GetByResourceAsync(string resource)
    {
        return await context.Permissions
            .Where(p => p.Resource == resource && p.IsActive)
            .OrderBy(p => p.Action)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves permissions grouped by resource
    /// </summary>
    /// <returns>A dictionary of permissions grouped by resource</returns>
    public async Task<Dictionary<string, IEnumerable<Permission>>> GetGroupedByResourceAsync()
    {
        var permissions = await context.Permissions
            .Where(p => p.IsActive)
            .OrderBy(p => p.Resource)
            .ThenBy(p => p.Action)
            .ToListAsync();

        return permissions
            .GroupBy(p => p.Resource)
            .ToDictionary(g => g.Key, g => g.AsEnumerable());
    }

    /// <summary>
    /// Retrieves permissions assigned to a role
    /// </summary>
    /// <param name="roleId">Role identifier</param>
    /// <returns>A collection of permissions assigned to the role</returns>
    public async Task<IEnumerable<Permission>> GetByRoleIdAsync(Guid roleId)
    {
        return await context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Include(rp => rp.Permission)
            .Select(rp => rp.Permission)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Resource)
            .ThenBy(p => p.Action)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves permissions available to a user through their roles
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>A collection of permissions available to the user</returns>
    public async Task<IEnumerable<Permission>> GetByUserIdAsync(Guid userId)
    {
        return await context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission)
            .Where(p => p.IsActive)
            .Distinct()
            .OrderBy(p => p.Resource)
            .ThenBy(p => p.Action)
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new permission
    /// </summary>
    /// <param name="permission">The permission to create</param>
    /// <returns>The created permission</returns>
    public async Task<Permission> CreateAsync(Permission permission)
    {
        context.Permissions.Add(permission);
        await context.SaveChangesAsync();
        return permission;
    }

    /// <summary>
    /// Updates an existing permission
    /// </summary>
    /// <param name="permission">The permission to update</param>
    /// <returns>The updated permission</returns>
    public async Task<Permission> UpdateAsync(Permission permission)
    {
        context.Permissions.Update(permission);
        await context.SaveChangesAsync();
        return permission;
    }

    /// <summary>
    /// Deletes a permission by its identifier
    /// </summary>
    /// <param name="id">The permission identifier</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task DeleteAsync(Guid id)
    {
        var permission = await context.Permissions.FindAsync(id);
        if (permission != null)
        {
            context.Permissions.Remove(permission);
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Deletes multiple permissions by their identifiers
    /// </summary>
    /// <param name="ids">The collection of permission identifiers</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task DeleteManyAsync(IEnumerable<Guid> ids)
    {
        var permissions = await context.Permissions
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
            
        context.Permissions.RemoveRange(permissions);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if a permission exists by its identifier
    /// </summary>
    /// <param name="id">The permission identifier</param>
    /// <returns>True if the permission exists; otherwise, false</returns>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await context.Permissions.AnyAsync(p => p.Id == id);
    }

    /// <summary>
    /// Checks if a permission name is already in use
    /// </summary>
    /// <param name="name">The permission name to check</param>
    /// <returns>True if the permission name exists; otherwise, false</returns>
    public async Task<bool> NameExistsAsync(string name)
    {
        return await context.Permissions.AnyAsync(p => p.Name == name);
    }

    /// <summary>
    /// Checks if a permission with the same resource and action already exists
    /// </summary>
    /// <param name="resource">The resource name</param>
    /// <param name="action">The action name</param>
    /// <param name="excludeId">Optional permission ID to exclude from the check</param>
    /// <returns>True if the combination exists; otherwise, false</returns>
    public async Task<bool> ResourceActionExistsAsync(string resource, string action, Guid? excludeId = null)
    {
        var query = context.Permissions.Where(p => p.Resource == resource && p.Action == action);
        
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    /// <summary>
    /// Gets the count of roles that have a specific permission
    /// </summary>
    /// <param name="permissionId">The permission identifier</param>
    /// <returns>The number of roles that have this permission</returns>
    public async Task<int> GetRoleCountAsync(Guid permissionId)
    {
        return await context.RolePermissions
            .Where(rp => rp.PermissionId == permissionId)
            .CountAsync();
    }

    /// <summary>
    /// Activates or deactivates multiple permissions
    /// </summary>
    /// <param name="ids">The collection of permission identifiers</param>
    /// <param name="isActive">The active status to set</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task UpdateActiveStatusAsync(IEnumerable<Guid> ids, bool isActive)
    {
        var permissions = await context.Permissions
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();

        foreach (var permission in permissions)
        {
            permission.IsActive = isActive;
            permission.UpdatedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();
    }

    // Legacy methods for backward compatibility - can be removed if not needed
    
    /// <summary>
    /// Deletes a permission
    /// </summary>
    /// <param name="permission">The permission to delete</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task DeleteAsync(Permission permission)
    {
        context.Permissions.Remove(permission);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if a permission name exists
    /// </summary>
    /// <param name="name">Permission name</param>
    /// <param name="excludeId">Optional permission ID to exclude from the check</param>
    /// <returns>True if the name exists; otherwise, false</returns>
    public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
    {
        var query = context.Permissions.Where(p => p.Name == name);
        
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    /// <summary>
    /// Checks if a resource and action combination exists
    /// </summary>
    /// <param name="resource">Resource name</param>
    /// <param name="action">Action name</param>
    /// <param name="excludeId">Optional permission ID to exclude from the check</param>
    /// <returns>True if the combination exists; otherwise, false</returns>
    public async Task<bool> ExistsByResourceAndActionAsync(string resource, string action, Guid? excludeId = null)
    {
        var query = context.Permissions.Where(p => p.Resource == resource && p.Action == action);
        
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    /// <summary>
    /// Checks if a permission is assigned to any roles
    /// </summary>
    /// <param name="permissionId">Permission identifier</param>
    /// <returns>True if the permission is assigned; otherwise, false</returns>
    public async Task<bool> IsAssignedToRolesAsync(Guid permissionId)
    {
        return await context.RolePermissions
            .AnyAsync(rp => rp.PermissionId == permissionId);
    }

    /// <summary>
    /// Performs bulk operations on permissions
    /// </summary>
    /// <param name="permissionIds">Collection of permission identifiers</param>
    /// <param name="operation">Operation to perform (activate, deactivate, delete)</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task BulkOperationAsync(IEnumerable<Guid> permissionIds, string operation)
    {
        var permissions = await context.Permissions
            .Where(p => permissionIds.Contains(p.Id))
            .ToListAsync();

        switch (operation.ToLower())
        {
            case "activate":
                foreach (var permission in permissions)
                {
                    permission.IsActive = true;
                    permission.UpdatedAt = DateTime.UtcNow;
                }
                break;

            case "deactivate":
                foreach (var permission in permissions)
                {
                    permission.IsActive = false;
                    permission.UpdatedAt = DateTime.UtcNow;
                }
                break;

            case "delete":
                context.Permissions.RemoveRange(permissions);
                break;
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Gets the count of permissions by status
    /// </summary>
    /// <param name="isActive">Filter by active status</param>
    /// <returns>The count of permissions</returns>
    public async Task<int> GetCountAsync(bool? isActive = null)
    {
        var query = context.Permissions.AsQueryable();
        
        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        return await query.CountAsync();
    }

    /// <summary>
    /// Gets permissions that match the specified names
    /// </summary>
    /// <param name="names">Collection of permission names</param>
    /// <returns>A collection of matching permissions</returns>
    public async Task<IEnumerable<Permission>> GetByNamesAsync(IEnumerable<string> names)
    {
        return await context.Permissions
            .Where(p => names.Contains(p.Name) && p.IsActive)
            .ToListAsync();
    }
}
