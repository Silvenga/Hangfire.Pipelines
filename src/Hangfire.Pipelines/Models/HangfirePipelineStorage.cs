namespace Hangfire.Pipelines.Models
{
    public class HangfirePipelineStorage : IPipelineStorage
    {
        private readonly JobActivatorContext _activatorContext;

        public HangfirePipelineStorage(JobActivatorContext activatorContext)
        {
            _activatorContext = activatorContext;
        }

        public T Get<T>(string key)
        {
            return _activatorContext.GetJobParameter<T>(key);
        }

        public void Set(string key, object value)
        {
            _activatorContext.SetJobParameter(key, value);
        }
    }
}