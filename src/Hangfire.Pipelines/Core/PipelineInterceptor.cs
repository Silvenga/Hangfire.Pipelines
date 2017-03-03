using System;
using System.Linq;

using Hangfire.Pipelines.Models;
using Hangfire.Pipelines.Storage;

using JetBrains.Annotations;

namespace Hangfire.Pipelines.Core
{
    public interface IPipelineInterceptor
    {
        void SetUpContext(Type jobType, object task, IPipelineStorage storage, Func<Guid> getPipelineId, Func<string> pipelineName, Func<string> stepName);

        void TearDownContext(object result, IPipelineStorage storage, Guid pipelineId);
    }

    public class PipelineInterceptor : IPipelineInterceptor
    {
        public virtual void SetUpContext(Type jobType, object task, IPipelineStorage storage, Func<Guid> getPipelineId, Func<string> pipelineName, Func<string> stepName)
        {
            var pipelineTaskType = GetPipelineTaskType(jobType);
            if (pipelineTaskType == null)
            {
                return;
            }

            var id = getPipelineId();

            var pipelineContext = CreateContext(pipelineTaskType.GenericTypeArguments, storage, id);
            SetContext(jobType, task, pipelineContext);
            pipelineContext.Load();
        }

        public virtual void TearDownContext(object result, IPipelineStorage storage, Guid pipelineId)
        {
            storage.Set(pipelineId, Constants.PipelineEntityKey, result);
        }

        [CanBeNull]
        private Type GetPipelineTaskType(Type jobType)
        {
            return jobType.GetInterfaces().SingleOrDefault(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IPipelineTask<>));
        }

        private void SetContext(Type jobType, object task, IPipelineContext pipelineContext)
        {
            var method = jobType.GetProperty(nameof(IPipelineTask<object>.PipelineContext));
            method.SetValue(task, pipelineContext);
        }

        private IPipelineContext CreateContext(Type[] typeArgs, [NotNull] IPipelineStorage storage, Guid pipelineId)
        {
            var type = typeof(PipelineContext<>);
            var genericType = type.MakeGenericType(typeArgs);
            var instance = Activator.CreateInstance(genericType, storage, pipelineId);
            return (IPipelineContext) instance;
        }
    }
}