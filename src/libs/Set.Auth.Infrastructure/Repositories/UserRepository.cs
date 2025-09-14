using Microsoft.EntityFrameworkCore;
using Set.Auth.Domain.Entities;
using Set.Auth.Domain.Interfaces;
using Set.Auth.Infrastructure.Data;

namespace Set.Auth.Infrastructure.Repositories;

/// <summary>
/// User repository for managing user data
/// </summary>
/// <remarks>
/// Constructor to initialize the repository with the DbContext
/// </remarks>
/// <param name="context"></param>
public class UserRepository(AuthDbContext context) : IUserRepository
{
    /// <inheritdoc />
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <inheritdoc />
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Email.Equals(email));
    }

    /// <inheritdoc />
    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
    {
        return await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
    }

    /// <inheritdoc />
    public async Task<User?> GetByEmailOrPhoneAsync(string emailOrPhone)
    {
        return await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Email.Equals(emailOrPhone) || u.PhoneNumber == emailOrPhone);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<User> CreateAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    /// <inheritdoc />
    public async Task<User> UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;

        context.Users.Update(user);
        await context.SaveChangesAsync();
        return user;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        var user = await context.Users.FindAsync(id);
        if (user != null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await context.Users.AnyAsync(u => u.Id == id);
    }

    /// <inheritdoc />
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await context.Users.AnyAsync(u => u.Email.Equals(email));
    }

    /// <inheritdoc />
    public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
    {
        return await context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
    }
}
