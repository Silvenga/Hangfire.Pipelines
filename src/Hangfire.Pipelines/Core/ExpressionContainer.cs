using System;

using Hangfire.Pipelines.Executors;

namespace Hangfire.Pipelines.Core
{
    public interface IExpressionContainer
    {
        string StartNew(IStepExecutor executor, Guid pipelineId);
        string StartContinuation(IStepExecutor executor, Guid pipelineId, string parrentId);
    }

    public class ExpressionContainer : IExpressionContainer
    {
        private readonly string _name;
        private readonly Func<IStepExecutor, Guid, string> _startNew;
        private readonly Func<IStepExecutor, Guid, string, string> _startContinuation;

        public ExpressionContainer(string name, Func<IStepExecutor, Guid, string> startNew, Func<IStepExecutor, Guid, string, string> startContinuation)
        {
            _name = name;
            _startNew = startNew;
            _startContinuation = startContinuation;
        }

        public string StartNew(IStepExecutor executor, Guid pipelineId)
        {
            return _startNew.Invoke(executor, pipelineId);
        }

        public string StartContinuation(IStepExecutor executor, Guid pipelineId, string parrentId)
        {
            return _startContinuation.Invoke(executor, pipelineId, parrentId);
        }

        public override string ToString()
        {
            return $"{_name}";
        }
    }
}