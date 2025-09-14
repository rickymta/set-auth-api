using AutoMapper;
using Set.Auth.Application.DTOs.Permission;
using Set.Auth.Application.Exceptions;
using Set.Auth.Application.Interfaces;
using Set.Auth.Domain.Entities;
using Set.Auth.Domain.Interfaces;

namespace Set.Auth.Application.Services;

/// <summary>
/// Service implementation for permission management operations
/// </summary>
/// <remarks>
/// Constructor to initialize dependencies
/// </remarks>
/// <param name="permissionRepository"></param>
/// <param name="roleRepository"></param>
/// <param name="mapper"></param>
public class PermissionService(
    IPermissionRepository permissionRepository,
    IRoleRepository roleRepository,
    IMapper mapper) : IPermissionService
{
    /// <inheritdoc/>
    public async Task<PermissionDto?> GetPermissionByIdAsync(Guid id)
    {
        var permission = await permissionRepository.GetByIdAsync(id);
        if (permission == null)
            return null;

        var permissionDto = mapper.Map<PermissionDto>(permission);
        permissionDto.RoleCount = await permissionRepository.GetRoleCountAsync(id);

        return permissionDto;
    }

    /// <inheritdoc/>
    public async Task<PermissionDto?> GetPermissionByNameAsync(string name)
    {
        var permission = await permissionRepository.GetByNameAsync(name);
        if (permission == null)
            return null;

        return await GetPermissionByIdAsync(permission.Id);
    }

    /// <inheritdoc/>
    public async Task<PermissionDto?> GetPermissionByResourceAndActionAsync(string resource, string action)
    {
        var permission = await permissionRepository.GetByResourceAndActionAsync(resource, action);
        if (permission == null)
            return null;

        return await GetPermissionByIdAsync(permission.Id);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync()
    {
        var permissions = await permissionRepository.GetAllAsync();
        var permissionDtos = new List<PermissionDto>();

        foreach (var permission in permissions)
        {
            var permissionDto = mapper.Map<PermissionDto>(permission);
            permissionDto.RoleCount = await permissionRepository.GetRoleCountAsync(permission.Id);
            permissionDtos.Add(permissionDto);
        }

        return permissionDtos;
    }

    /// <inheritdoc/>
    public async Task<PermissionListDto> GetPermissionsPagedAsync(PermissionFilterDto filter)
    {
        var (permissions, totalCount) = await permissionRepository.GetPagedAsync(
            filter.PageNumber,
            filter.PageSize,
            filter.SearchTerm,
            filter.Resource,
            filter.Action,
            filter.IsActive,
            filter.SortBy,
            filter.SortDirection);

        var permissionDtos = new List<PermissionDto>();
        foreach (var permission in permissions)
        {
            var permissionDto = mapper.Map<PermissionDto>(permission);
            permissionDto.RoleCount = await permissionRepository.GetRoleCountAsync(permission.Id);
            permissionDtos.Add(permissionDto);
        }

        var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

        return new PermissionListDto
        {
            Permissions = permissionDtos,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalPages = totalPages
        };
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PermissionDto>> GetPermissionsByRoleIdAsync(Guid roleId)
    {
        var permissions = await permissionRepository.GetByRoleIdAsync(roleId);
        return permissions.Select(p => mapper.Map<PermissionDto>(p));
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PermissionDto>> GetPermissionsByUserIdAsync(Guid userId)
    {
        var permissions = await permissionRepository.GetByUserIdAsync(userId);
        return permissions.Select(p => mapper.Map<PermissionDto>(p));
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, IEnumerable<PermissionDto>>> GetPermissionsGroupedByResourceAsync()
    {
        var groupedPermissions = await permissionRepository.GetGroupedByResourceAsync();
        var result = new Dictionary<string, IEnumerable<PermissionDto>>();

        foreach (var group in groupedPermissions)
        {
            result[group.Key] = group.Value.Select(p => mapper.Map<PermissionDto>(p));
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<PermissionDto> CreatePermissionAsync(PermissionCreateDto createDto)
    {
        // Validate permission name doesn't exist
        if (await permissionRepository.NameExistsAsync(createDto.Name))
        {
            throw new Exceptions.ValidationException($"Permission with name '{createDto.Name}' already exists");
        }

        // Validate resource and action combination doesn't exist
        if (await permissionRepository.ResourceActionExistsAsync(createDto.Resource, createDto.Action))
        {
            throw new Exceptions.ValidationException($"Permission for resource '{createDto.Resource}' and action '{createDto.Action}' already exists");
        }

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Description = createDto.Description,
            Resource = createDto.Resource,
            Action = createDto.Action,
            IsActive = createDto.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdPermission = await permissionRepository.CreateAsync(permission);
        return await GetPermissionByIdAsync(createdPermission.Id) ?? throw new InvalidOperationException("Failed to create permission");
    }

    /// <inheritdoc/>
    public async Task<PermissionDto> UpdatePermissionAsync(Guid id, PermissionUpdateDto updateDto)
    {
        var permission = await permissionRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Permission with ID '{id}' not found");

        // Validate permission name doesn't exist (excluding current permission)
        var existingPermission = await permissionRepository.GetByNameAsync(updateDto.Name);
        if (existingPermission != null && existingPermission.Id != id)
        {
            throw new Exceptions.ValidationException($"Permission with name '{updateDto.Name}' already exists");
        }

        // Validate resource and action combination doesn't exist (excluding current permission)
        if (await permissionRepository.ResourceActionExistsAsync(updateDto.Resource, updateDto.Action, id))
        {
            throw new Exceptions.ValidationException($"Permission for resource '{updateDto.Resource}' and action '{updateDto.Action}' already exists");
        }

        permission.Name = updateDto.Name;
        permission.Description = updateDto.Description;
        permission.Resource = updateDto.Resource;
        permission.Action = updateDto.Action;
        permission.IsActive = updateDto.IsActive;
        permission.UpdatedAt = DateTime.UtcNow;

        await permissionRepository.UpdateAsync(permission);
        return await GetPermissionByIdAsync(id) ?? throw new InvalidOperationException("Failed to update permission");
    }

    /// <inheritdoc/>
    public async Task DeletePermissionAsync(Guid id)
    {
        _ = await permissionRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Permission with ID '{id}' not found");

        // Check if permission is assigned to any roles
        var roleCount = await permissionRepository.GetRoleCountAsync(id);
        if (roleCount > 0)
        {
            throw new Exceptions.ValidationException("Cannot delete permission that is assigned to roles");
        }

        await permissionRepository.DeleteAsync(id);
    }

    /// <inheritdoc/>
    public async Task DeletePermissionsAsync(IEnumerable<Guid> ids)
    {
        foreach (var id in ids)
        {
            var permission = await permissionRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Permission with ID '{id}' not found");
            var roleCount = await permissionRepository.GetRoleCountAsync(id);
            if (roleCount > 0)
            {
                throw new Exceptions.ValidationException($"Cannot delete permission '{permission.Name}' that is assigned to roles");
            }
        }

        await permissionRepository.DeleteManyAsync(ids);
    }

    /// <inheritdoc/>
    public async Task ActivatePermissionAsync(Guid id)
    {
        var permission = await permissionRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Permission with ID '{id}' not found");
        if (permission.IsActive)
        {
            return; // Already active
        }

        permission.IsActive = true;
        permission.UpdatedAt = DateTime.UtcNow;
        await permissionRepository.UpdateAsync(permission);
    }

    /// <inheritdoc/>
    public async Task DeactivatePermissionAsync(Guid id)
    {
        var permission = await permissionRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Permission with ID '{id}' not found");
        if (!permission.IsActive)
        {
            return; // Already inactive
        }

        permission.IsActive = false;
        permission.UpdatedAt = DateTime.UtcNow;
        await permissionRepository.UpdateAsync(permission);
    }

    /// <inheritdoc/>
    public async Task BulkPermissionOperationAsync(BulkPermissionDto bulkDto)
    {
        switch (bulkDto.Operation.ToLower())
        {
            case "activate":
                await permissionRepository.UpdateActiveStatusAsync(bulkDto.PermissionIds, true);
                break;
            case "deactivate":
                await permissionRepository.UpdateActiveStatusAsync(bulkDto.PermissionIds, false);
                break;
            case "delete":
                await DeletePermissionsAsync(bulkDto.PermissionIds);
                break;
            default:
                throw new Exceptions.ValidationException($"Invalid operation '{bulkDto.Operation}'. Supported operations: activate, deactivate, delete");
        }
    }

    /// <inheritdoc/>
    public async Task<PermissionAssignmentResponseDto> AssignPermissionsToRoleAsync(PermissionAssignmentDto assignmentDto)
    {
        var role = await roleRepository.GetByIdAsync(assignmentDto.RoleId) ?? throw new NotFoundException($"Role with ID '{assignmentDto.RoleId}' not found");

        // Validate all permission IDs exist
        foreach (var permissionId in assignmentDto.PermissionIds)
        {
            if (!await permissionRepository.ExistsAsync(permissionId))
            {
                throw new ValidationException($"Permission with ID '{permissionId}' not found");
            }
        }

        // Get current role permissions
        var currentPermissions = await permissionRepository.GetByRoleIdAsync(assignmentDto.RoleId);
        var currentPermissionIds = currentPermissions.Select(p => p.Id).ToList();

        // Add new permissions
        var permissionsToAdd = assignmentDto.PermissionIds.Except(currentPermissionIds);
        foreach (var permissionId in permissionsToAdd)
        {
            role.RolePermissions.Add(new RolePermission
            {
                RoleId = assignmentDto.RoleId,
                PermissionId = permissionId,
                GrantedAt = DateTime.UtcNow,
                IsActive = true
            });
        }

        await roleRepository.UpdateAsync(role);

        var assignedPermissions = await GetPermissionsByRoleIdAsync(assignmentDto.RoleId);

        return new PermissionAssignmentResponseDto
        {
            RoleId = assignmentDto.RoleId,
            RoleName = role.Name,
            Permissions = [.. assignedPermissions],
            AssignedAt = DateTime.UtcNow
        };
    }

    /// <inheritdoc/>
    public async Task<bool> PermissionExistsAsync(Guid id)
    {
        return await permissionRepository.ExistsAsync(id);
    }

    /// <inheritdoc/>
    public async Task<bool> PermissionNameExistsAsync(string name, Guid? excludeId = null)
    {
        var existingPermission = await permissionRepository.GetByNameAsync(name);
        return existingPermission != null && (excludeId == null || existingPermission.Id != excludeId);
    }

    /// <inheritdoc/>
    public async Task<bool> ResourceActionExistsAsync(string resource, string action, Guid? excludeId = null)
    {
        return await permissionRepository.ResourceActionExistsAsync(resource, action, excludeId);
    }

    /// <inheritdoc/>
    public async Task<bool> CanPerformPermissionOperationAsync(Guid userId, string operation)
    {
        // Get user permissions
        var userPermissions = await permissionRepository.GetByUserIdAsync(userId);

        // Define required permissions for each operation
        var requiredPermissions = operation.ToLower() switch
        {
            "create" => ["permissions.create", "permissions.manage"],
            "read" => ["permissions.read", "permissions.manage"],
            "update" => ["permissions.update", "permissions.manage"],
            "delete" => ["permissions.delete", "permissions.manage"],
            "assign" => ["permissions.assign", "permissions.manage"],
            _ => new[] { "permissions.manage" }
        };

        return userPermissions.Any(p => requiredPermissions.Contains(p.Name) && p.IsActive);
    }

    /// <inheritdoc/>
    public async Task<bool> UserHasPermissionAsync(Guid userId, string resource, string action)
    {
        var userPermissions = await permissionRepository.GetByUserIdAsync(userId);
        return userPermissions.Any(p => 
            p.Resource.Equals(resource, StringComparison.OrdinalIgnoreCase) &&
            p.Action.Equals(action, StringComparison.OrdinalIgnoreCase) &&
            p.IsActive);
    }

    /// <inheritdoc/>
    public async Task<bool> UserHasAnyPermissionAsync(Guid userId, IEnumerable<string> permissions)
    {
        var userPermissions = await permissionRepository.GetByUserIdAsync(userId);
        var userPermissionNames = userPermissions.Where(p => p.IsActive).Select(p => p.Name);
        return permissions.Any(p => userPermissionNames.Contains(p));
    }

    /// <inheritdoc/>
    public async Task<bool> UserHasAllPermissionsAsync(Guid userId, IEnumerable<string> permissions)
    {
        var userPermissions = await permissionRepository.GetByUserIdAsync(userId);
        var userPermissionNames = userPermissions.Where(p => p.IsActive).Select(p => p.Name);
        return permissions.All(p => userPermissionNames.Contains(p));
    }
}
