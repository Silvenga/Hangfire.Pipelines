using System;

using Hangfire.ActivationExtensions.Interceptor;
using Hangfire.Pipelines.Storage;

namespace Hangfire.Pipelines.Core
{
    public class HangfireActivatorInterceptor : IJobActivatorFilter
    {
        private readonly IPipelineInterceptor _interceptor;
        private readonly IPipelineStorage _storage;

        public HangfireActivatorInterceptor(IPipelineInterceptor interceptor, IPipelineStorage storage)
        {
            _interceptor = interceptor;
            _storage = storage;
        }

        public void OnMaterializing(Type jobType, JobActivatorContext context)
        {
        }

        public void OnMaterialized(Type jobType, object activatedJob, JobActivatorContext context)
        {
            _interceptor.SetUpContext(jobType, activatedJob, () => context.GetJobParameter<Guid>(Constants.PipelineIdKey), _storage);
        }

        public void OnScopeCreating(JobActivatorContext context)
        {
        }

        public void OnScopeCreated(JobActivatorContext context, JobActivatorScope createdScope)
        {
        }

        public void OnScopeDisposing(Type jobType, object activatedJob, JobActivatorContext context)
        {
        }

        public void OnScopeDisposed(Type jobType, object activatedJob, JobActivatorContext context)
        {
        }
    }
}