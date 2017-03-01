using System;

using Hangfire.Pipelines.Executors;
using Hangfire.Pipelines.Storage;

namespace Hangfire.Pipelines.Core
{
    public delegate IPipelineStorage CreatePipelineStorage(Guid pipelineId);

    public delegate IStepExecutor CreateStepExecutor(Guid pipelineId);

    public static class Constants
    {
        public const string PipelineEntityKey = "PipelineEntity";
        public const string PipelineIdKey = "PipelineId";
        public const string StepName = "StepName";
    }
}