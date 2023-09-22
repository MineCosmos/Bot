using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineCosmos.Bot.Entity;
using SqlSugar;

namespace MineCosmos.Bot.Service.MyCache;

/// <summary>
/// 缓存
/// </summary>
public class DbcacheService : BaseService, IMyCache
{
    public DbcacheService()
    {
        _db = SqlSugarHelper.Instance;
    }

    public async Task Set(string key, string value, TimeSpan timeSpan)
    {
        if (await Exit(key))
        {
            await _db.Deleteable<CacheEntity>().Where(a => a.Key == key).ExecuteCommandAsync();
        }

        await _db.Insertable(new CacheEntity
        {
            Key = key,
            Val = value,
            Expiration = DateTime.Now.Add(timeSpan)
        }).ExecuteCommandAsync();
    }

    public async Task<CacheEntity> Get(string key)
    {
        return await _db.Queryable<CacheEntity>().FirstAsync(a => a.Key == key);
    }

    public async Task<bool> Exit(string key)
    {
        return await _db.Queryable<CacheEntity>().AnyAsync(a => a.Key == key);
    }

    public async Task Del(string key)
    {
        await _db.Deleteable<CacheEntity>().Where(a => a.Key == key).ExecuteCommandAsync();
    }
}


public class MemoryCacheService : BaseService, IMyCache
{
    public MemoryCacheService()
    {
        
    }

    public Task Del(string key)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Exit(string key)
    {
        throw new NotImplementedException();
    }

    public Task<CacheEntity> Get(string key)
    {
        throw new NotImplementedException();
    }

    public Task Set(string key, string value, TimeSpan timeSpan)
    {
        throw new NotImplementedException();
    }
}

public interface IMyCache: IBaseService
{
    Task Set(string key, string value, TimeSpan timeSpan);

    Task<CacheEntity> Get(string key);

    Task<bool> Exit(string key);

    Task Del(string key);
}

