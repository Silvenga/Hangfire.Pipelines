using Hangfire.Pipelines.Models;

using Xunit;

namespace Hangfire.Pipelines.Tests
{
    public class PipelineTemplateFacts
    {
        [Fact]
        public void Can_add_step()
        {
            var template = new PipelineTemplate<TypeFixture>();

            // Act
            //template.AddStep<StepFixture>(x => x.Instance());

            // Assert
        }
    }

    public class TypeFixture
    {
    }

    public class StepFixture : IPipelineTask<TypeFixture>
    {
        public void Instance()
        {
        }

        public PipelineContext<TypeFixture> PipelineContext { get; }
    }
}