using System;

using Hangfire.Pipelines.Storage;

using JetBrains.Annotations;

namespace Hangfire.Pipelines.Models
{
    public class PipelineContext<T> : IBasicPipelineContext
    {
        private const string PipelineEntityKey = "PipelineEntity";
        private readonly IPipelineStorage _pipelineStorage;

        [CanBeNull]
        public virtual T Entity { get; set; }

        public virtual Guid PipelineId { get; }

        public PipelineContext([NotNull] IPipelineStorage pipelineStorage, Guid pipelineId)
        {
            _pipelineStorage = pipelineStorage;
            PipelineId = pipelineId;
        }

        public virtual void Load()
        {
            Entity = _pipelineStorage.Get<T>(PipelineId, PipelineEntityKey);
        }

        public virtual void Save()
        {
            _pipelineStorage.Set(PipelineId, PipelineEntityKey, Entity);
        }
    }

    public interface IBasicPipelineContext
    {
        void Load();
        void Save();
    }
}