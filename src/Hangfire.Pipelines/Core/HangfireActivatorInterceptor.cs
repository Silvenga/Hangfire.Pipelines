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
        [NotNull] private readonly IPipelineStorage _storage;

        public HangfireActivatorInterceptor([NotNull] IPipelineStorage storage)
        {
            _storage = storage;
        }

        public void OnMaterializing(Type jobType, JobActivatorContext context)
        {
        }

        public void OnMaterialized(Type jobType, object activatedJob, JobActivatorContext context)
        {
            SetUpContext(jobType, activatedJob, () => context.GetJobParameter<Guid>("PipelineId"));
        }

        public void OnScopeCreating(JobActivatorContext context)
        {
        }

        public void OnScopeCreated(JobActivatorContext context, JobActivatorScope createdScope)
        {
        }

        public void OnScopeDisposing(Type jobType, object activatedJob, JobActivatorContext context)
        {
            TearDownContext(jobType, activatedJob);
        }

        public void OnScopeDisposed(Type jobType, object activatedJob, JobActivatorContext context)
        {
        }

        public void SetUpContext(Type jobType, object activatedJob, Func<Guid> getPipelineId)
        {
            var pipelineTaskType = jobType.GetInterfaces().SingleOrDefault(x =>
                                               x.IsGenericType &&
                                               x.GetGenericTypeDefinition() == typeof(IPipelineTask<>));

            if (pipelineTaskType == null)
            {
                return;
            }

            var id = getPipelineId();

            var pipelineContext = ContextHelper.CreateContext(pipelineTaskType.GenericTypeArguments, _storage, id);
            ContextHelper.SetContext(jobType, activatedJob, pipelineContext);
            ContextHelper.Setup(pipelineContext);
        }

        private static void TearDownContext(Type jobType, object activatedJob)
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
    }
}