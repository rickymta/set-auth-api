using AutoMapper;
using Set.Auth.Application.DTOs.Role;
using Set.Auth.Application.Exceptions;
using Set.Auth.Application.Interfaces;
using Set.Auth.Domain.Entities;
using Set.Auth.Domain.Interfaces;

namespace Set.Auth.Application.Services;

/// <summary>
/// Service implementation for role management operations
/// </summary>
/// <remarks>
/// Constructor for RoleService
/// </remarks>
/// <param name="roleRepository"></param>
/// <param name="userRepository"></param>
/// <param name="permissionRepository"></param>
/// <param name="mapper"></param>
public class RoleService(
    IRoleRepository roleRepository,
    IUserRepository userRepository,
    IPermissionRepository permissionRepository,
    IMapper mapper) : IRoleService
{
    /// <inheritdoc/>
    public async Task<RoleDto?> GetRoleByIdAsync(Guid id)
    {
        var role = await roleRepository.GetByIdAsync(id);
        if (role == null)
            return null;

        var roleDto = mapper.Map<RoleDto>(role);
        
        // Get permissions for this role
        var permissions = await permissionRepository.GetByRoleIdAsync(id);
        roleDto.Permissions = [.. permissions.Select(p => p.Name)];
        
        // Get user count for this role
        var users = await userRepository.GetAllAsync();
        roleDto.UserCount = users.Count(u => u.UserRoles.Any(ur => ur.RoleId == id && ur.IsActive));

        return roleDto;
    }

    /// <inheritdoc/>
    public async Task<RoleDto?> GetRoleByNameAsync(string name)
    {
        var role = await roleRepository.GetByNameAsync(name);
        if (role == null)
            return null;

        return await GetRoleByIdAsync(role.Id);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roles = await roleRepository.GetAllAsync();
        var roleDtos = new List<RoleDto>();

        foreach (var role in roles)
        {
            var roleDto = mapper.Map<RoleDto>(role);
            
            // Get permissions for this role
            var permissions = await permissionRepository.GetByRoleIdAsync(role.Id);
            roleDto.Permissions = [.. permissions.Select(p => p.Name)];
            
            // Get user count for this role
            var users = await userRepository.GetAllAsync();
            roleDto.UserCount = users.Count(u => u.UserRoles.Any(ur => ur.RoleId == role.Id && ur.IsActive));

            roleDtos.Add(roleDto);
        }

        return roleDtos;
    }

    /// <inheritdoc/>
    public async Task<RoleListDto> GetRolesPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null,
        bool? isActive = null,
        string sortBy = "Name",
        string sortDirection = "asc")
    {
        // For now, implement basic filtering and pagination
        // In a real implementation, this would be done at the repository level
        var allRoles = await GetAllRolesAsync();
        
        // Apply filters
        var filteredRoles = allRoles.AsQueryable();
        
        if (!string.IsNullOrEmpty(searchTerm))
        {
            filteredRoles = filteredRoles.Where(r => 
                r.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (r.Description != null && r.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
        }
        
        if (isActive.HasValue)
        {
            filteredRoles = filteredRoles.Where(r => r.IsActive == isActive.Value);
        }
        
        // Apply sorting
        filteredRoles = sortBy.ToLower() switch
        {
            "name" => sortDirection.Equals("desc", StringComparison.CurrentCultureIgnoreCase)
                ? filteredRoles.OrderByDescending(r => r.Name)
                : filteredRoles.OrderBy(r => r.Name),
            "createdat" => sortDirection.Equals("desc", StringComparison.CurrentCultureIgnoreCase)
                ? filteredRoles.OrderByDescending(r => r.CreatedAt)
                : filteredRoles.OrderBy(r => r.CreatedAt),
            "usercount" => sortDirection.Equals("desc", StringComparison.CurrentCultureIgnoreCase)
                ? filteredRoles.OrderByDescending(r => r.UserCount)
                : filteredRoles.OrderBy(r => r.UserCount),
            _ => filteredRoles.OrderBy(r => r.Name)
        };
        
        var totalCount = filteredRoles.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        var roles = filteredRoles
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new RoleListDto
        {
            Roles = roles,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<RoleDto>> GetRolesByUserIdAsync(Guid userId)
    {
        var roles = await roleRepository.GetByUserIdAsync(userId);
        var roleDtos = new List<RoleDto>();

        foreach (var role in roles)
        {
            var roleDto = mapper.Map<RoleDto>(role);
            var permissions = await permissionRepository.GetByRoleIdAsync(role.Id);
            roleDto.Permissions = [.. permissions.Select(p => p.Name)];
            roleDtos.Add(roleDto);
        }

        return roleDtos;
    }

    /// <inheritdoc/>
    public async Task<RoleDto> CreateRoleAsync(RoleCreateDto createDto)
    {
        // Validate role name doesn't exist
        if (await roleRepository.NameExistsAsync(createDto.Name))
        {
            throw new Exceptions.ValidationException($"Role with name '{createDto.Name}' already exists");
        }

        // Validate permission IDs exist
        foreach (var permissionId in createDto.PermissionIds)
        {
            if (!await permissionRepository.ExistsAsync(permissionId))
            {
                throw new Exceptions.ValidationException($"Permission with ID '{permissionId}' not found");
            }
        }

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Description = createDto.Description,
            IsActive = createDto.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdRole = await roleRepository.CreateAsync(role);

        // Assign permissions to role
        if (createDto.PermissionIds.Count != 0)
        {
            await AssignPermissionsToRoleAsync(createdRole.Id, createDto.PermissionIds);
        }

        return await GetRoleByIdAsync(createdRole.Id) ?? throw new InvalidOperationException("Failed to create role");
    }

    /// <inheritdoc/>
    public async Task<RoleDto> UpdateRoleAsync(Guid id, RoleUpdateDto updateDto)
    {
        var role = await roleRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Role with ID '{id}' not found");

        // Validate role name doesn't exist (excluding current role)
        var existingRole = await roleRepository.GetByNameAsync(updateDto.Name);
        if (existingRole != null && existingRole.Id != id)
        {
            throw new ValidationException($"Role with name '{updateDto.Name}' already exists");
        }

        // Validate permission IDs exist
        foreach (var permissionId in updateDto.PermissionIds)
        {
            if (!await permissionRepository.ExistsAsync(permissionId))
            {
                throw new ValidationException($"Permission with ID '{permissionId}' not found");
            }
        }

        role.Name = updateDto.Name;
        role.Description = updateDto.Description;
        role.IsActive = updateDto.IsActive;
        role.UpdatedAt = DateTime.UtcNow;

        await roleRepository.UpdateAsync(role);

        // Update role permissions
        var currentPermissions = await permissionRepository.GetByRoleIdAsync(id);
        var currentPermissionIds = currentPermissions.Select(p => p.Id).ToList();
        
        var permissionsToAdd = updateDto.PermissionIds.Except(currentPermissionIds);
        var permissionsToRemove = currentPermissionIds.Except(updateDto.PermissionIds);

        if (permissionsToAdd.Any())
        {
            await AssignPermissionsToRoleAsync(id, permissionsToAdd);
        }

        if (permissionsToRemove.Any())
        {
            await RemovePermissionsFromRoleAsync(id, permissionsToRemove);
        }

        return await GetRoleByIdAsync(id) ?? throw new InvalidOperationException("Failed to update role");
    }

    /// <inheritdoc/>
    public async Task DeleteRoleAsync(Guid id)
    {
        var role = await roleRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Role with ID '{id}' not found");

        // Check if role is assigned to any users
        var users = await userRepository.GetAllAsync();
        var hasActiveUsers = users.Any(u => u.UserRoles.Any(ur => ur.RoleId == id && ur.IsActive));
        
        if (hasActiveUsers)
        {
            throw new ValidationException("Cannot delete role that is assigned to active users");
        }

        await roleRepository.DeleteAsync(id);
    }

    /// <inheritdoc/>
    public async Task ActivateRoleAsync(Guid id)
    {
        var role = await roleRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Role with ID '{id}' not found");
        if (role.IsActive)
        {
            return; // Already active
        }

        role.IsActive = true;
        role.UpdatedAt = DateTime.UtcNow;
        await roleRepository.UpdateAsync(role);
    }

    /// <inheritdoc/>
    public async Task DeactivateRoleAsync(Guid id)
    {
        var role = await roleRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Role with ID '{id}' not found");
        if (!role.IsActive)
        {
            return; // Already inactive
        }

        role.IsActive = false;
        role.UpdatedAt = DateTime.UtcNow;
        await roleRepository.UpdateAsync(role);
    }

    /// <inheritdoc/>
    public async Task<RoleAssignmentResponseDto> AssignRolesToUserAsync(RoleAssignmentDto assignmentDto)
    {
        var user = await userRepository.GetByIdAsync(assignmentDto.UserId) ?? throw new NotFoundException($"User with ID '{assignmentDto.UserId}' not found");

        // Validate all role IDs exist
        foreach (var roleId in assignmentDto.RoleIds)
        {
            if (!await roleRepository.ExistsAsync(roleId))
            {
                throw new Exceptions.ValidationException($"Role with ID '{roleId}' not found");
            }
        }

        // Get current user roles
        var currentRoleIds = user.UserRoles.Where(ur => ur.IsActive).Select(ur => ur.RoleId).ToList();
        
        // Add new roles
        var rolesToAdd = assignmentDto.RoleIds.Except(currentRoleIds);
        foreach (var roleId in rolesToAdd)
        {
            user.UserRoles.Add(new UserRole
            {
                UserId = assignmentDto.UserId,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow,
                ExpiresAt = assignmentDto.ExpiresAt,
                IsActive = true
            });
        }

        await userRepository.UpdateAsync(user);

        var assignedRoles = await GetRolesByUserIdAsync(assignmentDto.UserId);
        
        return new RoleAssignmentResponseDto
        {
            UserId = assignmentDto.UserId,
            UserName = $"{user.FirstName} {user.LastName}".Trim(),
            Roles = [.. assignedRoles],
            AssignedAt = DateTime.UtcNow
        };
    }

    /// <inheritdoc/>
    public async Task RemoveRolesFromUserAsync(Guid userId, IEnumerable<Guid> roleIds)
    {
        var user = await userRepository.GetByIdAsync(userId) ?? throw new NotFoundException($"User with ID '{userId}' not found");
        foreach (var roleId in roleIds)
        {
            var userRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == roleId && ur.IsActive);
            if (userRole != null)
            {
                userRole.IsActive = false;
            }
        }

        await userRepository.UpdateAsync(user);
    }

    /// <inheritdoc/>
    public async Task AssignPermissionsToRoleAsync(Guid roleId, IEnumerable<Guid> permissionIds)
    {
        var role = await roleRepository.GetByIdAsync(roleId) ?? throw new NotFoundException($"Role with ID '{roleId}' not found");
        foreach (var permissionId in permissionIds)
        {
            if (!await permissionRepository.ExistsAsync(permissionId))
            {
                throw new Exceptions.ValidationException($"Permission with ID '{permissionId}' not found");
            }

            // Check if permission is already assigned
            var existingAssignment = role.RolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId && rp.IsActive);
            if (existingAssignment == null)
            {
                role.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId,
                    GrantedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }
        }

        await roleRepository.UpdateAsync(role);
    }

    /// <inheritdoc/>
    public async Task RemovePermissionsFromRoleAsync(Guid roleId, IEnumerable<Guid> permissionIds)
    {
        var role = await roleRepository.GetByIdAsync(roleId) ?? throw new NotFoundException($"Role with ID '{roleId}' not found");
        foreach (var permissionId in permissionIds)
        {
            var rolePermission = role.RolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId && rp.IsActive);
            if (rolePermission != null)
            {
                rolePermission.IsActive = false;
            }
        }

        await roleRepository.UpdateAsync(role);
    }

    /// <inheritdoc/>
    public async Task<bool> RoleExistsAsync(Guid id)
    {
        return await roleRepository.ExistsAsync(id);
    }

    /// <inheritdoc/>
    public async Task<bool> RoleNameExistsAsync(string name, Guid? excludeId = null)
    {
        var existingRole = await roleRepository.GetByNameAsync(name);
        return existingRole != null && (excludeId == null || existingRole.Id != excludeId);
    }

    /// <inheritdoc/>
    public async Task<bool> CanPerformRoleOperationAsync(Guid userId, string operation)
    {
        // Get user permissions
        var userPermissions = await permissionRepository.GetByUserIdAsync(userId);
        
        // Define required permissions for each operation
        var requiredPermissions = operation.ToLower() switch
        {
            "create" => ["roles.create", "roles.manage"],
            "read" => ["roles.read", "roles.manage"],
            "update" => ["roles.update", "roles.manage"],
            "delete" => ["roles.delete", "roles.manage"],
            "assign" => ["roles.assign", "roles.manage"],
            _ => new[] { "roles.manage" }
        };

        return userPermissions.Any(p => requiredPermissions.Contains(p.Name) && p.IsActive);
    }
}
