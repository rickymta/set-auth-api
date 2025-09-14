namespace Set.Auth.Domain.Interfaces;

/// <summary>
/// ICacheService defines methods for caching operations
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a cached item by key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// Sets a cached item with an optional expiration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiration"></param>
    /// <returns></returns>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Removes a cached item by key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task RemoveAsync(string key);

    /// <summary>
    /// Removes cached items matching a pattern
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    Task RemoveByPatternAsync(string pattern);

    /// <summary>
    /// Exists checks if a cached item exists by key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<bool> ExistsAsync(string key);
}
