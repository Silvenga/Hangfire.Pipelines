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
        public CreatePipelineStorage StorageDelegate { get; }
        public CreateStepExecutor ExecutorDelegate { get; }

        public IList<IExpressionContainer> Steps { get; }

        internal PipelineDefinition(IPipelineStorage storage, IStepExecutor executor, [ItemNotNull] IList<IExpressionContainer> steps)
        {
            StorageDelegate = guid => storage;
            ExecutorDelegate = guid => executor;
            Steps = steps;
        }

        public PipelineDefinition(CreatePipelineStorage storage, CreateStepExecutor executor)
        {
            StorageDelegate = storage;
            ExecutorDelegate = executor;
            Steps = new List<IExpressionContainer>();
        }

        public void AddStep<T>(Expression<Action<T>> expression, [CanBeNull] string name = null) where T : IPipelineTask<TEntity>
        {
            var nextIndex = Steps.Count;
            name = name ?? $"Step {nextIndex}";
            var container = new ExpressionContainer(
                (executor, pipelineId) => executor.RunNew(expression, pipelineId, name),
                (executor, pipelineId, parrentId) => executor.RunContinuation(expression, pipelineId, parrentId, name)
            );
            Steps.Add(container);
        }

        public PipelineExecutor<TEntity> CreateExecutor()
        {
            return new PipelineExecutor<TEntity>(Steps, StorageDelegate, ExecutorDelegate);
        }
    }
}