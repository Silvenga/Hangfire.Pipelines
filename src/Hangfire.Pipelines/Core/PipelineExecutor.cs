using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Hangfire.Pipelines.Executors;
using Hangfire.Pipelines.Storage;

namespace Hangfire.Pipelines.Core
{
    public class PipelineExecutor<TEntity>
    {
        private readonly IReadOnlyList<IExpressionContainer> _steps;
        private readonly IPipelineStorage _pipelineStorage;

        public PipelineExecutor(IList<IExpressionContainer> steps, IPipelineStorage pipelineStorage)
        {
            _steps = new ReadOnlyCollection<IExpressionContainer>(steps);
            _pipelineStorage = pipelineStorage;
        }

        public Guid Process(TEntity entity, IStepExecutor executor)
        {
            var id = Guid.NewGuid();

            _pipelineStorage.Set(id, "PipelineEntity", entity);

            var first = _steps.First();
            var lastId = first.StartNew(executor, id);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var invoker in _steps.Skip(1))
            {
                lastId = invoker.StartContinuation(executor, lastId, id);
            }

            return id;
        }
    }
}