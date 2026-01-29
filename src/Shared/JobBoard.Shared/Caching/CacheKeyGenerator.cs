using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace JobBoard.Shared.Caching;

public static class CacheKeyGenerator
{
    public static string GetCacheKey<T>(string prefix, T body) 
    {
        var json = JsonSerializer.Serialize(body);
        
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
        
        var hash = Convert.ToHexString(bytes);
        
        return $"{prefix}:{hash}";
    }
}