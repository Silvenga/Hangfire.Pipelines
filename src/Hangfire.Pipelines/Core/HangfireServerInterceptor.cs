using System;

using Hangfire.Pipelines.Storage;
using Hangfire.Server;

namespace Hangfire.Pipelines.Core
{
    public class HangfireServerInterceptor : IServerFilter
    {
        private readonly IPipelineInterceptor _interceptor;
        private readonly IPipelineStorage _storage;

        public HangfireServerInterceptor(IPipelineInterceptor interceptor, IPipelineStorage storage)
        {
            _interceptor = interceptor;
            _storage = storage;
        }

        public void OnPerforming(PerformingContext filterContext)
        {
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            var result = filterContext.Result;
            _interceptor.TearDownContext(result, _storage, filterContext.GetJobParameter<Guid>(Constants.PipelineIdKey));
        }
    }
}