using System;
using System.Linq.Expressions;

using Hangfire.Pipelines.Core;
using Hangfire.Pipelines.Storage;

namespace Hangfire.Pipelines.Executors
{
    public class MemoryStepExecutor : IStepExecutor
    {
        private readonly IPipelineStorage _pipelineStorage;

        public MemoryStepExecutor(IPipelineStorage pipelineStorage)
        {
            _pipelineStorage = pipelineStorage;
        }

        public string RunNew<T>(Expression<Action<T>> expression, Guid pipelineId)
        {
            var activatedJob = CreateObject<T>();
            var jobType = typeof(T);

            PipelineInterceptor.SetUpContext(jobType, activatedJob, _pipelineStorage, () => pipelineId);

            expression.Compile().Invoke(activatedJob);

            PipelineInterceptor.TearDownContext(jobType, activatedJob);

            return Guid.NewGuid().ToString("N");
        }

        public string RunContinuation<T>(Expression<Action<T>> expression, Guid pipelineId, string parrentId)
        {
            return RunNew(expression, pipelineId);
        }

        private T CreateObject<T>()
        {
            return (T) Activator.CreateInstance(typeof(T));
        }
    }
}