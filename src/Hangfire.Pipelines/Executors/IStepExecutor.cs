using System;
using System.Linq.Expressions;

namespace Hangfire.Pipelines.Executors
{
    public interface IStepExecutor
    {
        void StartedRun(Guid pipelineId);
        string RunNew<T>(Expression<Action<T>> expression, Guid pipelineId, string pipelineName, string stepName);
        string RunNew<T, TResult>(Expression<Func<T, TResult>> expression, Guid pipelineId, string pipelineName, string stepName);
        string RunContinuation<T>(Expression<Action<T>> expression, Guid pipelineId, string parrentId, string pipelineName, string stepName);
        string RunContinuation<T, TResult>(Expression<Func<T, TResult>> expression, Guid pipelineId, string parrentId, string pipelineName, string stepName);
        void CompletedRun(Guid pipelineId);
    }
}