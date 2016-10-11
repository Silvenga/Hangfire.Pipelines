using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Hangfire.Pipelines.Models;
using Hangfire.Pipelines.Storage;

namespace Hangfire.Pipelines.Core
{
    public class PipelineDefinition<TEntity>
    {
        private readonly IPipelineStorage _pipelineStorage;
        private readonly IList<ExpressionContainer> _steps = new List<ExpressionContainer>();

        public PipelineDefinition(IPipelineStorage pipelineStorage)
        {
            _pipelineStorage = pipelineStorage;
        }

        public void AddStep<T>(Expression<Action<T>> expression) where T : IPipelineTask<TEntity>
        {
            var container = new ExpressionContainer(
                (executor, pipelineId) => executor.RunNew(expression, pipelineId),
                (executor, pipelineId, parrentId) => executor.RunContinuation(expression, pipelineId, parrentId)
            );
            _steps.Add(container);
        }

        public PipelineExecutor<TEntity> CreateExecutor()
        {
            return new PipelineExecutor<TEntity>(_steps, _pipelineStorage);
        }
    }
}