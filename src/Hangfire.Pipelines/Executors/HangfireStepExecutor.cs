using System;
using System.Linq.Expressions;

using Hangfire.MetaExtensions;

namespace Hangfire.Pipelines.Executors
{
    public class HangfireStepExecutor : IStepExecutor
    {
        private readonly IBackgroundJobClient _client;

        public HangfireStepExecutor(IBackgroundJobClient client)
        {
            _client = client;
        }

        public string RunNew<T>(Expression<Action<T>> expression, Guid pipelineId)
        {
            return _client.AddOrUpdateMeta("PipelineId", pipelineId).Enqueue(expression);
        }

        public string RunContinuation<T>(Expression<Action<T>> expression, Guid pipelineId, string parrentId)
        {
            return _client.AddOrUpdateMeta("PipelineId", pipelineId).ContinueWith(parrentId, expression);
        }
    }
}