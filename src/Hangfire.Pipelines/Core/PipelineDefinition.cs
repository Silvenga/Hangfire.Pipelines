using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Hangfire.Pipelines.Executors;
using Hangfire.Pipelines.Models;
using Hangfire.Pipelines.Storage;

using JetBrains.Annotations;

namespace Hangfire.Pipelines.Core
{
    public class PipelineDefinition<TEntity>
    {
        public IPipelineStorage Storage { get; }
        public IList<IExpressionContainer> Steps { get; }
        public IStepExecutor Executor { get; }

        public PipelineDefinition(IPipelineStorage storage, IStepExecutor executor, [CanBeNull, ItemNotNull] IList<IExpressionContainer> steps = null)
        {
            Storage = storage;
            Executor = executor;
            Steps = steps ?? new List<IExpressionContainer>();
        }

        public void AddStep<T>(Expression<Action<T>> expression) where T : IPipelineTask<TEntity>
        {
            var container = new ExpressionContainer(
                (executor, pipelineId) => executor.RunNew(expression, pipelineId),
                (executor, pipelineId, parrentId) => executor.RunContinuation(expression, pipelineId, parrentId)
            );
            Steps.Add(container);
        }

        public PipelineExecutor<TEntity> CreateExecutor()
        {
            return new PipelineExecutor<TEntity>(Steps, Storage, Executor);
        }
    }
}