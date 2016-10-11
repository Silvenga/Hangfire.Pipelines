using System;
using System.Linq.Expressions;

namespace Hangfire.Pipelines.Executors
{
    public interface IStepExecutor
    {
        string RunNew<T>(Expression<Action<T>> expression, Guid pipelineId);
        string RunContinuation<T>(Expression<Action<T>> expression, Guid pipelineId, string parrentId);
    }
}