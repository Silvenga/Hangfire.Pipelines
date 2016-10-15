using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Hangfire.Pipelines.Executors;
using Hangfire.Pipelines.Storage;

using JetBrains.Annotations;

namespace Hangfire.Pipelines.Core
{
    public class PipelineExecutor<TEntity>
    {
        public IReadOnlyList<IExpressionContainer> Steps { get; }
        public IPipelineStorage Storage { get; }
        public IStepExecutor Executor { get; }

        public PipelineExecutor(IList<IExpressionContainer> steps, IPipelineStorage storage, IStepExecutor executor)
        {
            Steps = new ReadOnlyCollection<IExpressionContainer>(steps);
            Storage = storage;
            Executor = executor;
        }

        public Guid Process(TEntity entity, [CanBeNull] IStepExecutor executor = null)
        {
            var id = Guid.NewGuid();

            Storage.Set(id, "PipelineEntity", entity);

            var first = Steps.First();
            var lastId = first.StartNew(Executor, id);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var invoker in Steps.Skip(1))
            {
                lastId = invoker.StartContinuation(executor ?? Executor, id, lastId);
            }

            return id;
        }
    }
}