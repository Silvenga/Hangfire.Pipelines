using System;

using Newtonsoft.Json;

using StackExchange.Redis;

namespace Hangfire.Pipelines.Storage.Redis
{
    public class RedisPipelineStorage : IPipelineStorage
    {
        public T Get<T>(Guid pipelineId, string key)
        {
            throw new NotImplementedException();
        }

        public void Set(Guid pipelineId, string key, object value)
        {
            throw new NotImplementedException();
        }
    }

    public class RedisDriver : IDisposable
    {
        private readonly int _db;
        private readonly string _prefix;
        private readonly ConnectionMultiplexer _redis;

        public RedisDriver(string connectionStr, int db, string prefix)
        {
            _db = db;
            _prefix = prefix;
            _redis = ConnectionMultiplexer.Connect(connectionStr);
        }

        public IDatabase GetDatabase()
        {
            return _redis.GetDatabase(_db);
        }

        public void Set(string id, string key, object value)
        {
            var json = ToJson(value);
            var database = GetDatabase();
            database.HashSet($"{_prefix}:{id}", key, json);
        }

        private string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        private T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void Dispose()
        {
            _redis.Dispose();
        }
    }
}