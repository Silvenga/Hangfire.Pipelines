namespace Hangfire.Pipelines.Models
{
    public interface IPipelineStorage
    {
        T Get<T>(string key);
        void Set(string key, object value);
    }
}