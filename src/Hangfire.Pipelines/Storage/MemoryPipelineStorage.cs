using System;
using System.Collections.Concurrent;

namespace Hangfire.Pipelines.Storage
{
    public class MemoryPipelineStorage : IPipelineStorage
    {
        private readonly ConcurrentDictionary<Tuple<Guid, string>, object> _values = new ConcurrentDictionary<Tuple<Guid, string>, object>();

        public T Get<T>(Guid pipelineId, string key)
        {
            var lookup = new Tuple<Guid, string>(pipelineId, key);
            object o;
            var success = _values.TryGetValue(lookup, out o);
            return success ? (T) o : default(T);
        }

        public void Set(Guid pipelineId, string key, object value)
        {
            var lookup = new Tuple<Guid, string>(pipelineId, key);
            _values.AddOrUpdate(lookup, value, (x, v) => value);
        }
    }
}