﻿using Hangfire.ActivationExtensions;
using Hangfire.MetaExtensions;
using Hangfire.Pipelines.Core;
using Hangfire.Pipelines.Storage;

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
        public static IGlobalConfiguration UsePipelines(this IGlobalConfiguration configuration, IPipelineStorage pipelineStorage)
        {
            var activationFilter = new HangfireActivatorInterceptor(pipelineStorage);
            configuration.UseDefaultActivatorInterceptor(activationFilter);
            configuration.UseMetaExtensions();

            return configuration;
        }
    }
}