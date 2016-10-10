using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Hangfire.Pipelines.Expressions;
using Hangfire.Pipelines.Models;
using Hangfire.Pipelines.Storage;

namespace Hangfire.Pipelines.Core
{
    public class PipelineDefinition<TEntity>
    {
        private readonly IExpressionFactory _expressionFactory;
        private readonly IPipelineStorage _pipelineStorage;

        private readonly IList<ExpressionContainer> _steps = new List<ExpressionContainer>();

        public PipelineDefinition(IExpressionFactory expressionFactory, IPipelineStorage pipelineStorage)
        {
            _expressionFactory = expressionFactory;
            _pipelineStorage = pipelineStorage;
        }

        public void AddStep<T>(Expression<Action<T>> expression) where T : IPipelineTask<TEntity>
        {
            var container = _expressionFactory.Create(expression);
            _steps.Add(container);
        }

        public Guid Process(TEntity entity)
        {
            var id = Guid.NewGuid();

            _pipelineStorage.Set(id, "PipelineEntity", entity);

            var cache = _steps.ToList();

            var first = cache.FirstOrDefault();
            if (first == null)
            {
                throw new NotSupportedException($"Use {nameof(AddStep)} to add steps before calling this method.");
            }

            var lastId = first.StartNew(id);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var invoker in _steps.Skip(1))
            {
                lastId = invoker.StartContinuation(lastId, id);
            }

            return id;
        }
    }
}