using Hangfire.Pipelines.Core;
using Hangfire.Pipelines.Executors;
using Hangfire.Pipelines.Storage;
using Hangfire.Pipelines.Tests.Models;

using Ploeh.AutoFixture;

using Xunit;

namespace Hangfire.Pipelines.Tests.Integrations
{
    public class MemoryExecutorFacts
    {
        private static readonly Fixture Autofixture = new Fixture();

        [Fact]
        public void Works()
        {
            var startInput = Autofixture.Create<string>();
            var intInput = Autofixture.Create<int>();
            var stringInput = Autofixture.Create<string>();

            var memory = new MemoryPipelineStorage();
            IStepExecutor stepExecutor = new MemoryStepExecutor(new PipelineInterceptor(), memory);
            var testPipeline = new PipelineDefinition<string>(id => memory, id => stepExecutor)
                .SetName("Test Pipeline")
                .AddStep<TestTasks.One, int>(x => x.RunIntAsync(), "Hello")
                .AddStep<TestTasks.Two, string>(x => x.RunString(), "Step 2");

            PipelineExecutor<string> executor = testPipeline.CreateExecutor();

            // Act
            executor.Process(startInput);

            // Assert
        }
    }
}