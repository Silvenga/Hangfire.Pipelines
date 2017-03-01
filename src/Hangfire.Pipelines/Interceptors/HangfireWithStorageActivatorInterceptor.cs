using System;

using Hangfire.ActivationExtensions.Interceptor;
using Hangfire.Pipelines.Core;
using Hangfire.Pipelines.Storage;

namespace Hangfire.Pipelines.Interceptors
{
    public class HangfireWithStorageActivatorInterceptor : IJobActivatorFilter
    {
        private readonly PipelineInterceptor _interceptor;

        public HangfireWithStorageActivatorInterceptor(PipelineInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        public void OnMaterializing(Type jobType, JobActivatorContext context)
        {
        }

        public void OnMaterialized(Type jobType, object activatedJob, JobActivatorContext context)
        {
            var storage = new HangfirePipelineStorage(context);
            _interceptor.SetUpContext(jobType, activatedJob, () => context.GetJobParameter<Guid>(Constants.PipelineIdKey), storage);
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