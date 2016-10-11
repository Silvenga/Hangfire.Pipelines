namespace Hangfire.Pipelines.Models
{
    public interface IPipelineTask<T>
    {
        IPipelineContext<T> PipelineContext { get; set; }
    }
}