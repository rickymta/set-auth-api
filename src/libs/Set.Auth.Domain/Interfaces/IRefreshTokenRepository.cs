using Set.Auth.Domain.Entities;

namespace Set.Auth.Domain.Interfaces;

/// <summary>
/// Repository interface for managing refresh tokens
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Gets a refresh token by its token string
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<RefreshToken?> GetByTokenAsync(string token);

    /// <summary>
    /// Gets all refresh tokens for a specific user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<IEnumerable<RefreshToken>> GetByUserIdAsync(Guid userId);

    /// <summary>
    /// Gets all active (not revoked or expired) refresh tokens for a specific user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<IEnumerable<RefreshToken>> GetActiveByUserIdAsync(Guid userId);

    /// <summary>
    /// Creates a new refresh token
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    Task<RefreshToken> CreateAsync(RefreshToken refreshToken);

    /// <summary>
    /// Updates an existing refresh token
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    Task<RefreshToken> UpdateAsync(RefreshToken refreshToken);

    /// <summary>
    /// Deletes a refresh token by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Revokes a refresh token by its token string
    /// </summary>
    /// <param name="token"></param>
    /// <param name="revokedByIp"></param>
    /// <returns></returns>
    Task<Guid?> RevokeAsync(string token, string? revokedByIp = null);

    /// <summary>
    /// Revokes all refresh tokens for a specific user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="revokedByIp"></param>
    /// <returns></returns>
    Task RevokeAllByUserIdAsync(Guid userId, string? revokedByIp = null);

    /// <summary>
    /// Revokes all refresh tokens for a specific user and device ID
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="deviceId"></param>
    /// <param name="revokedByIp"></param>
    /// <returns></returns>
    Task RevokeByDeviceIdAsync(Guid userId, string deviceId, string? revokedByIp = null);

    /// <summary>
    /// Cleanup expired refresh tokens
    /// </summary>
    /// <returns></returns>
    Task CleanupExpiredTokensAsync();
}
