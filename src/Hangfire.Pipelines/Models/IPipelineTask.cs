namespace Hangfire.Pipelines.Models
{
    public interface IPipelineTask<T>
    {
        PipelineContext<T> PipelineContext { get; set; }
    }
}