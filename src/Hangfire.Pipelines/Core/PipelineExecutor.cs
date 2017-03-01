using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using JetBrains.Annotations;

namespace Hangfire.Pipelines.Core
{
    public class PipelineExecutor<TEntity>
    {
        public ImmutableArray<IExpressionContainer> Steps { get; }
        public CreatePipelineStorage StorageDelegate { get; }
        public CreateStepExecutor ExecutorDelegate { get; }

        public PipelineExecutor([NotNull] IEnumerable<IExpressionContainer> steps, [NotNull] CreatePipelineStorage storageDelegate,
                                [NotNull] CreateStepExecutor executorDelegate)
        {
            Steps = steps.ToImmutableArray();
            StorageDelegate = storageDelegate;
            ExecutorDelegate = executorDelegate;

            if (!Steps.Any())
            {
                throw new NotSupportedException($"A {nameof(PipelineExecutor<TEntity>)} must have steps to be constructed.");
            }
        }

        public Guid Process([NotNull] TEntity entity, [CanBeNull] CreateStepExecutor executor = null)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var id = Guid.NewGuid();
            var localExecutor = (executor ?? ExecutorDelegate).Invoke(id);
            var localStorage = StorageDelegate.Invoke(id);

            localExecutor.StartedRun(id);

            localStorage.Set(id, Constants.PipelineEntityKey, entity);

            var first = Steps.First();
            var lastId = first.StartNew(localExecutor, id);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var invoker in Steps.Skip(1))
            {
                lastId = invoker.StartContinuation(localExecutor, id, lastId);
            }
            localExecutor.CompletedRun(id);

            return id;
        }
    }
}