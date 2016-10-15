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

        void Load();
        void Save();
    }

    public interface IPipelineContext<T> : IPipelineContext
    {
        T Entity { get; set; }
    }

    public class PipelineContext<T> : IPipelineContext<T>
    {
        public IPipelineStorage Storage { get; }

        [CanBeNull]
        public T Entity { get; set; }

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

        public void Save()
        {
            Storage.Set(PipelineId, Constants.PipelineEntityKey, Entity);
        }
    }
}