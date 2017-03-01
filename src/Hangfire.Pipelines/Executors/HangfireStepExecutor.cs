using System;
using System.Linq.Expressions;

using Hangfire.MetaExtensions;
using Hangfire.Pipelines.Core;

namespace Hangfire.Pipelines.Executors
{
    public class HangfireStepExecutor : IStepExecutor
    {
        private readonly IBackgroundJobClient _client;

        public HangfireStepExecutor(IBackgroundJobClient client)
        {
            _client = client;
        }

        public void StartedRun(Guid pipelineId)
        {
        }

        public string RunNew<T>(Expression<Action<T>> expression, Guid pipelineId, string stepName)
        {
            return _client
                .AddOrUpdateMeta(Constants.PipelineIdKey, pipelineId)
                .AddOrUpdateMeta(Constants.StepName, stepName)
                .Enqueue(expression);
        }

        public string RunContinuation<T>(Expression<Action<T>> expression, Guid pipelineId, string parrentId, string stepName)
        {
            return _client
                .AddOrUpdateMeta(Constants.PipelineIdKey, pipelineId)
                .AddOrUpdateMeta(Constants.StepName, stepName)
                .ContinueWith(parrentId, expression);
        }

        public void CompletedRun(Guid pipelineId)
        {
        }
    }
}