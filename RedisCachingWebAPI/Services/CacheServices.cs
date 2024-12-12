
using StackExchange.Redis;
using System.Text.Json;

namespace RedisCachingWebAPI.Services;

public class CacheServices : ICacheServices
{
   private IDatabase _cacheDb;
    public CacheServices()
    {
        var redis = ConnectionMultiplexer.Connect("localhost:6379");
        _cacheDb= redis.GetDatabase();
    }
    public T GetData<T>(string key)
    {
        var value=_cacheDb.StringGet(key);
        if (!string.IsNullOrEmpty(value))
            return JsonSerializer.Deserialize<T>(value);
        return default;
    }

    public object RemoveData(string key)
    {
       var _exist=_cacheDb.KeyExists(key);
        if(_exist)
         return _cacheDb.KeyDelete(key);
        return false;

    }

    public bool SetData<T>(string key, T value, DateTimeOffset ExpirationTime)
    {
       var expiryTime=ExpirationTime.DateTime.Subtract(DateTime.Now);
       return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expiryTime);
    }
}
