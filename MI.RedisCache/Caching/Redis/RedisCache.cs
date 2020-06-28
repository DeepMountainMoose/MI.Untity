using MI.Core;
using MI.Core.Runtime.Caching;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MI.RedisCache.Caching.Redis
{
    /// <summary>
    ///     用于将数据缓存到Redis服务器
    /// </summary>
    public class RedisCache : CacheBase
    {
        private readonly IDatabase _database;
        private readonly IRedisCacheSerializer _serializer;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public RedisCache(string name, IRedisCacheDatabaseProvider redisCacheDatabaseProvider, IRedisCacheSerializer serializer)
            : base(name)
        {
            _serializer = serializer;
            _database = redisCacheDatabaseProvider.GetDatabase();
        }

        public override object GetOrDefault(string key)
        {
            var objbyte = _database.StringGet(GetLocalizedRedisKey(key));
            return objbyte.HasValue ? Deserialize(objbyte) : null;
        }

        public override TValue GetOrDefault<TKey, TValue>(TKey key)
        {
            var objbyte = _database.StringGet(GetLocalizedRedisKey(key.ToString()));
            return objbyte.HasValue ? Deserialize<TValue>(objbyte) : default;
        }

        public override object[] GetOrDefault(string[] keys)
        {
            var redisValues = _database.StringGet(GetRedisKeyList(keys));
            var objbytes = redisValues.Select(obj => obj.HasValue ? Deserialize(obj) : null);
            return objbytes.ToArray();
        }

        public override TValue[] GetOrDefault<TKey, TValue>(TKey[] keys)
        {
            var redisValues = _database.StringGet(GetRedisKeyList(keys));
            var objbytes = redisValues.Select(obj => obj.HasValue ? Deserialize<TValue>(obj) : default);
            return objbytes.ToArray();
        }

        public override async Task<object> GetOrDefaultAsync(string key)
        {
            var objbyte = await _database.StringGetAsync(GetLocalizedRedisKey(key));
            return objbyte.HasValue ? Deserialize(objbyte) : null;
        }

        public override async Task<TValue> GetOrDefaultAsync<TKey, TValue>(TKey key)
        {
            var objbyte = await _database.StringGetAsync(GetLocalizedRedisKey(key.ToString()));
            return objbyte.HasValue ? Deserialize<TValue>(objbyte) : default;
        }

        public override async Task<object[]> GetOrDefaultAsync(string[] keys)
        {
            var redisValues = await _database.StringGetAsync(GetRedisKeyList(keys));
            var objbytes = redisValues.Select(obj => obj.HasValue ? Deserialize(obj) : null);
            return objbytes.ToArray();
        }

        public override async Task<TValue[]> GetOrDefaultAsync<TKey, TValue>(TKey[] keys)
        {
            var redisValues = await _database.StringGetAsync(GetRedisKeyList(keys));
            var objbytes = redisValues.Select(obj => obj.HasValue ? Deserialize<TValue>(obj) : default);
            return objbytes.ToArray();
        }

        public override void Set(string key, object value, TimeSpan? expireTime = null, CacheExpireType? expireType = null)
        {
            if (value == null)
            {
                throw new CoreException("Can not insert null values to the cache!");
            }

            _database.StringSet(
                GetLocalizedRedisKey(key),
                Serialize(value, value.GetType()),
                expireTime ?? DefaultExpireTime
                );
        }

        public override void Set<TKey, TValue>(TKey key, TValue value, TimeSpan? expireTime = null, CacheExpireType? expireType = null)
        {
            if (value == null)
            {
                throw new CoreException("Can not insert null values to the cache!");
            }

            _database.StringSet(
                GetLocalizedRedisKey(key.ToString()),
                Serialize(value, typeof(TValue)),
                expireTime ?? DefaultExpireTime
            );
        }

        public override void Set(KeyValuePair<string, object>[] pairs, TimeSpan? expireTime = null, CacheExpireType? expireType = null)
        {
            if (pairs.Any(p => p.Value == null))
            {
                throw new CoreException("Can not insert null values to the cache!");
            }

            var redisPairs = pairs.Select(p => new KeyValuePair<RedisKey, RedisValue>
                (GetLocalizedRedisKey(p.Key),
                    Serialize(p.Value, p.GetType()))
            );

            _database.StringSet(redisPairs.ToArray());
        }


        public override Task SetAsync(string key, object value, TimeSpan? expireTime = null, CacheExpireType? expireType = null)
        {
            if (value == null)
            {
                throw new CoreException("Can not insert null values to the cache!");
            }

            return _database.StringSetAsync(
                GetLocalizedRedisKey(key),
                Serialize(value, value.GetType()),
                expireTime ?? DefaultExpireTime
            );
        }

        public override Task SetAsync<TKey, TValue>(TKey key, TValue value, TimeSpan? expireTime = null, CacheExpireType? expireType = null)
        {
            if (value == null)
            {
                throw new CoreException("Can not insert null values to the cache!");
            }

            return _database.StringSetAsync(
                GetLocalizedRedisKey(key.ToString()),
                Serialize(value, typeof(TValue)),
                expireTime ?? DefaultExpireTime
            );
        }

        public override Task SetAsync(KeyValuePair<string, object>[] pairs, TimeSpan? expireTime = null, CacheExpireType? expireType = null)
        {
            if (pairs.Any(p => p.Value == null))
            {
                throw new CoreException("Can not insert null values to the cache!");
            }

            var redisPairs = pairs.Select(p => new KeyValuePair<RedisKey, RedisValue>
                (GetLocalizedRedisKey(p.Key), Serialize(p.Value, p.GetType()))
            );
            return _database.StringSetAsync(redisPairs.ToArray());
        }

        public override void Remove(string key)
        {
            _database.KeyDelete(GetLocalizedRedisKey(key));
        }

        public override void Remove(string[] keys)
        {
            _database.KeyDelete(GetRedisKeyList(keys));
        }

        public override Task RemoveAsync(string key)
        {
            return _database.KeyDeleteAsync(GetLocalizedRedisKey(key));
        }

        public override async Task RemoveAsync(string[] keys)
        {
            await _database.KeyDeleteAsync(GetRedisKeyList(keys));
        }


        public override void Clear()
        {
            _database.KeyDeleteWithPrefix(GetLocalizedRedisKey("*"));
        }

        protected virtual RedisKey GetLocalizedRedisKey(string key)
        {
            return GetLocalizedKey(key);
        }

        protected virtual string GetLocalizedKey(string key)
        {
            return "n:" + Name + "|f:" + key;
        }

        protected virtual string Serialize(object value, Type type)
        {
            return _serializer.Serialize(value, type);
        }

        protected virtual object Deserialize(RedisValue objbyte)
        {
            return _serializer.Deserialize(objbyte);
        }

        protected virtual T Deserialize<T>(RedisValue objbyte)
        {
            return _serializer.Deserialize<T>(objbyte);
        }

        private RedisKey[] GetRedisKeyList<TKey>(TKey[] keys)
        {
            return keys.Select(a => GetLocalizedRedisKey(a.ToString())).ToArray();
        }
    }
}
