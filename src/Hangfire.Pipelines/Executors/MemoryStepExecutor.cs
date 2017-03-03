using System;
using System.Linq.Expressions;

using Hangfire.Pipelines.Core;
using Hangfire.Pipelines.Storage;

namespace Hangfire.Pipelines.Executors
{
    public class MemoryStepExecutor : IStepExecutor
    {
        private readonly IPipelineInterceptor _interceptor;
        private readonly IPipelineStorage _storage;

        public MemoryStepExecutor(IPipelineInterceptor interceptor, IPipelineStorage storage)
        {
            _interceptor = interceptor;
            _storage = storage;
        }

        public void StartedRun(Guid pipelineId)
        {
        }

        public string RunNew<T>(Expression<Action<T>> expression, Guid pipelineId, string stepName)
        {
            return RunInMemory<T, object>(x => ToNullExpression<T, object>(x, expression), pipelineId);
        }

        public string RunNew<T, TResult>(Expression<Func<T, TResult>> expression, Guid pipelineId, string stepName)
        {
            return RunInMemory(expression, pipelineId);
        }

        public string RunContinuation<T>(Expression<Action<T>> expression, Guid pipelineId, string parrentId, string stepName)
        {
            return RunInMemory<T, object>(x => ToNullExpression<T, object>(x, expression), pipelineId);
        }

        public string RunContinuation<T, TResult>(Expression<Func<T, TResult>> expression, Guid pipelineId, string parrentId, string stepName)
        {
            return RunInMemory(expression, pipelineId);
        }

        public void CompletedRun(Guid pipelineId)
        {
        }

        private TResult ToNullExpression<T, TResult>(T obj, Expression<Action<T>> expression)
        {
            expression.Compile().Invoke(obj);
            return default(TResult);
        }

        protected virtual string RunInMemory<T, TResult>(Expression<Func<T, TResult>> expression, Guid pipelineId)
        {
            var activatedJob = CreateObject<T>();
            var jobType = typeof(T);

            _interceptor.SetUpContext(jobType, activatedJob, () => pipelineId, _storage);

            var result = expression.Compile().Invoke(activatedJob);

            _interceptor.TearDownContext(result, _storage, pipelineId);

            return Guid.NewGuid().ToString("N");
        }

        protected virtual T CreateObject<T>()
        {
            return (T) Activator.CreateInstance(typeof(T));
        }
    }
}