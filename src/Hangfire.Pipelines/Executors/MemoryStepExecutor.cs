using System;
using System.Linq.Expressions;

using Hangfire.Pipelines.Core;
using Hangfire.Pipelines.Storage;

namespace Hangfire.Pipelines.Executors
{
    public class MemoryStepExecutor : IStepExecutor
    {
        private readonly PipelineInterceptor _interceptor;
        private readonly IPipelineStorage _storage;

        public MemoryStepExecutor(PipelineInterceptor interceptor, IPipelineStorage storage)
        {
            _interceptor = interceptor;
            _storage = storage;
        }

        public void StartedRun(Guid pipelineId)
        {
        }

        public string RunNew<T>(Expression<Action<T>> expression, Guid pipelineId, string stepName)
        {
            return RunInMemory(expression, pipelineId);
        }

        public string RunContinuation<T>(Expression<Action<T>> expression, Guid pipelineId, string parrentId, string stepName)
        {
            return RunInMemory(expression, pipelineId);
        }

        public void CompletedRun(Guid pipelineId)
        {
        }

        protected virtual string RunInMemory<T>(Expression<Action<T>> expression, Guid pipelineId)
        {
            var activatedJob = CreateObject<T>();
            var jobType = typeof(T);

            _interceptor.SetUpContext(jobType, activatedJob, () => pipelineId, _storage);

            expression.Compile().Invoke(activatedJob);

            _interceptor.TearDownContext(jobType, activatedJob);

            return Guid.NewGuid().ToString("N");
        }

        protected virtual T CreateObject<T>()
        {
            return (T) Activator.CreateInstance(typeof(T));
        }
    }
}