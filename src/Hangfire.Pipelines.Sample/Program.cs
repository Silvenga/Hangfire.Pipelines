using System;

using Hangfire.MemoryStorage;
using Hangfire.Pipelines.Core;
using Hangfire.Pipelines.Expressions;
using Hangfire.Pipelines.Helpers;
using Hangfire.Pipelines.Models;
using Hangfire.Pipelines.Storage;

namespace Hangfire.Pipelines.Sample
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var memory = new MemoryPipelineStorage();

            GlobalConfiguration.Configuration
                .UseColouredConsoleLogProvider()
                .UseMemoryStorage();
            GlobalConfiguration.Configuration
                .UsePipelines(memory);

            var server = new BackgroundJobServer();

            Console.WriteLine("Start?");

            var client = new BackgroundJobClient();

            IExpressionFactory factory = new HangfireExpressionFactory(client);


            var testPipeline = new PipelineDefinition<string>(factory, memory);

            testPipeline.AddStep<TestStep>(x => x.Run());
            testPipeline.AddStep<TestStep>(x => x.Run2());

            testPipeline.Process("test");

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