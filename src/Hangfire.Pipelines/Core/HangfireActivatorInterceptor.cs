using System;

using Hangfire.ActivationExtensions.Interceptor;
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
            PipelineInterceptor.SetUpContext(jobType, activatedJob, _storage, () => context.GetJobParameter<Guid>("PipelineId"));
        }

        public void OnScopeCreating(JobActivatorContext context)
        {
        }

        public void OnScopeCreated(JobActivatorContext context, JobActivatorScope createdScope)
        {
        }

        public void OnScopeDisposing(Type jobType, object activatedJob, JobActivatorContext context)
        {
            PipelineInterceptor.TearDownContext(jobType, activatedJob);
        }

        public void OnScopeDisposed(Type jobType, object activatedJob, JobActivatorContext context)
        {
        }
    }
}