using System.Text.Json;
using StackExchange.Redis;
using Set.Auth.Domain.Interfaces;

namespace Set.Auth.Infrastructure.Services;

/// <inheritdoc />
/// <summary>
/// Redis-based implementation of ICacheService
/// </summary>
public class RedisCacheService : ICacheService
{
    /// <summary>
    /// IDatabase instance for Redis operations
    /// </summary>
    private readonly IDatabase _database;

    /// <summary>
    /// IServer instance for Redis server operations
    /// </summary>
    private readonly IServer _server;

    /// <summary>
    /// JsonSerializerOptions for consistent serialization settings
    /// </summary>
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Constructor to initialize Redis connection and JSON options
    /// </summary>
    /// <param name="redis"></param>
    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
        _server = redis.GetServer(redis.GetEndPoints().First());
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        var value = await _database.StringGetAsync(key);
        
        if (!value.HasValue)
            return null;

        try
        {
            return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
        await _database.StringSetAsync(key, serializedValue, expiration);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    /// <inheritdoc />
    public async Task RemoveByPatternAsync(string pattern)
    {
        var keys = _server.Keys(pattern: pattern);
        foreach (var key in keys)
        {
            await _database.KeyDeleteAsync(key);
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }
}
