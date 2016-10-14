using System;

using Hangfire.Pipelines.Executors;

namespace Hangfire.Pipelines.Core
{
    public interface IExpressionContainer
    {
        string StartNew(IStepExecutor executor, Guid pipelineId);
        string StartContinuation(IStepExecutor executor, string parrentId, Guid pipelineId);
    }

    public class ExpressionContainer : IExpressionContainer
    {
        private readonly Func<IStepExecutor, Guid, string> _startNew;
        private readonly Func<IStepExecutor, Guid, string, string> _startContinuation;

        public ExpressionContainer(Func<IStepExecutor, Guid, string> startNew, Func<IStepExecutor, Guid, string, string> startContinuation)
        {
            _startNew = startNew;
            _startContinuation = startContinuation;
        }

        public string StartNew(IStepExecutor executor, Guid pipelineId)
        {
            return _startNew.Invoke(executor, pipelineId);
        }

        public string StartContinuation(IStepExecutor executor, string parrentId, Guid pipelineId)
        {
            return _startContinuation.Invoke(executor, pipelineId, parrentId);
        }
    }
}