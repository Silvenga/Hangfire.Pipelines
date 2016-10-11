using System;

using Hangfire.Pipelines.Core;
using Hangfire.Pipelines.Executors;
using Hangfire.Pipelines.Models;
using Hangfire.Pipelines.Storage;

namespace Hangfire.Pipelines.Sample
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var memory = new MemoryPipelineStorage();

            //GlobalConfiguration.Configuration
            //    .UseColouredConsoleLogProvider()
            //    .UseMemoryStorage();
            //GlobalConfiguration.Configuration
            //    .UsePipelines(memory);

            //var server = new BackgroundJobServer();

            //Console.WriteLine("Start?");

            //var client = new BackgroundJobClient();

            //var testPipeline = new PipelineDefinition<string>(memory);

            //testPipeline.AddStep<TestStep>(x => x.Run());
            //testPipeline.AddStep<TestStep>(x => x.Run2());

            //var executor = testPipeline.CreateExecutor();

            //IStepExecutor stepExecutor = new HangfireStepExecutor(client);
            //executor.Process("test", stepExecutor);

            //Console.WriteLine("Ready...");
            //Console.ReadLine();

            var testPipeline = new PipelineDefinition<string>(memory);
            var memoryEx = new MemoryStepExecutor(memory);

            testPipeline.AddStep<TestStep>(x => x.Run());
            testPipeline.AddStep<TestStep>(x => x.Run2());

            var executor = testPipeline.CreateExecutor();
            executor.Process("test", memoryEx);

            Console.WriteLine("Ready...");
            Console.ReadLine();
        }
    }

    public class TestStep : IPipelineTask<string>
    {
        public PipelineContext<string> PipelineContext { get; set; }

        public void Run()
        {
            PipelineContext.Entity = "test2";
        }

        public void Run2()
        {
        }
    }
}