using System;

namespace Hangfire.Pipelines.Storage
{
    public interface IPipelineStorage
    {
        T Get<T>(Guid pipelineId, string key);
        void Set(Guid pipelineId, string key, object value);
    }
}