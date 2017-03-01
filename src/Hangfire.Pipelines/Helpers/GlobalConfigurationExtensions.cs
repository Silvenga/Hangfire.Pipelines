using Hangfire.ActivationExtensions;
using Hangfire.MetaExtensions;
using Hangfire.Pipelines.Core;
using Hangfire.Pipelines.Storage;

using JetBrains.Annotations;

namespace Hangfire.Pipelines.Helpers
{
    public static class GlobalConfigurationExtensions
    {
        /// <summary>
        /// Ntoe: This should be called after job activator configuration. 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="pipelineStorage"></param>
        /// <returns></returns>
        public static IGlobalConfiguration UsePipelines([NotNull] this IGlobalConfiguration configuration, [NotNull] IPipelineStorage pipelineStorage)
        {
            var activationFilter = new HangfireActivatorInterceptor(new PipelineInterceptor(), pipelineStorage);
            configuration.UseDefaultActivatorInterceptor(activationFilter);
            configuration.UseMetaExtensions();

            return configuration;
        }
    }
}