using System;

using Hangfire.ActivationExtensions.Interceptor;

namespace Hangfire.Pipelines.Core
{
    public class HangfireActivatorInterceptor : IJobActivatorFilter
    {
        private readonly PipelineInterceptor _interceptor;

        public HangfireActivatorInterceptor(PipelineInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        public void OnMaterializing(Type jobType, JobActivatorContext context)
        {
        }

        public void OnMaterialized(Type jobType, object activatedJob, JobActivatorContext context)
        {
            _interceptor.SetUpContext(jobType, activatedJob, () => context.GetJobParameter<Guid>(Constants.PipelineIdKey));
        }

        public void OnScopeCreating(JobActivatorContext context)
        {
        }

        public void OnScopeCreated(JobActivatorContext context, JobActivatorScope createdScope)
        {
        }

        public void OnScopeDisposing(Type jobType, object activatedJob, JobActivatorContext context)
        {
            _interceptor.TearDownContext(jobType, activatedJob);
        }

        public void OnScopeDisposed(Type jobType, object activatedJob, JobActivatorContext context)
        {
        }
    }
}