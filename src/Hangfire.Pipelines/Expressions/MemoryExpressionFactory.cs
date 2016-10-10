using System;
using System.Linq.Expressions;

namespace Hangfire.Pipelines.Expressions
{
    public class MemoryExpressionFactory : IExpressionFactory
    {
        public ExpressionContainer Create<T>(Expression<Action<T>> expression)
        {
            var action = expression.Compile();

            var container = new ExpressionContainer(pipelineId => RunNew(pipelineId, action),
                (pipelineId, parrentId) => RunContinuation(pipelineId, parrentId, action));

            throw new NotImplementedException();
        }

        private string RunNew<T>(Guid pipelineId, Action<T> action)
        {
            throw new NotImplementedException();
        }

        private string RunContinuation<T>(Guid pipelineId, string parrentId, Action<T> action)
        {
            throw new NotImplementedException();
        }
    }
}