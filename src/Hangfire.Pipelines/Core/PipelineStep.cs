using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Hangfire.Pipelines.Models;

namespace Hangfire.Pipelines.Core
{
    public class PipelineStep<TStart, TInput>
    {
        private readonly PipelineDefinition<TStart> _definition;

        public PipelineStep(PipelineDefinition<TStart> definition)
        {
            _definition = definition;
        }

        public PipelineStep<TStart, TNext> AddStep<T, TNext>(Expression<Func<T, TNext>> task, string name = null) where T : IPipelineTask<TInput>
        {
            Add(task, name);
            return new PipelineStep<TStart, TNext>(_definition);
        }

        public PipelineStep<TStart, TNext> AddStep<T, TNext>(Expression<Func<T, Task<TNext>>> task, string name = null) where T : IPipelineTask<TInput>
        {
            Add(task, name);
            return new PipelineStep<TStart, TNext>(_definition);
        }

        private void Add<T, TReturn>(Expression<Func<T, TReturn>> expression, string name)
        {
            var nextIndex = _definition.Steps.Count;
            name = name ?? $"Step {nextIndex}";
            var container = new ExpressionContainer(
                (executor, pipelineId) => executor.RunNew(expression, pipelineId, name),
                (executor, pipelineId, parrentId) => executor.RunContinuation(expression, pipelineId, parrentId, name)
            );
            _definition.Steps.Add(container);
        }

        private void Add<T, TReturn>(Expression<Func<T, Task<TReturn>>> expression, string name)
        {
            var nextIndex = _definition.Steps.Count;
            name = name ?? $"Step {nextIndex}";
            var container = new ExpressionContainer(
                (executor, pipelineId) => executor.RunNew(expression, pipelineId, name),
                (executor, pipelineId, parrentId) => executor.RunContinuation(expression, pipelineId, parrentId, name)
            );
            _definition.Steps.Add(container);
        }
    }
}