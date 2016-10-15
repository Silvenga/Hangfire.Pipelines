using System;
using System.Linq;

using Hangfire.Pipelines.Models;
using Hangfire.Pipelines.Storage;

using JetBrains.Annotations;

namespace Hangfire.Pipelines.Core
{
    public class PipelineInterceptor
    {
        public static void SetUpContext(Type jobType, object activatedJob, IPipelineStorage storage, Func<Guid> getPipelineId)
        {
            var pipelineTaskType = GetPipelineTaskType(jobType);
            if (pipelineTaskType == null)
            {
                return;
            }

            var id = getPipelineId();

            var pipelineContext = CreateContext(pipelineTaskType.GenericTypeArguments, storage, id);
            SetContext(jobType, activatedJob, pipelineContext);
            Setup(pipelineContext);
        }

        public static void TearDownContext(Type jobType, object activatedJob)
        {
            var pipelineTaskType = GetPipelineTaskType(jobType);
            if (pipelineTaskType == null)
            {
                return;
            }

            var pipelineContext = GetContext(jobType, activatedJob);
            TearDown(pipelineContext);
        }

        [CanBeNull]
        private static Type GetPipelineTaskType(Type jobType)
        {
            return jobType.GetInterfaces().SingleOrDefault(x =>
                               x.IsGenericType &&
                               x.GetGenericTypeDefinition() == typeof(IPipelineTask<>));
        }

        private static void SetContext(Type jobType, object pipelineTask, object pipelineContext)
        {
            var method = jobType.GetProperty(nameof(IPipelineTask<object>.PipelineContext));
            method.SetValue(pipelineTask, pipelineContext);
        }

        private static object GetContext(Type jobType, object activatedJob)
        {
            var method = jobType.GetProperty(nameof(IPipelineTask<object>.PipelineContext));
            return method.GetValue(activatedJob);
        }

        private static object CreateContext(Type[] typeArgs, [NotNull] IPipelineStorage storage, Guid pipelineId)
        {
            var type = typeof(PipelineContext<>);
            var genericType = type.MakeGenericType(typeArgs);
            var instance = Activator.CreateInstance(genericType, storage, pipelineId);
            return instance;
        }

        private static void Setup(object pipelineContext)
        {
            var basicContext = (IPipelineContext) pipelineContext;
            basicContext.Load();
        }

        private static void TearDown(object pipelineContext)
        {
            var basicContext = (IPipelineContext) pipelineContext;
            basicContext.Save();
        }
    }
}