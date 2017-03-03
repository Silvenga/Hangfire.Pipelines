using System;
using System.Threading.Tasks;

using Hangfire.MemoryStorage;
using Hangfire.Pipelines.Core;
using Hangfire.Pipelines.Executors;
using Hangfire.Pipelines.Helpers;
using Hangfire.Pipelines.Models;
using Hangfire.Pipelines.Storage;

namespace Hangfire.Pipelines.Sample
{
    public class Program
    {
        private static void Main()
        {
            var memory = new MemoryPipelineStorage();

            GlobalConfiguration.Configuration
                               .UseColouredConsoleLogProvider()
                               .UseMemoryStorage();
            GlobalConfiguration.Configuration
                               .UsePipelines(memory);

            // ReSharper disable once UnusedVariable
            var server = new BackgroundJobServer();

            Console.WriteLine("Start?");

            var client = new BackgroundJobClient();
            IStepExecutor stepExecutor = new HangfireStepExecutor(client);

            var testPipeline = new PipelineDefinition<string>(id => memory, id => stepExecutor);

            testPipeline.AddStep<TestStep, int>(x => x.Run())
                        .AddStep<TestStep2, string>(x => x.Run2());

            var executor = testPipeline.CreateExecutor();
            executor.Process("test");

            Console.WriteLine("Ready...");
            Console.ReadLine();

            //var memoryEx = new MemoryStepExecutor(memory);
            //var testPipeline = new PipelineDefinition<string>(memory, memoryEx);

            //testPipeline.AddStep<TestStep>(x => x.Run());
            //testPipeline.AddStep<TestStep>(x => x.Run2());

            //var executor = testPipeline.CreateExecutor();
            //executor.Process("test");

            //Console.WriteLine("Ready...");
            //Console.ReadLine();
        }
    }

    public class TestStep : IPipelineTask<string>
    {
        public IPipelineContext<string> PipelineContext { get; set; }

        public Task<int> Run()
        {
            Console.WriteLine($"Run {PipelineContext.Entity}");
            return Task.FromResult(5);
        }
    }

    public class TestStep2 : IPipelineTask<int>
    {
        public IPipelineContext<int> PipelineContext { get; set; }

        public string Run2()
        {
            Console.WriteLine($"Run2 {PipelineContext.Entity}");
            return "";
        }
    }
}