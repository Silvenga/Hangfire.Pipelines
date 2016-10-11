using System;

using Hangfire.Pipelines.Storage;

using JetBrains.Annotations;

namespace Hangfire.Pipelines.Models
{
    public interface IPipelineContext
    {
        Guid PipelineId { get; }

        void Load();
        void Save();
    }

    public interface IPipelineContext<T> : IPipelineContext
    {
        T Entity { get; set; }
    }

    public class PipelineContext<T> : IPipelineContext<T>
    {
        private const string PipelineEntityKey = "PipelineEntity";
        private readonly IPipelineStorage _pipelineStorage;

        [CanBeNull]
        public T Entity { get; set; }

        public Guid PipelineId { get; }

        public PipelineContext([NotNull] IPipelineStorage pipelineStorage, Guid pipelineId)
        {
            _pipelineStorage = pipelineStorage;
            PipelineId = pipelineId;
        }

        public void Load()
        {
            Entity = _pipelineStorage.Get<T>(PipelineId, PipelineEntityKey);
        }

        public void Save()
        {
            _pipelineStorage.Set(PipelineId, PipelineEntityKey, Entity);
        }
    }
}