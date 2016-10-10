using System;
using System.Linq.Expressions;

using Hangfire.MetaExtensions;

namespace Hangfire.Pipelines.Expressions
{
    public class HangfireExpressionFactory : IExpressionFactory
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public HangfireExpressionFactory(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public ExpressionContainer Create<T>(Expression<Action<T>> expression)
        {
            return new ExpressionContainer(CreateNew(expression), CreateContinuation(expression));
        }

        private Func<Guid, string> CreateNew<T>(Expression<Action<T>> expression)
        {
            return pipelineId => _backgroundJobClient.AddOrUpdateMeta("PipelineId", pipelineId).Enqueue(expression);
        }

        private Func<Guid, string, string> CreateContinuation<T>(Expression<Action<T>> expression)
        {
            return (pipelineId, parrentId) => _backgroundJobClient.AddOrUpdateMeta("PipelineId", pipelineId).ContinueWith(parrentId, expression);
        }
    }
}