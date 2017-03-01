using System;

namespace Hangfire.Pipelines.Storage
{
    public class HangfirePipelineStorage : IPipelineStorage
    {
        private readonly JobActivatorContext _context;

        public HangfirePipelineStorage(JobActivatorContext context)
        {
            _context = context;
        }

        public T Get<T>(Guid pipelineId, string key)
        {
            return _context.GetJobParameter<T>(key);
        }

        public void Set(Guid pipelineId, string key, object value)
        {
            _context.SetJobParameter(key, value);
        }
    }
}