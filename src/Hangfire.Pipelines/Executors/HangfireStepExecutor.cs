using System;
using System.Linq.Expressions;
using System.Reflection;

using Hangfire.Common;
using Hangfire.MetaExtensions;
using Hangfire.Pipelines.Core;
using Hangfire.States;

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

        public string RunNew<T, TResult>(Expression<Func<T, TResult>> expression, Guid pipelineId, string stepName)
        {
            var enqueuedState = new EnqueuedState();
            return _client
                .AddOrUpdateMeta(Constants.PipelineIdKey, pipelineId)
                .AddOrUpdateMeta(Constants.StepName, stepName)
                .Create(GetJob<T>(expression), enqueuedState);
        }

        public string RunContinuation<T>(Expression<Action<T>> expression, Guid pipelineId, string parrentId, string stepName)
        {
            return _client
                .AddOrUpdateMeta(Constants.PipelineIdKey, pipelineId)
                .AddOrUpdateMeta(Constants.StepName, stepName)
                .ContinueWith(parrentId, expression);
        }

        public string RunContinuation<T, TResult>(Expression<Func<T, TResult>> expression, Guid pipelineId, string parrentId, string stepName)
        {
            var awaitingState = new AwaitingState(parrentId, new EnqueuedState(), JobContinuationOptions.OnlyOnSucceededState);
            return _client
                .AddOrUpdateMeta(Constants.PipelineIdKey, pipelineId)
                .AddOrUpdateMeta(Constants.StepName, stepName)
                .Create(GetJob<T>(expression), awaitingState);
        }

        private static Job GetJob<T>(LambdaExpression expression)
        {
            // HACK
            var methodInfo = typeof(Job).GetMethod("FromExpression", BindingFlags.Static | BindingFlags.NonPublic);
            return (Job) methodInfo.Invoke(null, new object[] {expression, typeof(T)});
        }

        public void CompletedRun(Guid pipelineId)
        {
        }
    }
}