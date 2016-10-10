using System;

using JetBrains.Annotations;

namespace Hangfire.Pipelines.Models
{
    public class PipelineContext<T>
    {
        private readonly IPipelineStorage _pipelineStorage;

        [CanBeNull]
        public virtual T Entity { get; private set; }

        public Guid PipelineId { get; }

        public PipelineContext([NotNull] IPipelineStorage pipelineStorage, Guid pipelineId)
        {
            _pipelineStorage = pipelineStorage;
            PipelineId = pipelineId;
            Load();
        }

        public void Load()
        {
            Entity = _pipelineStorage.Get<T>(PipelineId, "");
        }

        public void Commit()
        {
            _pipelineStorage.Set(PipelineId, "", Entity);
        }
    }
}