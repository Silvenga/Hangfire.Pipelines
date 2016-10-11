using System;
using System.Linq;
using System.Linq.Expressions;

using Hangfire.Pipelines.Core;
using Hangfire.Pipelines.Helpers;
using Hangfire.Pipelines.Models;
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
            var pipelineTaskType = jobType.GetInterfaces().SingleOrDefault(x =>
                                               x.IsGenericType &&
                                               x.GetGenericTypeDefinition() == typeof(IPipelineTask<>));

            if (pipelineTaskType == null)
            {
                throw new NotSupportedException($"Setup must implement {nameof(IPipelineTask<object>)}");
            }

            var pipelineContext = ContextHelper.CreateContext(pipelineTaskType.GenericTypeArguments, _pipelineStorage, pipelineId);
            ContextHelper.SetContext(jobType, activatedJob, pipelineContext);
            ContextHelper.Setup(pipelineContext);

            expression.Compile().Invoke(activatedJob);

            ContextHelper.TearDown(pipelineContext);

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