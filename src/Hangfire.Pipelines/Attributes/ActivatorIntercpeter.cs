using System;
using System.Linq;

using Hangfire.ActivationExtensions.Interceptor;
using Hangfire.Pipelines.Models;
using Hangfire.Pipelines.Storage;

using JetBrains.Annotations;

namespace Hangfire.Pipelines.Attributes
{
    public class ActivatorInterceptor : IJobActivatorFilter
    {
        [NotNull] private readonly IPipelineStorage _pipelineStorage;

        public ActivatorInterceptor([NotNull] IPipelineStorage pipelineStorage)
        {
            _pipelineStorage = pipelineStorage;
        }

        public void OnMaterializing(Type jobType, JobActivatorContext context)
        {
        }

        public void OnMaterialized(Type jobType, object activatedJob, JobActivatorContext context)
        {
            var pipelineTaskType = jobType.GetInterfaces().SingleOrDefault(x =>
                                               x.IsGenericType &&
                                               x.GetGenericTypeDefinition() == typeof(IPipelineTask<>));

            if (pipelineTaskType == null)
            {
                return;
            }

            var id = context.GetJobParameter<Guid>("PipelineId");
            var pipelineContext = CreateContext(pipelineTaskType.GenericTypeArguments, _pipelineStorage, id);
            SetContext(jobType, activatedJob, pipelineContext);

            var basicContext = (IBasicPipelineContext) pipelineContext;
            basicContext.Load();
        }

        private void SetContext(Type jobType, object activatedJob, object pipelineContext)
        {
            var method = jobType.GetProperty(nameof(IPipelineTask<object>.PipelineContext));
            method.SetValue(activatedJob, pipelineContext);
        }

        private object GetContext(Type jobType, object activatedJob)
        {
            var method = jobType.GetProperty(nameof(IPipelineTask<object>.PipelineContext));
            return method.GetValue(activatedJob);
        }

        private object CreateContext(Type[] typeArgs, [NotNull] IPipelineStorage pipelineStorage, Guid pipelineId)
        {
            var type = typeof(PipelineContext<>);
            var genericType = type.MakeGenericType(typeArgs);
            var instance = Activator.CreateInstance(genericType, pipelineStorage, pipelineId);
            return instance;
        }

        public void OnScopeCreating(JobActivatorContext context)
        {
        }

        public void OnScopeCreated(JobActivatorContext context, JobActivatorScope createdScope)
        {
        }

        public void OnScopeDisposing(Type jobType, object activatedJob, JobActivatorContext context)
        {
            var isPipelineTask = jobType.GetInterfaces().SingleOrDefault(x =>
                                             x.IsGenericType &&
                                             x.GetGenericTypeDefinition() == typeof(IPipelineTask<>));

            if (isPipelineTask == null)
            {
                return;
            }

            var pipelineContext = GetContext(isPipelineTask, activatedJob);
            var basicContext = (IBasicPipelineContext) pipelineContext;
            basicContext.Save();
        }

        public void OnScopeDisposed(Type jobType, object activatedJob, JobActivatorContext context)
        {
        }
    }
}