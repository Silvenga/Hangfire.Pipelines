using JetBrains.Annotations;

namespace Hangfire.Pipelines.Models
{
    public class PipelineContext<T>
    {
        private readonly IPipelineStorage _pipelineStorage;

        [CanBeNull]
        public virtual T Entity { get; private set; }

        public PipelineContext([NotNull] IPipelineStorage pipelineStorage)
        {
            _pipelineStorage = pipelineStorage;
            Load();
        }

        public void Load()
        {
            Entity = _pipelineStorage.Get<T>("");
        }

        public void Commit()
        {
            _pipelineStorage.Set("", Entity);
        }
    }
}