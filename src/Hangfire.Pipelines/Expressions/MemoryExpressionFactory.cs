using System;
using System.Linq;
using System.Linq.Expressions;

using Hangfire.Pipelines.Core;
using Hangfire.Pipelines.Models;
using Hangfire.Pipelines.Storage;

namespace Hangfire.Pipelines.Expressions
{
    public class MemoryExpressionFactory : IExpressionFactory
    {
        private readonly IPipelineStorage _pipelineStorage;

        public MemoryExpressionFactory(IPipelineStorage pipelineStorage)
        {
            _pipelineStorage = pipelineStorage;
        }

        public ExpressionContainer Create<T>(Expression<Action<T>> expression)
        {
            var action = expression.Compile();

            var container = new ExpressionContainer(pipelineId => RunNew(pipelineId, action),
                (pipelineId, parrentId) => RunNew(pipelineId, action));

            return container;
        }

        private string RunNew<T>(Guid pipelineId, Action<T> action)
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

            action.Invoke(activatedJob);

            ContextHelper.TearDown(pipelineContext);

            return Guid.NewGuid().ToString("N");
        }

        private T CreateObject<T>()
        {
            return (T) Activator.CreateInstance(typeof(T));
        }
    }
}