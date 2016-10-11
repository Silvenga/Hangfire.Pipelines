using System;
using System.Linq;

using Hangfire.ActivationExtensions.Interceptor;
using Hangfire.Pipelines.Helpers;
using Hangfire.Pipelines.Models;
using Hangfire.Pipelines.Storage;

using JetBrains.Annotations;

namespace Hangfire.Pipelines.Core
{
    public class HangfireActivatorInterceptor : IJobActivatorFilter
    {
        [NotNull] private readonly IPipelineStorage _pipelineStorage;

        public HangfireActivatorInterceptor([NotNull] IPipelineStorage pipelineStorage)
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
            var pipelineContext = ContextHelper.CreateContext(pipelineTaskType.GenericTypeArguments, _pipelineStorage, id);
            ContextHelper.SetContext(jobType, activatedJob, pipelineContext);
            ContextHelper.Setup(pipelineContext);
        }

        public void OnScopeCreating(JobActivatorContext context)
        {
        }

        public void OnScopeCreated(JobActivatorContext context, JobActivatorScope createdScope)
        {
        }

        public void OnScopeDisposing(Type jobType, object activatedJob, JobActivatorContext context)
        {
            var pipelineTaskType = jobType.GetInterfaces().SingleOrDefault(x =>
                                             x.IsGenericType &&
                                             x.GetGenericTypeDefinition() == typeof(IPipelineTask<>));

            if (pipelineTaskType == null)
            {
                return;
            }

            var pipelineContext = ContextHelper.GetContext(jobType, activatedJob);
            ContextHelper.TearDown(pipelineContext);
        }

        public void OnScopeDisposed(Type jobType, object activatedJob, JobActivatorContext context)
        {
        }
    }
}