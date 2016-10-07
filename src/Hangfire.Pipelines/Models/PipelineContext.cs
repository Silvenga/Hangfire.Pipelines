namespace Hangfire.Pipelines.Models
{
    public class PipelineContext<T>
    {
        public virtual T Entity { get; }

        public PipelineContext(T entity)
        {
            Entity = entity;
        }
    }
}