using Microsoft.EntityFrameworkCore;
using Set.Auth.Domain.Entities;
using Set.Auth.Domain.Interfaces;
using Set.Auth.Infrastructure.Data;

namespace Set.Auth.Infrastructure.Repositories;

/// <inheritdoc />
/// <summary>
/// Constructor to initialize the repository with the database context
/// </summary>
/// <param name="context"></param>
public class RoleRepository(AuthDbContext context) : IRoleRepository
{
    /// <inheritdoc />
    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    /// <inheritdoc />
    public async Task<Role?> GetByNameAsync(string name)
    {
        return await context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Name == name);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Role>> GetByUserIdAsync(Guid userId)
    {
        return await context.UserRoles
            .Where(ur => ur.UserId == userId && ur.IsActive)
            .Include(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Select(ur => ur.Role)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Role> CreateAsync(Role role)
    {
        role.CreatedAt = DateTime.UtcNow;
        role.UpdatedAt = DateTime.UtcNow;

        context.Roles.Add(role);
        await context.SaveChangesAsync();
        return role;
    }

    /// <inheritdoc />
    public async Task<Role> UpdateAsync(Role role)
    {
        role.UpdatedAt = DateTime.UtcNow;

        context.Roles.Update(role);
        await context.SaveChangesAsync();
        return role;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        var role = await context.Roles.FindAsync(id);
        if (role != null)
        {
            context.Roles.Remove(role);
            await context.SaveChangesAsync();
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await context.Roles.AnyAsync(r => r.Id == id);
    }

    /// <inheritdoc />
    public async Task<bool> NameExistsAsync(string name)
    {
        return await context.Roles.AnyAsync(r => r.Name == name);
    }
}
