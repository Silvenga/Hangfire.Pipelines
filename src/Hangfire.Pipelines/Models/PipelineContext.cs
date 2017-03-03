using System;

using Hangfire.Pipelines.Core;
using Hangfire.Pipelines.Storage;

using JetBrains.Annotations;

namespace Hangfire.Pipelines.Models
{
    public interface IPipelineContext
    {
        Guid PipelineId { get; }
        IPipelineStorage Storage { get; }

        [UsedImplicitly]
        object InternalEntity { get; }

        void Load();
    }

    public interface IPipelineContext<out T> : IPipelineContext
    {
        T Entity { get; }
        void Set<TValue>(string key, TValue value);
        TValue Get<TValue>(string key);
    }

    public class PipelineContext<T> : IPipelineContext<T>
    {
        public IPipelineStorage Storage { get; }

        [CanBeNull]
        public T Entity { get; private set; }

        public object InternalEntity => Entity;

        public Guid PipelineId { get; }

        public PipelineContext([NotNull] IPipelineStorage storage, Guid pipelineId)
        {
            Storage = storage;
            PipelineId = pipelineId;
        }

        public void Load()
        {
            Entity = Storage.Get<T>(PipelineId, Constants.PipelineEntityKey);
        }

        public void Set<TValue>(string key, TValue value)
        {
            Storage.Set(PipelineId, key, value);
        }

        public TValue Get<TValue>(string key)
        {
            return Storage.Get<TValue>(PipelineId, key);
        }
    }
}