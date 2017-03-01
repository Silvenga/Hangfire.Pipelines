using System;
using System.Linq.Expressions;

namespace Hangfire.Pipelines.Executors
{
    public interface IStepExecutor
    {
        void StartedRun(Guid pipelineId);
        string RunNew<T>(Expression<Action<T>> expression, Guid pipelineId, string stepName);
        string RunContinuation<T>(Expression<Action<T>> expression, Guid pipelineId, string parrentId, string stepName);
        void CompletedRun(Guid pipelineId);
    }
}