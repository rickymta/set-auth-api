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
public class RefreshTokenRepository(AuthDbContext context) : IRefreshTokenRepository
{
    /// <inheritdoc />
    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(Guid userId)
    {
        return await context.RefreshTokens
            .Where(rt => rt.UserId == userId)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(Guid userId)
    {
        return await context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
    {
        refreshToken.CreatedAt = DateTime.UtcNow;

        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();
        return refreshToken;
    }

    /// <inheritdoc />
    public async Task<RefreshToken> UpdateAsync(RefreshToken refreshToken)
    {
        context.RefreshTokens.Update(refreshToken);
        await context.SaveChangesAsync();
        return refreshToken;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        var refreshToken = await context.RefreshTokens.FindAsync(id);
        if (refreshToken != null)
        {
            context.RefreshTokens.Remove(refreshToken);
            await context.SaveChangesAsync();
        }
    }

    /// <inheritdoc />
    public async Task RevokeAsync(string token, string? revokedByIp = null)
    {
        var refreshToken = await GetByTokenAsync(token);
        if (refreshToken != null && refreshToken.IsActive)
        {
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = revokedByIp;
            await UpdateAsync(refreshToken);
        }
    }

    /// <inheritdoc />
    public async Task RevokeAllByUserIdAsync(Guid userId, string? revokedByIp = null)
    {
        var activeTokens = await GetActiveByUserIdAsync(userId);
        foreach (var token in activeTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = revokedByIp;
        }

        if (activeTokens.Any())
        {
            context.RefreshTokens.UpdateRange(activeTokens);
            await context.SaveChangesAsync();
        }
    }

    /// <inheritdoc />
    public async Task RevokeByDeviceIdAsync(Guid userId, string deviceId, string? revokedByIp = null)
    {
        var deviceTokens = await context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.DeviceId == deviceId && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in deviceTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = revokedByIp;
        }

        if (deviceTokens.Any())
        {
            context.RefreshTokens.UpdateRange(deviceTokens);
            await context.SaveChangesAsync();
        }
    }

    /// <inheritdoc />
    public async Task CleanupExpiredTokensAsync()
    {
        var expiredTokens = await context.RefreshTokens.Where(rt => rt.ExpiresAt <= DateTime.UtcNow).ToListAsync();
        if (expiredTokens.Count != 0)
        {
            context.RefreshTokens.RemoveRange(expiredTokens);
            await context.SaveChangesAsync();
        }
    }
}
