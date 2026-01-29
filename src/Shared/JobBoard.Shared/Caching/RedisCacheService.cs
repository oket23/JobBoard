using StackExchange.Redis;
using System.Text.Json;

namespace JobBoard.Shared.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _db;
    private readonly IConnectionMultiplexer _redis;
    private readonly JsonSerializerOptions _serializerOptions;

    public RedisCacheService(IConnectionMultiplexer redis, JsonSerializerOptions? serializerOptions = null)
    {
        _redis = redis;
        _db = _redis.GetDatabase();
        _serializerOptions = serializerOptions ?? new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var value = await _db.StringGetAsync(key);
        if (!value.HasValue) return default;
        
        return JsonSerializer.Deserialize<T>((string)value, _serializerOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(value, _serializerOptions);
        await _db.StringSetAsync(key, json, ttl);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        await _db.KeyDeleteAsync(key);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: prefix + "*").ToArray();
        if (keys.Length > 0)
        {
            await _db.KeyDeleteAsync(keys);
        }
    }
}

