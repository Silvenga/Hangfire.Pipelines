using System;
using System.Linq.Expressions;

namespace Hangfire.Pipelines.Executors
{
    public interface IStepExecutor
    {
        void StartedRun(Guid pipelineId);
        string RunNew<T>(Expression<Action<T>> expression, Guid pipelineId);
        string RunContinuation<T>(Expression<Action<T>> expression, Guid pipelineId, string parrentId);
        void CompletedRun(Guid pipelineId);
    }
}