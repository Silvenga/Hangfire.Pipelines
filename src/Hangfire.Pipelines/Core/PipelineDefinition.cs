using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Hangfire.Pipelines.Executors;
using Hangfire.Pipelines.Models;
using Hangfire.Pipelines.Storage;

using JetBrains.Annotations;

namespace Hangfire.Pipelines.Core
{
    public class PipelineDefinition<TStart>
    {
        public CreatePipelineStorage StorageDelegate { get; }
        public CreateStepExecutor ExecutorDelegate { get; }

        internal IList<IExpressionContainer> Steps { get; }

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

        public PipelineStep<TStart, TNext> AddStep<T, TNext>(Expression<Func<T, TNext>> expression, [CanBeNull] string name = null)
            where T : IPipelineTask<TStart>
        {
            return new PipelineStep<TStart, TStart>(this).AddStep(expression, name);
        }

        public PipelineStep<TStart, TNext> AddStep<T, TNext>(Expression<Func<T, Task<TNext>>> expression, [CanBeNull] string name = null)
            where T : IPipelineTask<TStart>
        {
            return new PipelineStep<TStart, TStart>(this).AddStep(expression, name);
        }

        public PipelineExecutor<TStart> CreateExecutor()
        {
            return new PipelineExecutor<TStart>(Steps, StorageDelegate, ExecutorDelegate);
        }
    }
}