using System;
using System.Linq.Expressions;

using Hangfire.Pipelines.Core;

namespace Hangfire.Pipelines.Executors
{
    public class MemoryStepExecutor : IStepExecutor
    {
        private readonly PipelineInterceptor _interceptor;

        public MemoryStepExecutor(PipelineInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        public string RunNew<T>(Expression<Action<T>> expression, Guid pipelineId)
        {
            return RunInMemory(expression, pipelineId);
        }

        public string RunContinuation<T>(Expression<Action<T>> expression, Guid pipelineId, string parrentId)
        {
            return RunInMemory(expression, pipelineId);
        }

        protected virtual string RunInMemory<T>(Expression<Action<T>> expression, Guid pipelineId)
        {
            var activatedJob = CreateObject<T>();
            var jobType = typeof(T);

            _interceptor.SetUpContext(jobType, activatedJob, () => pipelineId);

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