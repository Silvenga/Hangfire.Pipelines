using System;

namespace Hangfire.Pipelines.Expressions
{
    public class ExpressionContainer
    {
        private readonly Func<Guid, string> _startNew;
        private readonly Func<Guid, string, string> _startContinuation;

        public ExpressionContainer(Func<Guid, string> startNew, Func<Guid, string, string> startContinuation)
        {
            _startNew = startNew;
            _startContinuation = startContinuation;
        }

        public string StartNew(Guid pipelineId)
        {
            return _startNew.Invoke(pipelineId);
        }

        public string StartContinuation(string parrentId, Guid pipelineId)
        {
            return _startContinuation.Invoke(pipelineId, parrentId);
        }
    }
}