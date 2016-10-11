using System;

using Hangfire.Pipelines.Models;
using Hangfire.Pipelines.Storage;

using JetBrains.Annotations;

namespace Hangfire.Pipelines.Helpers
{
    public static class ContextHelper
    {
        public static void SetContext(Type jobType, object pipelineTask, object pipelineContext)
        {
            var method = jobType.GetProperty(nameof(IPipelineTask<object>.PipelineContext));
            method.SetValue(pipelineTask, pipelineContext);
        }

        public static object GetContext(Type jobType, object activatedJob)
        {
            var method = jobType.GetProperty(nameof(IPipelineTask<object>.PipelineContext));
            return method.GetValue(activatedJob);
        }

        public static object CreateContext(Type[] typeArgs, [NotNull] IPipelineStorage pipelineStorage, Guid pipelineId)
        {
            var type = typeof(PipelineContext<>);
            var genericType = type.MakeGenericType(typeArgs);
            var instance = Activator.CreateInstance(genericType, pipelineStorage, pipelineId);
            return instance;
        }

        public static void Setup(object pipelineContext)
        {
            var basicContext = (IPipelineContext) pipelineContext;
            basicContext.Load();
        }

        public static void TearDown(object pipelineContext)
        {
            var basicContext = (IPipelineContext) pipelineContext;
            basicContext.Save();
        }
    }
}